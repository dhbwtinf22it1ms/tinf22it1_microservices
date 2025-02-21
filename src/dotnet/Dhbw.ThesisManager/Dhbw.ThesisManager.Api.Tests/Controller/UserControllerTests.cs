using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Models.Responses;
using Dhbw.ThesisManager.Api.Services;
using Moq;
using Xunit;

namespace Dhbw.ThesisManager.Api.Tests.Controller;

public class UserControllerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly UserControllerImpl _controller;
    private readonly RegistrationControllerImpl _registrationController;

    public UserControllerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _controller = new UserControllerImpl(_keycloakServiceMock.Object);
        _registrationController = new RegistrationControllerImpl(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task UsersGETAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new UserListResponse
        {
            Ok = new List<User>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe", Role = UserRole.Student },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith", Role = UserRole.Supervisor }
            }
        };

        _keycloakServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _controller.UsersGETAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Ok.Count);
        Assert.Equal("John", result.Ok.First().FirstName);
        Assert.Equal("Jane", result.Ok.Last().FirstName);
    }

    [Fact]
    public async Task UsersPOSTAsync_ShouldCreateUsers()
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
                    Role = UserRole.Student 
                },
                RegistrationToken = "token123"
            },
            new() 
            { 
                User = new User 
                { 
                    FirstName = "Jane", 
                    LastName = "Smith", 
                    Role = UserRole.Supervisor 
                },
                RegistrationToken = "token456"
            }
        };

        var expectedResponse = new UserIdListResponse
        {
            Ok = new List<long> { 1, 2 }
        };

        _keycloakServiceMock.Setup(x => x.CreateUsersAsync(It.IsAny<IEnumerable<UserCreationRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UsersPOSTAsync(userRequests);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Ok.Count);
        Assert.Equal(1, result.Ok.First());
        Assert.Equal(2, result.Ok.Last());
    }

    [Fact]
    public async Task UsersGET2Async_ShouldReturnUserById()
    {
        // Arrange
        var expectedUser = new UserResponse
        {
            Ok = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Student
            }
        };

        _keycloakServiceMock.Setup(x => x.GetUserAsync(1, "token123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.UsersGET2Async(1, "token123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Ok.Id);
        Assert.Equal("John", result.Ok.FirstName);
    }

    [Fact]
    public async Task UsersPUTAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Student
        };

        var expectedResponse = new UserResponse { Ok = user };

        _keycloakServiceMock.Setup(x => x.UpdateUserAsync(1, user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UsersPUTAsync(1, user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Ok.Id);
        Assert.Equal("John", result.Ok.FirstName);
    }

    [Fact]
    public async Task UsersDELETEAsync_ShouldDeleteUser()
    {
        // Arrange
        var expectedResponse = new EmptyResponse();

        _keycloakServiceMock.Setup(x => x.DeleteUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UsersDELETEAsync(1);

        // Assert
        Assert.NotNull(result);
        _keycloakServiceMock.Verify(x => x.DeleteUserAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TokenAsync_ShouldRegenerateRegistrationToken()
    {
        // Arrange
        var expectedResponse = new RegistrationTokenResponse
        {
            Ok = "new_token_123"
        };

        _keycloakServiceMock.Setup(x => x.RegenerateRegistrationTokenAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _registrationController.TokenAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new_token_123", result.Ok);
    }

    [Fact]
    public async Task UsersGET2Async_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _keycloakServiceMock.Setup(x => x.GetUserAsync(999, "token123", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.UsersGET2Async(999, "token123"));
    }

    [Fact]
    public async Task UsersPUTAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var user = new User
        {
            Id = 999,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Student
        };

        _keycloakServiceMock.Setup(x => x.UpdateUserAsync(999, user, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.UsersPUTAsync(999, user));
    }

    [Fact]
    public async Task UsersDELETEAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _keycloakServiceMock.Setup(x => x.DeleteUserAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.UsersDELETEAsync(999));
    }

    [Fact]
    public async Task TokenAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _keycloakServiceMock.Setup(x => x.RegenerateRegistrationTokenAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _registrationController.TokenAsync(999));
    }
}
