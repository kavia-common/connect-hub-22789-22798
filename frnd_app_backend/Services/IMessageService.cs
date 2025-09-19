using FrndAppBackend.DTOs;

namespace FrndAppBackend.Services
{
    public interface IMessageService
    {
        // PUBLIC_INTERFACE
        Task<MessageResponse> SendAsync(Guid fromUserId, SendMessageRequest request, CancellationToken ct = default);
        Task<List<MessageResponse>> GetConversationAsync(Guid me, Guid other, int take = 50, int skip = 0, CancellationToken ct = default);
    }
}
