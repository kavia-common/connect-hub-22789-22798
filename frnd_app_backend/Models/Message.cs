using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrndAppBackend.Models
{
    /// Represents a message sent from one user to another.
    public class Message
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid FromUserId { get; set; }

        [Required]
        public Guid ToUserId { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(FromUserId))]
        public User FromUser { get; set; } = null!;

        [ForeignKey(nameof(ToUserId))]
        public User ToUser { get; set; } = null!;
    }
}
