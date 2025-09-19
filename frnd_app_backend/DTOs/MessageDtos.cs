using System.ComponentModel.DataAnnotations;

namespace FrndAppBackend.DTOs
{
    /// Request to send a message to a friend.
    public class SendMessageRequest
    {
        [Required]
        public Guid ToUserId { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }

    /// Message representation in API responses.
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
