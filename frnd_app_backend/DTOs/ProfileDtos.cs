using System.ComponentModel.DataAnnotations;

namespace FrndAppBackend.DTOs
{
    /// Request to update current user's profile.
    public class UpdateProfileRequest
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(256)]
        public string? Bio { get; set; }
    }

    /// Profile response view.
    public class ProfileResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
