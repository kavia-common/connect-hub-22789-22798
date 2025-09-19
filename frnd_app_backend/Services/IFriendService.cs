using FrndAppBackend.DTOs;

namespace FrndAppBackend.Services
{
    public interface IFriendService
    {
        // PUBLIC_INTERFACE
        Task<FriendRequestResponse> SendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken ct = default);
        Task<FriendRequestResponse> AcceptAsync(Guid userId, Guid requestId, CancellationToken ct = default);
        Task<FriendRequestResponse> RejectAsync(Guid userId, Guid requestId, CancellationToken ct = default);
        Task<List<FriendRequestResponse>> ListAsync(Guid userId, bool pendingOnly, CancellationToken ct = default);
        Task<List<FriendEntry>> GetFriendsAsync(Guid userId, CancellationToken ct = default);
        Task<bool> AreFriendsAsync(Guid a, Guid b, CancellationToken ct = default);
    }
}
