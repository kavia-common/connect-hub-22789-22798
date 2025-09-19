using FrndAppBackend.Data;
using FrndAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FrndAppBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
        }

        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<bool> IsFriendAsync(Guid userId, Guid otherUserId, CancellationToken ct = default)
        {
            return await _db.FriendRequests.AnyAsync(fr =>
                ((fr.FromUserId == userId && fr.ToUserId == otherUserId) ||
                 (fr.FromUserId == otherUserId && fr.ToUserId == userId))
                && fr.Status == FriendRequestStatus.Accepted, ct);
        }

        public async Task<List<User>> GetFriendsAsync(Guid userId, CancellationToken ct = default)
        {
            var accepted = await _db.FriendRequests
                .Where(fr => (fr.FromUserId == userId || fr.ToUserId == userId) && fr.Status == FriendRequestStatus.Accepted)
                .ToListAsync(ct);

            var friendIds = accepted.Select(fr => fr.FromUserId == userId ? fr.ToUserId : fr.FromUserId).Distinct().ToList();
            return await _db.Users.Where(u => friendIds.Contains(u.Id)).ToListAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
