using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

        public KeycloakService(HttpClient httpClient, IOptions<KeycloakOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        private async Task EnsureAdminTokenAsync(CancellationToken cancellationToken)
        {
            if (_adminAccessToken != null && DateTime.UtcNow < _tokenExpiry)
                return;

            var tokenEndpoint = $"{_options.AdminUrl}/realms/master/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.AdminClientId,
                ["client_secret"] = _options.AdminClientSecret
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

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
                    JsonSerializer.Serialize(content),
                    Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (method == HttpMethod.Delete)
                return default;

            return await JsonSerializer.DeserializeAsync<T>(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                cancellationToken: cancellationToken);
        }

        public async Task<UserListResponse> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await SendAdminRequestAsync<List<KeycloakUser>>(HttpMethod.Get, "users", cancellationToken: cancellationToken);
            return new UserListResponse 
            { 
                Ok = users.Select(u => MapToUser(u)).ToList() 
            };
        }

        public async Task<UserIdListResponse> CreateUsersAsync(IEnumerable<UserCreationRequest> users, CancellationToken cancellationToken = default)
        {
            var userIds = new List<long>();
            foreach (var request in users)
            {
                var keycloakUser = new KeycloakUser
                {
                    Username = request.User.Email,
                    Email = request.User.Email,
                    FirstName = request.User.FirstName,
                    LastName = request.User.LastName,
                    Enabled = true,
                    Groups = new[] { request.User.Role.ToString() }
                };

                var createdUser = await SendAdminRequestAsync<KeycloakUser>(
                    HttpMethod.Post, 
                    "users", 
                    keycloakUser,
                    cancellationToken);

                userIds.Add(long.Parse(createdUser.Id));
            }

            return new UserIdListResponse { Ok = userIds };
        }

        public async Task<UserResponse> GetUserAsync(long userId, string registrationToken = null, CancellationToken cancellationToken = default)
        {
            var user = await SendAdminRequestAsync<KeycloakUser>(
                HttpMethod.Get, 
                $"users/{userId}", 
                cancellationToken: cancellationToken);

            return new UserResponse { Ok = MapToUser(user) };
        }

        public async Task<UserResponse> UpdateUserAsync(long userId, User user, CancellationToken cancellationToken = default)
        {
            var keycloakUser = new KeycloakUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Groups = new[] { user.Role.ToString() }
            };

            await SendAdminRequestAsync<object>(
                HttpMethod.Put, 
                $"users/{userId}", 
                keycloakUser,
                cancellationToken);

            return await GetUserAsync(userId, cancellationToken: cancellationToken);
        }

        public async Task<EmptyResponse> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            await SendAdminRequestAsync<object>(
                HttpMethod.Delete, 
                $"users/{userId}", 
                cancellationToken: cancellationToken);

            return new EmptyResponse { Ok = null };
        }

        public async Task<RegistrationTokenResponse> RegenerateRegistrationTokenAsync(long userId, CancellationToken cancellationToken = default)
        {
            // Generate a new registration token
            var token = await SendAdminRequestAsync<KeycloakActionToken>(
                HttpMethod.Post, 
                $"users/{userId}/execute-actions-email", 
                new[] { "UPDATE_PASSWORD" },
                cancellationToken);

            return new RegistrationTokenResponse { Ok = token.ActionToken };
        }

        private User MapToUser(KeycloakUser keycloakUser)
        {
            return new User
            {
                Id = long.Parse(keycloakUser.Id),
                FirstName = keycloakUser.FirstName,
                LastName = keycloakUser.LastName,
                Email = keycloakUser.Email,
                Role = Enum.Parse<UserRole>(keycloakUser.Groups?.FirstOrDefault() ?? "Student")
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
            public string[] Groups { get; set; }
        }

        private class KeycloakActionToken
        {
            public string ActionToken { get; set; }
        }
    }
}
