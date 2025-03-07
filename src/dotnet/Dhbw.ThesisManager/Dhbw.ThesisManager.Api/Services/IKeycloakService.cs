using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Api.Models.Responses;

namespace Dhbw.ThesisManager.Api.Services
{
    public interface IKeycloakService
    {
        Task<UserListResponse> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<UserIdListResponse> CreateUsersAsync(IEnumerable<UserCreationRequest> users, CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserAsync(long userId, string registrationToken = null, CancellationToken cancellationToken = default);
        Task<UserResponse> UpdateUserAsync(long userId, User user, CancellationToken cancellationToken = default);
        Task<EmptyResponse> DeleteUserAsync(long userId, CancellationToken cancellationToken = default);
        Task<RegistrationTokenResponse> RegenerateRegistrationTokenAsync(long userId, CancellationToken cancellationToken = default);
    }
}
