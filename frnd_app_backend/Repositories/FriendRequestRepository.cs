using FrndAppBackend.Data;
using FrndAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FrndAppBackend.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly AppDbContext _db;
        public FriendRequestRepository(AppDbContext db) { _db = db; }

        public Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == id, ct);

        public async Task AddAsync(FriendRequest request, CancellationToken ct = default) =>
            await _db.FriendRequests.AddAsync(request, ct);

        public Task UpdateAsync(FriendRequest request, CancellationToken ct = default)
        {
            _db.FriendRequests.Update(request);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsPendingAsync(Guid fromUserId, Guid toUserId, CancellationToken ct = default) =>
            _db.FriendRequests.AnyAsync(fr =>
                fr.FromUserId == fromUserId && fr.ToUserId == toUserId && fr.Status == FriendRequestStatus.Pending, ct);

        public Task<List<FriendRequest>> GetPendingForUserAsync(Guid userId, CancellationToken ct = default) =>
            _db.FriendRequests.Where(fr => fr.ToUserId == userId && fr.Status == FriendRequestStatus.Pending)
                .OrderByDescending(fr => fr.CreatedAt).ToListAsync(ct);

        public Task<List<FriendRequest>> GetAllForUserAsync(Guid userId, CancellationToken ct = default) =>
            _db.FriendRequests.Where(fr => fr.FromUserId == userId || fr.ToUserId == userId)
                .OrderByDescending(fr => fr.CreatedAt).ToListAsync(ct);

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
