using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrndAppBackend.Models
{
    /// Represents a friend request between two users.
    public class FriendRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid FromUserId { get; set; }

        [Required]
        public Guid ToUserId { get; set; }

        [Required]
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }

        [ForeignKey(nameof(FromUserId))]
        public User FromUser { get; set; } = null!;

        [ForeignKey(nameof(ToUserId))]
        public User ToUser { get; set; } = null!;
    }

    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
