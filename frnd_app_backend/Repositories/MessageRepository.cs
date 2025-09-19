using FrndAppBackend.Data;
using FrndAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FrndAppBackend.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _db;
        public MessageRepository(AppDbContext db) { _db = db; }

        public async Task AddAsync(Message message, CancellationToken ct = default) =>
            await _db.Messages.AddAsync(message, ct);

        public Task<List<Message>> GetConversationAsync(Guid userA, Guid userB, int take = 50, int skip = 0, CancellationToken ct = default)
        {
            return _db.Messages
                .Where(m => (m.FromUserId == userA && m.ToUserId == userB) || (m.FromUserId == userB && m.ToUserId == userA))
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
