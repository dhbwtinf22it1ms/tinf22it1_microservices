using System.Net;
using Dhbw.ThesisManager.Api.Configuration;
using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dhbw.ThesisManager.Api.Tests.Services;

[Collection("Integration Tests")]
[Trait("Category", "Integration")]
public class KeycloakServiceIntegrationTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly KeycloakService _keycloakService;
    private readonly List<long> _createdUserIds = new();

    public KeycloakServiceIntegrationTests()
    {
        // Create a real HTTP client with a timeout
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Configure Keycloak options to connect to the Docker instance
        _keycloakOptions = new KeycloakOptions
        {
            // Using localhost:8080 as specified in docker-compose.yml
            AdminUrl = "http://localhost:8080",
            Realm = "thesis-management",
            // These values should match what's in the Keycloak realm configuration
            AdminClientId = "admin-cli",
            AdminClientSecret = "admin-secret"
        };
        
        var options = Options.Create(_keycloakOptions);
        
        // Create the real KeycloakService
        _keycloakService = new KeycloakService(_httpClient, options);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsers()
    {
        // Act
        var result = await _keycloakService.GetUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Ok);
        
        // There should be at least the 3 test users from the realm import
        Assert.True(result.Ok.Count >= 3, "Expected at least 3 users from the imported realm data");
        
        // Verify that the test users from the realm import are present
        var testUser1 = result.Ok.FirstOrDefault(u => u.Email == "testuser1@dhbw-mannheim.de");
        var testUser2 = result.Ok.FirstOrDefault(u => u.Email == "testuser2@dhbw-mannheim.de");
        var testUser3 = result.Ok.FirstOrDefault(u => u.Email == "testuser3@dhbw-mannheim.de");
        
        Assert.NotNull(testUser1);
        Assert.NotNull(testUser2);
        Assert.NotNull(testUser3);
        
        // Verify roles based on groups
        Assert.Equal(UserRole.administrator, testUser1.Role);
        Assert.Equal(UserRole.student, testUser2.Role);
        Assert.Equal(UserRole.supervisor, testUser3.Role); // Teacher group maps to Supervisor role
    }

    [Fact]
    public async Task CreateAndGetUserAsync_ShouldCreateAndReturnUser()
    {
        // Arrange
        var testEmail = $"test.user.{Guid.NewGuid()}@dhbw-mannheim.de";
        var userRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = testEmail,
                Role = UserRole.student
            },
            RegistrationToken = "initialPassword123"
        };

        try
        {
            // Act - Create user
            var createResult = await _keycloakService.CreateUsersAsync(new[] { userRequest });

            // Assert - Create
            Assert.NotNull(createResult);
            Assert.NotNull(createResult.Ok);
            Assert.Single(createResult.Ok);
            
            var userId = createResult.Ok[0];
            _createdUserIds.Add(userId); // Track for cleanup
            
            // Act - Get user
            var getResult = await _keycloakService.GetUserAsync(userId);

            // Assert - Get
            Assert.NotNull(getResult);
            Assert.NotNull(getResult.Ok);
            Assert.Equal(userId, getResult.Ok.Id);
            Assert.Equal("Test", getResult.Ok.FirstName);
            Assert.Equal("User", getResult.Ok.LastName);
            Assert.Equal(testEmail, getResult.Ok.Email);
            Assert.Equal(UserRole.student, getResult.Ok.Role);
            Assert.Null(getResult.Ok.RegistrationStatus.Pending); // Verified users have Pending = null
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser()
    {
        // Arrange - Create a user first
        var testEmail = $"test.user.{Guid.NewGuid()}@dhbw-mannheim.de";
        var userRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = testEmail,
                Role = UserRole.student
            }
        };

        try
        {
            var createResult = await _keycloakService.CreateUsersAsync(new[] { userRequest });
            var userId = createResult.Ok[0];
            _createdUserIds.Add(userId); // Track for cleanup

            // Arrange - Update data
            var updatedUser = new User
            {
                Id = userId,
                FirstName = "Updated",
                LastName = "Name",
                Email = testEmail, // Keep the same email
                Role = UserRole.supervisor // Change role
            };

            // Act
            var updateResult = await _keycloakService.UpdateUserAsync(userId, updatedUser);

            // Assert
            Assert.NotNull(updateResult);
            Assert.NotNull(updateResult.Ok);
            Assert.Equal(userId, updateResult.Ok.Id);
            Assert.Equal("Updated", updateResult.Ok.FirstName);
            Assert.Equal("Name", updateResult.Ok.LastName);
            Assert.Equal(testEmail, updateResult.Ok.Email);
            Assert.Equal(UserRole.supervisor, updateResult.Ok.Role);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser()
    {
        // Arrange - Create a user first
        var testEmail = $"test.user.{Guid.NewGuid()}@dhbw-mannheim.de";
        var userRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = testEmail,
                Role = UserRole.student
            }
        };

        try
        {
            var createResult = await _keycloakService.CreateUsersAsync(new[] { userRequest });
            var userId = createResult.Ok[0];
            // Don't track for cleanup since we're deleting in the test

            // Act
            var deleteResult = await _keycloakService.DeleteUserAsync(userId);

            // Assert
            Assert.NotNull(deleteResult);

            // Verify the user is actually deleted by trying to get it
            await Assert.ThrowsAsync<Exception>(() => _keycloakService.GetUserAsync(userId));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task RegenerateRegistrationTokenAsync_ShouldRegenerateToken()
    {
        // Arrange - Create a user first
        var testEmail = $"test.user.{Guid.NewGuid()}@dhbw-mannheim.de";
        var userRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = testEmail,
                Role = UserRole.student
            }
        };

        try
        {
            var createResult = await _keycloakService.CreateUsersAsync(new[] { userRequest });
            var userId = createResult.Ok[0];
            _createdUserIds.Add(userId); // Track for cleanup

            // Act
            var tokenResult = await _keycloakService.RegenerateRegistrationTokenAsync(userId);

            // Assert
            Assert.NotNull(tokenResult);
            Assert.NotNull(tokenResult.Ok);
            Assert.NotEmpty(tokenResult.Ok);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task GetAllUsersAndFilterByRole_ShouldReturnFilteredUsers()
    {
        // Arrange - Create users with different roles
        var testEmailStudent = $"test.student.{Guid.NewGuid()}@dhbw-mannheim.de";
        var testEmailSupervisor = $"test.supervisor.{Guid.NewGuid()}@dhbw-mannheim.de";
        
        var studentRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "Student",
                Email = testEmailStudent,
                Role = UserRole.student
            }
        };
        
        var supervisorRequest = new UserCreationRequest
        {
            User = new User
            {
                FirstName = "Test",
                LastName = "Supervisor",
                Email = testEmailSupervisor,
                Role = UserRole.supervisor
            }
        };

        try
        {
            var createStudentResult = await _keycloakService.CreateUsersAsync(new[] { studentRequest });
            var createSupervisorResult = await _keycloakService.CreateUsersAsync(new[] { supervisorRequest });
            
            var studentId = createStudentResult.Ok[0];
            var supervisorId = createSupervisorResult.Ok[0];
            
            _createdUserIds.Add(studentId);
            _createdUserIds.Add(supervisorId);

            // Act
            var allUsers = await _keycloakService.GetUsersAsync();
            
            // Filter users by role
            var students = allUsers.Ok.Where(u => u.Role == UserRole.student).ToList();
            var supervisors = allUsers.Ok.Where(u => u.Role == UserRole.supervisor).ToList();

            // Assert
            Assert.NotNull(allUsers);
            Assert.NotNull(allUsers.Ok);
            
            // Check that our test users are in the correct role lists
            Assert.Contains(students, u => u.Id == studentId);
            Assert.Contains(supervisors, u => u.Id == supervisorId);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Clean up any users created during tests
        foreach (var userId in _createdUserIds)
        {
            try
            {
                _keycloakService.DeleteUserAsync(userId).Wait();
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
        
        _httpClient.Dispose();
    }
}
