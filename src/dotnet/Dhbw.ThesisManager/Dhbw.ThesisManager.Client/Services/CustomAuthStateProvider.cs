using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Dhbw.ThesisManager.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Create an anonymous user by default (not authenticated)
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = new AuthenticationState(anonymousUser);

            try
            {
                // Check if we're running in a browser context
                // This prevents JS interop calls during server-side rendering
                if (!_httpClient.BaseAddress?.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ?? true)
                {
                    return authState;
                }

                // Try to load auth state from local storage
                var storedAuthState = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_state");
                if (string.IsNullOrEmpty(storedAuthState))
                {
                    return authState;
                }

                // Parse the token response
                // Use dynamic to handle both snake_case and PascalCase property names
                var tokenResponseObj = JsonSerializer.Deserialize<JsonElement>(storedAuthState);
                string? accessToken = null;
                int expiresIn = 0;
                
                // Try to get access_token (snake_case) or AccessToken (PascalCase)
                if (tokenResponseObj.TryGetProperty("access_token", out var atSnake))
                {
                    accessToken = atSnake.GetString();
                }
                else if (tokenResponseObj.TryGetProperty("AccessToken", out var atPascal))
                {
                    accessToken = atPascal.GetString();
                }
                
                // Try to get expires_in (snake_case) or ExpiresIn (PascalCase)
                if (tokenResponseObj.TryGetProperty("expires_in", out var eiSnake))
                {
                    expiresIn = eiSnake.GetInt32();
                }
                else if (tokenResponseObj.TryGetProperty("ExpiresIn", out var eiPascal))
                {
                    expiresIn = eiPascal.GetInt32();
                }
                
                if (string.IsNullOrEmpty(accessToken))
                {
                    return authState;
                }

                // Check if token is expired
                var expiresAt = DateTime.Now.AddSeconds(expiresIn);
                if (expiresAt <= DateTime.Now)
                {
                    // Token is expired, clear auth state
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_state");
                    return authState;
                }

                // Parse the access token to get user info
                var userInfo = ParseAccessToken(accessToken);
                if (userInfo == null)
                {
                    return authState;
                }

                // Create claims identity
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id ?? string.Empty),
                    new Claim(ClaimTypes.Name, userInfo.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, userInfo.Email ?? string.Empty),
                    new Claim(ClaimTypes.GivenName, userInfo.FirstName ?? string.Empty),
                    new Claim(ClaimTypes.Surname, userInfo.LastName ?? string.Empty),
                }, "Keycloak");

                // Add role claims
                foreach (var role in userInfo.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                // If there's an error, return an anonymous user
                return authState;
            }
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
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

}
