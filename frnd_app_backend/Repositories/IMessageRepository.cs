using FrndAppBackend.Models;

namespace FrndAppBackend.Repositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message, CancellationToken ct = default);
        Task<List<Message>> GetConversationAsync(Guid userA, Guid userB, int take = 50, int skip = 0, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
