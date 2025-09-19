using FrndAppBackend.Models;

namespace FrndAppBackend.Repositories
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(FriendRequest request, CancellationToken ct = default);
        Task UpdateAsync(FriendRequest request, CancellationToken ct = default);
        Task<bool> ExistsPendingAsync(Guid fromUserId, Guid toUserId, CancellationToken ct = default);
        Task<List<FriendRequest>> GetPendingForUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<FriendRequest>> GetAllForUserAsync(Guid userId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
