using FrndAppBackend.DTOs;
using FrndAppBackend.Models;
using FrndAppBackend.Repositories;

namespace FrndAppBackend.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messages;
        private readonly IFriendService _friends;

        public MessageService(IMessageRepository messages, IFriendService friends)
        {
            _messages = messages;
            _friends = friends;
        }

        public async Task<MessageResponse> SendAsync(Guid fromUserId, SendMessageRequest request, CancellationToken ct = default)
        {
            var areFriends = await _friends.AreFriendsAsync(fromUserId, request.ToUserId, ct);
            if (!areFriends) throw new InvalidOperationException("You can only message your friends.");

            var msg = new Message
            {
                FromUserId = fromUserId,
                ToUserId = request.ToUserId,
                Content = request.Content.Trim()
            };
            await _messages.AddAsync(msg, ct);
            await _messages.SaveChangesAsync(ct);

            return Map(msg);
        }

        public async Task<List<MessageResponse>> GetConversationAsync(Guid me, Guid other, int take = 50, int skip = 0, CancellationToken ct = default)
        {
            var areFriends = await _friends.AreFriendsAsync(me, other, ct);
            if (!areFriends) throw new InvalidOperationException("You can only view conversations with friends.");

            var list = await _messages.GetConversationAsync(me, other, take, skip, ct);
            return list.OrderBy(m => m.SentAt).Select(Map).ToList();
        }

        private static MessageResponse Map(Message m) => new()
        {
            Id = m.Id,
            FromUserId = m.FromUserId,
            ToUserId = m.ToUserId,
            Content = m.Content,
            SentAt = m.SentAt
        };
    }
}
