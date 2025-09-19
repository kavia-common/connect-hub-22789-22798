using FrndAppBackend.DTOs;

namespace FrndAppBackend.Services
{
    public interface IAuthService
    {
        // PUBLIC_INTERFACE
        AuthResponse CreateToken(UserSummary user);
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
