using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Dhbw.ThesisManager.Api.Configuration;
using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Dhbw.ThesisManager.Api.Tests.Services;

public class KeycloakServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly KeycloakService _keycloakService;

    public KeycloakServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _keycloakOptions = new KeycloakOptions
        {
            AdminUrl = "http://localhost:8080",
            Realm = "thesis-management",
            AdminClientId = "admin-cli",
            AdminClientSecret = "admin-secret"
        };
        
        var options = new Mock<IOptions<KeycloakOptions>>();
        options.Setup(o => o.Value).Returns(_keycloakOptions);
        
        _keycloakService = new KeycloakService(_httpClient, options.Object);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsers_WhenKeycloakReturnsUsers()
    {
        // Arrange
        var keycloakUsers = new[]
        {
            new
            {
                id = "1",
                username = "john.doe@example.com",
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                enabled = true,
                groups = new[] { "Student" }
            },
            new
            {
                id = "2",
                username = "jane.smith@example.com",
                firstName = "Jane",
                lastName = "Smith",
                email = "jane.smith@example.com",
                enabled = true,
                groups = new[] { "Supervisor" }
            }
        };

        // Setup token response
        SetupTokenResponse();
        
        // Setup users response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(keycloakUsers), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _keycloakService.GetUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Ok.Count);
        Assert.Equal(1, result.Ok[0].Id);
        Assert.Equal("John", result.Ok[0].FirstName);
        Assert.Equal("Doe", result.Ok[0].LastName);
        Assert.Equal("john.doe@example.com", result.Ok[0].Email);
        Assert.Equal(UserRole.student, result.Ok[0].Role);
        
        Assert.Equal(2, result.Ok[1].Id);
        Assert.Equal("Jane", result.Ok[1].FirstName);
        Assert.Equal("Smith", result.Ok[1].LastName);
        Assert.Equal("jane.smith@example.com", result.Ok[1].Email);
        Assert.Equal(UserRole.supervisor, result.Ok[1].Role);
    }

    [Fact]
    public async Task GetUserAsync_ShouldReturnUser_WhenKeycloakReturnsUser()
    {
        // Arrange
        var keycloakUsers = new[]
        {
            new
            {
                id = "00000000-0000-0000-0000-000000000001", // This ID will hash to 1
                username = "john.doe@example.com",
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                enabled = true,
                emailVerified = true
            }
        };

        var userGroups = new[]
        {
            new
            {
                id = "group1",
                name = "Student",
                path = "/Student"
            }
        };

        // Setup token response
        SetupTokenResponse();
        
        // Setup users list response (now we get all users first)
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(keycloakUsers), Encoding.UTF8, "application/json")
            });
            
        // Setup user groups response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/groups")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(userGroups), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _keycloakService.GetUserAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Ok.Id);
        Assert.Equal("John", result.Ok.FirstName);
        Assert.Equal("Doe", result.Ok.LastName);
        Assert.Equal("john.doe@example.com", result.Ok.Email);
        Assert.Equal(UserRole.student, result.Ok.Role);
    }

    [Fact]
    public async Task CreateUsersAsync_ShouldCreateUsers_WhenKeycloakAcceptsUsers()
    {
        // Arrange
        var userRequests = new List<UserCreationRequest>
        {
            new() 
            { 
                User = new User 
                { 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john.doe@example.com",
                    Role = UserRole.student 
                },
                RegistrationToken = "token123"
            }
        };

        // Setup token response
        SetupTokenResponse();
        
        // Setup create user response with Location header
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created
        };
        responseMessage.Headers.Location = new Uri("http://localhost:8080/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001");
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);
            
        // Setup groups response
        var groups = new[]
        {
            new
            {
                id = "group1",
                name = "Student",
                path = "/Student"
            }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/groups")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(groups), Encoding.UTF8, "application/json")
            });
            
        // Setup add to group response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/groups/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });
            
        // Setup reset password response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/reset-password")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _keycloakService.CreateUsersAsync(userRequests);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Ok);
        // The ID is a hash of the UUID, which should be 1 for our test UUID
        Assert.Equal(1, result.Ok[0]);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser_WhenKeycloakAcceptsUpdate()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Role = UserRole.student
        };

        // Setup token response
        SetupTokenResponse();
        
        // Setup get all users response (for finding the Keycloak UUID)
        var keycloakUsers = new[]
        {
            new
            {
                id = "00000000-0000-0000-0000-000000000001", // This ID will hash to 1
                username = "john.doe@example.com",
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                enabled = true,
                emailVerified = true
            }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(keycloakUsers), Encoding.UTF8, "application/json")
            });
        
        // Setup update user response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });
        
        // Setup get user groups response
        var currentGroups = new[]
        {
            new
            {
                id = "group1",
                name = "Student",
                path = "/Student"
            }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/groups")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(currentGroups), Encoding.UTF8, "application/json")
            });
        
        // Act
        var result = await _keycloakService.UpdateUserAsync(1, user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Ok.Id);
        Assert.Equal("John", result.Ok.FirstName);
        Assert.Equal("Doe", result.Ok.LastName);
        Assert.Equal("john.doe@example.com", result.Ok.Email);
        Assert.Equal(UserRole.student, result.Ok.Role);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenKeycloakAcceptsDelete()
    {
        // Arrange
        // Setup token response
        SetupTokenResponse();
        
        // Setup get all users response (for finding the Keycloak UUID)
        var keycloakUsers = new[]
        {
            new
            {
                id = "00000000-0000-0000-0000-000000000001", // This ID will hash to 1
                username = "john.doe@example.com",
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                enabled = true,
                emailVerified = true
            }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(keycloakUsers), Encoding.UTF8, "application/json")
            });
        
        // Setup delete user response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _keycloakService.DeleteUserAsync(1);

        // Assert
        Assert.NotNull(result);
        
        // Verify the delete request was sent
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001")),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task RegenerateRegistrationTokenAsync_ShouldRegenerateToken_WhenKeycloakAcceptsRequest()
    {
        // Arrange
        // Setup token response
        SetupTokenResponse();
        
        // Setup get all users response (for finding the Keycloak UUID)
        var keycloakUsers = new[]
        {
            new
            {
                id = "00000000-0000-0000-0000-000000000001", // This ID will hash to 1
                username = "john.doe@example.com",
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com",
                enabled = true,
                emailVerified = true
            }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(keycloakUsers), Encoding.UTF8, "application/json")
            });
        
        // Setup reset password response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/reset-password")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });
        
        // Setup execute-actions-email response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users/00000000-0000-0000-0000-000000000001/execute-actions-email")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _keycloakService.RegenerateRegistrationTokenAsync(1);

        // Assert
        Assert.NotNull(result);
        // The new implementation generates a GUID as the token
        Assert.NotNull(result.Ok);
        Assert.NotEmpty(result.Ok);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldThrowException_WhenKeycloakReturnsError()
    {
        // Arrange
        // Setup token response
        SetupTokenResponse();
        
        // Setup users response with error
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error", Encoding.UTF8, "application/json")
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _keycloakService.GetUsersAsync());
    }

    [Fact]
    public async Task GetUserAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        // Setup token response
        SetupTokenResponse();
        
        // Setup empty users list response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/admin/realms/thesis-management/users")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _keycloakService.GetUserAsync(999));
    }

    private void SetupTokenResponse()
    {
        var tokenResponse = new
        {
            access_token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJfT2RwZDRPZnZBbzF4QmRnVGJWMmJtLWxLcmJTQ3R5UXRpZEJSWFU4UmxFIn0...",
            expires_in = 300,
            refresh_expires_in = 1800,
            refresh_token = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJhODliNTgwYS1iNWFlLTRiZTQtODFlYi0yYjhlYzlhOGJmZDQifQ...",
            token_type = "Bearer",
            not_before_policy = 0,
            session_state = "a856fb91-eabc-4159-bbcb-52d00e5b0cee",
            scope = "email profile"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri.ToString().Contains("/realms/master/protocol/openid-connect/token")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(tokenResponse), Encoding.UTF8, "application/json")
            });
    }
}
