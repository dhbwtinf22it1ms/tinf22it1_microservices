using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dhbw.ThesisManager.Api.Configuration;
using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Models.Responses;
using Microsoft.Extensions.Options;

namespace Dhbw.ThesisManager.Api.Services
{
    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakOptions _options;
        private string _adminAccessToken;
        private DateTime _tokenExpiry;
        private readonly JsonSerializerOptions _jsonOptions;

        public KeycloakService(HttpClient httpClient, IOptions<KeycloakOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        private async Task EnsureAdminTokenAsync(CancellationToken cancellationToken)
        {
            if (_adminAccessToken != null && DateTime.UtcNow < _tokenExpiry)
                return;

            var tokenEndpoint = $"{_options.AdminUrl}/realms/master/protocol/openid-connect/token";
            
            // Use password grant flow with admin credentials instead of client credentials
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _options.AdminClientId,
                ["username"] = "admin", // Using the admin user from docker-compose
                ["password"] = "admin"  // Using the admin password from docker-compose
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Failed to obtain admin token. Status: {response.StatusCode}, Error: {errorContent}");
            }

            var tokenResponse = await JsonSerializer.DeserializeAsync<JsonElement>(
                await response.Content.ReadAsStreamAsync(cancellationToken), 
                cancellationToken: cancellationToken);

            _adminAccessToken = tokenResponse.GetProperty("access_token").GetString();
            var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 30); // Buffer of 30 seconds
        }

        private async Task<T> SendAdminRequestAsync<T>(
            HttpMethod method, 
            string endpoint, 
            object content = null,
            CancellationToken cancellationToken = default)
        {
            await EnsureAdminTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(method, $"{_options.AdminUrl}/admin/realms/{_options.Realm}/{endpoint}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            if (content != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(content, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Keycloak API request failed. Status: {response.StatusCode}, Error: {errorContent}");
            }

            // Special case for user creation - Keycloak returns 201 Created with Location header
            if (method == HttpMethod.Post && endpoint == "users" && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                if (typeof(T) == typeof(KeycloakUser) && response.Headers.Location != null)
                {
                    // Extract user ID from Location header
                    // Location format: http://localhost:8080/admin/realms/thesis-management/users/{userId}
                    var locationPath = response.Headers.Location.AbsolutePath;
                    var userId = locationPath.Substring(locationPath.LastIndexOf('/') + 1);
                    
                    // Create a KeycloakUser with just the ID
                    var user = new KeycloakUser { Id = userId };
                    return (T)(object)user;
                }
            }

            if (method == HttpMethod.Delete || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            // For other cases, try to deserialize the response content
            try
            {
                var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);
                if (responseContent.Length > 0)
                {
                    return await JsonSerializer.DeserializeAsync<T>(responseContent, _jsonOptions, cancellationToken);
                }
                return default;
            }
            catch (JsonException)
            {
                // If deserialization fails (e.g., empty response), return default
                return default;
            }
        }

        public async Task<UserListResponse> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Get all users
                var users = await SendAdminRequestAsync<List<KeycloakUser>>(
                    HttpMethod.Get, 
                    "users", 
                    cancellationToken: cancellationToken);

                // For each user, get their groups
                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.Id))
                        continue;

                    var groups = await SendAdminRequestAsync<List<KeycloakGroup>>(
                        HttpMethod.Get,
                        $"users/{user.Id}/groups",
                        cancellationToken: cancellationToken);

                    user.Groups = groups.Select(g => g.Name).ToArray();
                }

                return new UserListResponse 
                { 
                    Ok = users.Select(u => MapToUser(u)).ToList() 
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get users from Keycloak: {ex.Message}", ex);
            }
        }

        public async Task<UserIdListResponse> CreateUsersAsync(IEnumerable<UserCreationRequest> users, CancellationToken cancellationToken = default)
        {
            var userIds = new List<long>();
            
            foreach (var request in users)
            {
                try
                {
                    // 1. Create the user
                    var keycloakUser = new KeycloakUser
                    {
                        Username = request.User.Email,
                        Email = request.User.Email,
                        FirstName = request.User.FirstName,
                        LastName = request.User.LastName,
                        Enabled = true,
                        EmailVerified = true
                    };

                    var createdUser = await SendAdminRequestAsync<KeycloakUser>(
                        HttpMethod.Post, 
                        "users", 
                        keycloakUser,
                        cancellationToken);

                    if (string.IsNullOrEmpty(createdUser?.Id))
                        throw new Exception("Failed to create user in Keycloak");

                    // 2. Add user to the appropriate group based on role
                    // First, get all groups to find the group ID
                    var groups = await SendAdminRequestAsync<List<KeycloakGroup>>(
                        HttpMethod.Get,
                        "groups",
                        cancellationToken: cancellationToken);

                    // Find the group that matches the user's role
                    var targetGroupName = request.User.Role.ToString();
                    var targetGroup = groups.FirstOrDefault(g => g.Name.Equals(targetGroupName, StringComparison.OrdinalIgnoreCase));
                    
                    if (targetGroup == null)
                        throw new Exception($"Group for role {targetGroupName} not found in Keycloak");

                    // Add user to the group using the group ID
                    await SendAdminRequestAsync<object>(
                        HttpMethod.Put,
                        $"users/{createdUser.Id}/groups/{targetGroup.Id}",
                        cancellationToken: cancellationToken);

                    // 3. Set temporary password if needed
                    if (!string.IsNullOrEmpty(request.RegistrationToken))
                    {
                        var credential = new KeycloakCredential
                        {
                            Type = "password",
                            Value = request.RegistrationToken,
                            Temporary = true
                        };

                        await SendAdminRequestAsync<object>(
                            HttpMethod.Put,
                            $"users/{createdUser.Id}/reset-password",
                            credential,
                            cancellationToken);
                    }

                    // Convert UUID to a deterministic long ID
                    var idBytes = Guid.Parse(createdUser.Id).ToByteArray();
                    var idHash = Math.Abs(BitConverter.ToInt64(idBytes, 0));
                    userIds.Add(idHash);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create user in Keycloak: {ex.Message}", ex);
                }
            }

            return new UserIdListResponse { Ok = userIds };
        }

        public async Task<UserResponse> GetUserAsync(long userId, string registrationToken = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get all users first
                var allUsers = await SendAdminRequestAsync<List<KeycloakUser>>(
                    HttpMethod.Get, 
                    "users", 
                    cancellationToken: cancellationToken);

                // Find the user with the matching hashed ID
                KeycloakUser matchingUser = null;
                foreach (var user in allUsers)
                {
                    if (string.IsNullOrEmpty(user.Id))
                        continue;

                    var idBytes = Guid.Parse(user.Id).ToByteArray();
                    var idHash = Math.Abs(BitConverter.ToInt64(idBytes, 0));
                    
                    if (idHash == userId)
                    {
                        matchingUser = user;
                        break;
                    }
                }

                if (matchingUser == null)
                    throw new Exception($"User with ID {userId} not found in Keycloak");

                // Get user groups
                var groups = await SendAdminRequestAsync<List<KeycloakGroup>>(
                    HttpMethod.Get,
                    $"users/{matchingUser.Id}/groups",
                    cancellationToken: cancellationToken);

                matchingUser.Groups = groups.Select(g => g.Name).ToArray();

                return new UserResponse { Ok = MapToUser(matchingUser) };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user from Keycloak: {ex.Message}", ex);
            }
        }

        public async Task<UserResponse> UpdateUserAsync(long userId, User user, CancellationToken cancellationToken = default)
        {
            try
            {
                // Find the Keycloak UUID for this user
                var keycloakId = await GetKeycloakIdFromHashedId(userId, cancellationToken);
                if (string.IsNullOrEmpty(keycloakId))
                    throw new Exception($"User with ID {userId} not found in Keycloak");

                // 1. Update user details
                var keycloakUser = new KeycloakUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailVerified = true
                };

                await SendAdminRequestAsync<object>(
                    HttpMethod.Put, 
                    $"users/{keycloakId}", 
                    keycloakUser,
                    cancellationToken);

                // 2. Get current user groups
                var currentGroups = await SendAdminRequestAsync<List<KeycloakGroup>>(
                    HttpMethod.Get,
                    $"users/{keycloakId}/groups",
                    cancellationToken: cancellationToken);

                // 3. Update group membership if role has changed
                var targetGroupName = user.Role.ToString();
                var isInTargetGroup = currentGroups.Any(g => g.Name.Equals(targetGroupName, StringComparison.OrdinalIgnoreCase));

                if (!isInTargetGroup)
                {
                    // Get all groups to find the target group ID
                    var allGroups = await SendAdminRequestAsync<List<KeycloakGroup>>(
                        HttpMethod.Get,
                        "groups",
                        cancellationToken: cancellationToken);

                    // Find the group that matches the user's role
                    var targetGroup = allGroups.FirstOrDefault(g => g.Name.Equals(targetGroupName, StringComparison.OrdinalIgnoreCase));
                    
                    if (targetGroup == null)
                        throw new Exception($"Group for role {targetGroupName} not found in Keycloak");

                    // Add user to the target group using the group ID
                    await SendAdminRequestAsync<object>(
                        HttpMethod.Put,
                        $"users/{keycloakId}/groups/{targetGroup.Id}",
                        cancellationToken: cancellationToken);
                    
                    // Remove user from any role-specific groups that don't match the target role
                    // This preserves membership in non-role groups
                    var roleGroups = new[] { "student", "supervisor", "administrator", "secretary" };
                    foreach (var group in currentGroups)
                    {
                        if (roleGroups.Contains(group.Name.ToLower()) && 
                            !group.Name.Equals(targetGroupName, StringComparison.OrdinalIgnoreCase))
                        {
                            await SendAdminRequestAsync<object>(
                                HttpMethod.Delete,
                                $"users/{keycloakId}/groups/{group.Id}",
                                cancellationToken: cancellationToken);
                        }
                    }
                }

                return await GetUserAsync(userId, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user in Keycloak: {ex.Message}", ex);
            }
        }

        public async Task<EmptyResponse> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Find the Keycloak UUID for this user
                var keycloakId = await GetKeycloakIdFromHashedId(userId, cancellationToken);
                if (string.IsNullOrEmpty(keycloakId))
                    throw new Exception($"User with ID {userId} not found in Keycloak");

                await SendAdminRequestAsync<object>(
                    HttpMethod.Delete, 
                    $"users/{keycloakId}", 
                    cancellationToken: cancellationToken);

                return new EmptyResponse { Ok = null };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete user from Keycloak: {ex.Message}", ex);
            }
        }

        public async Task<RegistrationTokenResponse> RegenerateRegistrationTokenAsync(long userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Find the Keycloak UUID for this user
                var keycloakId = await GetKeycloakIdFromHashedId(userId, cancellationToken);
                if (string.IsNullOrEmpty(keycloakId))
                    throw new Exception($"User with ID {userId} not found in Keycloak");

                // Generate a temporary password
                var tempPassword = Guid.NewGuid().ToString();
                
                var credential = new KeycloakCredential
                {
                    Type = "password",
                    Value = tempPassword,
                    Temporary = true
                };

                await SendAdminRequestAsync<object>(
                    HttpMethod.Put,
                    $"users/{keycloakId}/reset-password",
                    credential,
                    cancellationToken);

                // Send required actions email
                await SendAdminRequestAsync<object>(
                    HttpMethod.Put,
                    $"users/{keycloakId}/execute-actions-email",
                    new[] { "UPDATE_PASSWORD" },
                    cancellationToken);

                return new RegistrationTokenResponse { Ok = tempPassword };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to regenerate registration token: {ex.Message}", ex);
            }
        }

        // Helper method to find the Keycloak UUID from our hashed ID
        private async Task<string> GetKeycloakIdFromHashedId(long hashedId, CancellationToken cancellationToken = default)
        {
            // Get all users
            var allUsers = await SendAdminRequestAsync<List<KeycloakUser>>(
                HttpMethod.Get, 
                "users", 
                cancellationToken: cancellationToken);

            // Find the user with the matching hashed ID
            foreach (var user in allUsers)
            {
                if (string.IsNullOrEmpty(user.Id))
                    continue;

                var idBytes = Guid.Parse(user.Id).ToByteArray();
                var idHash = Math.Abs(BitConverter.ToInt64(idBytes, 0));
                
                if (idHash == hashedId)
                {
                    return user.Id;
                }
            }

            return null; // User not found
        }

        private User MapToUser(KeycloakUser keycloakUser)
        {
            if (keycloakUser == null)
                return null;

            var role = UserRole.student; // Default role
            
            if (keycloakUser.Groups != null && keycloakUser.Groups.Any())
            {
                // Try to parse the group name to a UserRole
                var groupName = keycloakUser.Groups.FirstOrDefault();
                if (!string.IsNullOrEmpty(groupName) && 
                    Enum.TryParse<UserRole>(groupName, true, out var parsedRole))
                {
                    role = parsedRole;
                }
            }

            // Generate a deterministic ID from the UUID
            var idBytes = Guid.Parse(keycloakUser.Id).ToByteArray();
            var idHash = Math.Abs(BitConverter.ToInt64(idBytes, 0));

            return new User
            {
                Id = idHash, // Use a hash of the UUID as the ID
                FirstName = keycloakUser.FirstName ?? string.Empty,
                LastName = keycloakUser.LastName ?? string.Empty,
                Email = keycloakUser.Email ?? keycloakUser.Username ?? string.Empty,
                Role = role,
                RegistrationStatus = new RegistrationStatus 
                { 
                    Pending = keycloakUser.EmailVerified ? null : "true" 
                }
            };
        }

        private class KeycloakUser
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public bool Enabled { get; set; }
            public bool EmailVerified { get; set; }
            public string[] Groups { get; set; }
        }

        private class KeycloakGroup
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
        }

        private class KeycloakCredential
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public bool Temporary { get; set; }
        }

        private class KeycloakActionToken
        {
            public string ActionToken { get; set; }
        }
    }
}
