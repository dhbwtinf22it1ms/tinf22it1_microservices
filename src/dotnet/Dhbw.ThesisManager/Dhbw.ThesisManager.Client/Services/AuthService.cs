using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Dhbw.ThesisManager.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly string _keycloakUrl;
        private readonly string _realm;
        private readonly string _clientId;
        private AuthState? _authState;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            
            // These values should come from configuration
            _keycloakUrl = "http://localhost:8080";
            _realm = "thesis-management";
            _clientId = "thesis-management-webapp";
        }

        public bool IsAuthenticated => _authState != null && !string.IsNullOrEmpty(_authState.AccessToken) && _authState.ExpiresAt > DateTime.Now;

        public string? AccessToken => _authState?.AccessToken;

        public string? RefreshToken => _authState?.RefreshToken;

        public UserInfo? CurrentUser => _authState?.UserInfo;

        public async Task InitializeAsync()
        {
            // Try to load auth state from local storage
            var storedAuthState = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_state");
            if (!string.IsNullOrEmpty(storedAuthState))
            {
                try
                {
                    _authState = JsonSerializer.Deserialize<AuthState>(storedAuthState);
                    
                    // Check if token is expired or about to expire
                    if (_authState != null && _authState.ExpiresAt <= DateTime.Now.AddMinutes(5))
                    {
                        await RefreshTokenAsync();
                    }
                }
                catch
                {
                    // If there's an error deserializing, clear the stored state
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state");
                    _authState = null;
                }
            }
        }

        public async Task LoginAsync()
        {
            // Generate a random state for CSRF protection
            var state = Guid.NewGuid().ToString();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_state_param", state);

            // Build the authorization URL
            var authUrl = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/auth" +
                          $"?client_id={_clientId}" +
                          $"&redirect_uri={Uri.EscapeDataString(_httpClient.BaseAddress + "authentication/callback")}" +
                          $"&response_type=code" +
                          $"&scope=openid profile email" +
                          $"&state={state}";

            // Redirect to Keycloak login page
            await _jsRuntime.InvokeVoidAsync("window.location.assign", authUrl);
        }

        public async Task HandleCallbackAsync(string code, string state)
        {
            // Verify state parameter to prevent CSRF
            var storedState = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_state_param");
            if (string.IsNullOrEmpty(storedState) || storedState != state)
            {
                throw new InvalidOperationException("Invalid state parameter");
            }

            // Clear the stored state
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state_param");

            // Exchange authorization code for tokens
            var tokenEndpoint = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = _clientId,
                ["code"] = code,
                ["redirect_uri"] = $"{_httpClient.BaseAddress}authentication/callback"
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to exchange code for tokens: {response.StatusCode}");
            }

            var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
                await response.Content.ReadAsStreamAsync());

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to parse token response");
            }

            // Parse the access token to get user info
            var userInfo = ParseAccessToken(tokenResponse.AccessToken);

            // Store the auth state
            _authState = new AuthState
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                UserInfo = userInfo
            };

            // Save to local storage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_state", 
                JsonSerializer.Serialize(_authState));
        }

        public async Task LogoutAsync()
        {
            if (_authState == null)
                return;

            // Call Keycloak logout endpoint
            var logoutUrl = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/logout" +
                           $"?client_id={_clientId}" +
                           $"&refresh_token={Uri.EscapeDataString(_authState.RefreshToken ?? string.Empty)}";

            try
            {
                await _httpClient.GetAsync(logoutUrl);
            }
            catch
            {
                // Ignore errors during logout
            }

            // Clear local auth state
            _authState = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state");
        }

        public async Task RefreshTokenAsync()
        {
            if (_authState == null || string.IsNullOrEmpty(_authState.RefreshToken))
                return;

            var tokenEndpoint = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _clientId,
                ["refresh_token"] = _authState.RefreshToken
            });

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    // If refresh fails, clear auth state and redirect to login
                    _authState = null;
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state");
                    return;
                }

                var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
                    await response.Content.ReadAsStreamAsync());

                if (tokenResponse == null)
                    return;

                // Parse the access token to get user info
                var userInfo = ParseAccessToken(tokenResponse.AccessToken);

                // Update the auth state
                _authState = new AuthState
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                    UserInfo = userInfo
                };

                // Save to local storage
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_state", 
                    JsonSerializer.Serialize(_authState));
            }
            catch
            {
                // If refresh fails, clear auth state
                _authState = null;
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state");
            }
        }

        public HttpClient CreateAuthorizedClient()
        {
            var client = new HttpClient
            {
                BaseAddress = _httpClient.BaseAddress
            };

            if (IsAuthenticated && !string.IsNullOrEmpty(_authState?.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
            }

            return client;
        }

        private UserInfo ParseAccessToken(string accessToken)
        {
            try
            {
                // JWT tokens consist of three parts: header.payload.signature
                var parts = accessToken.Split('.');
                if (parts.Length != 3)
                    return new UserInfo();

                // Decode the payload (second part)
                var payload = parts[1];
                
                // Add padding if needed
                while (payload.Length % 4 != 0)
                {
                    payload += "=";
                }
                
                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);
                
                var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
                if (claims == null)
                    return new UserInfo();

                var userInfo = new UserInfo();
                
                if (claims.TryGetValue("sub", out var sub))
                    userInfo.Id = sub.GetString();
                
                if (claims.TryGetValue("preferred_username", out var username))
                    userInfo.Username = username.GetString();
                
                if (claims.TryGetValue("email", out var email))
                    userInfo.Email = email.GetString();
                
                if (claims.TryGetValue("given_name", out var firstName))
                    userInfo.FirstName = firstName.GetString();
                
                if (claims.TryGetValue("family_name", out var lastName))
                    userInfo.LastName = lastName.GetString();
                
                // Initialize roles list
                var rolesList = new List<string>();
                
                // Check for groups claim (used in Keycloak for group membership)
                if (claims.TryGetValue("groups", out var groups))
                {
                    foreach (var group in groups.EnumerateArray())
                    {
                        var groupName = group.GetString();
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            // Extract the group name from the path (e.g., "/GroupName" -> "GroupName")
                            var name = groupName.TrimStart('/');
                            rolesList.Add(name);
                        }
                    }
                }
                
                // Also check for roles in realm_access for backward compatibility
                if (claims.TryGetValue("realm_access", out var realmAccess) && 
                    realmAccess.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        rolesList.Add(role.GetString() ?? string.Empty);
                    }
                }
                
                userInfo.Roles = rolesList;

                return userInfo;
            }
            catch
            {
                return new UserInfo();
            }
        }
    }

    public class AuthState
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserInfo? UserInfo { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public bool HasRole(string role)
        {
            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}
