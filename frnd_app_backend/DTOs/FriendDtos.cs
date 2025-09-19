namespace FrndAppBackend.DTOs
{
    /// Create a friend request to the specified user.
    public class FriendRequestCreate
    {
        public Guid ToUserId { get; set; }
    }

    /// Friend request representation in API responses.
    public class FriendRequestResponse
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    /// Friend entry for accepted friends list.
    public class FriendEntry
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}
