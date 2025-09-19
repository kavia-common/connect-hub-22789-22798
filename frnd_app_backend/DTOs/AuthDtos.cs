using System.ComponentModel.DataAnnotations;

namespace FrndAppBackend.DTOs
{
    /// Request payload for user registration.
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DisplayName { get; set; }
    }

    /// Request payload for user login by email or username.
    public class LoginRequest
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// Response returned after authentication, with the JWT and basic user info.
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserSummary User { get; set; } = new();
    }

    /// Lightweight representation of a user for auth/profile responses.
    public class UserSummary
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}
