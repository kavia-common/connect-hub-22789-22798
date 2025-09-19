using FrndAppBackend.DTOs;

namespace FrndAppBackend.Services
{
    public interface IProfileService
    {
        // PUBLIC_INTERFACE
        Task<ProfileResponse> GetMyProfileAsync(Guid userId, CancellationToken ct = default);
        Task<ProfileResponse> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
        Task<ProfileResponse?> GetByIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<UserSummary>> SearchAsync(string query, int take = 20, int skip = 0, CancellationToken ct = default);
    }
}
