using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Models.Responses;
using Dhbw.ThesisManager.Api.Services;

namespace Dhbw.ThesisManager.Api;

public class UserControllerImpl : IUserController
{
    private readonly IKeycloakService _keycloakService;

    public UserControllerImpl(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<UserListResponse> UsersGETAsync(CancellationToken cancellationToken = default)
    {
        return await _keycloakService.GetUsersAsync(cancellationToken);
    }

    public async Task<UserIdListResponse> UsersPOSTAsync(IEnumerable<UserCreationRequest> body, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.CreateUsersAsync(body, cancellationToken);
    }

    public async Task<UserResponse> UsersGET2Async(long userId, string registration_token, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.GetUserAsync(userId, registration_token, cancellationToken);
    }

    public async Task<UserResponse> UsersPUTAsync(long userId, User body, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.UpdateUserAsync(userId, body, cancellationToken);
    }

    public async Task<EmptyResponse> UsersDELETEAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.DeleteUserAsync(userId, cancellationToken);
    }
}

public class RegistrationControllerImpl : IRegistrationController
{
    private readonly IKeycloakService _keycloakService;

    public RegistrationControllerImpl(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<RegistrationTokenResponse> TokenAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.RegenerateRegistrationTokenAsync(userId, cancellationToken);
    }
}
