using FrndAppBackend.DTOs;
using FrndAppBackend.Models;
using FrndAppBackend.Repositories;

namespace FrndAppBackend.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRequestRepository _requests;
        private readonly IUserRepository _users;

        public FriendService(IFriendRequestRepository requests, IUserRepository users)
        {
            _requests = requests;
            _users = users;
        }

        public async Task<FriendRequestResponse> SendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken ct = default)
        {
            if (fromUserId == toUserId) throw new InvalidOperationException("Cannot send a request to yourself.");

            var to = await _users.GetByIdAsync(toUserId, ct) ?? throw new KeyNotFoundException("Recipient user not found.");
            var alreadyFriends = await _users.IsFriendAsync(fromUserId, toUserId, ct);
            if (alreadyFriends) throw new InvalidOperationException("You are already friends.");
            var existingPending = await _requests.ExistsPendingAsync(fromUserId, toUserId, ct);
            if (existingPending) throw new InvalidOperationException("A pending request already exists.");

            var request = new FriendRequest
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Status = FriendRequestStatus.Pending
            };
            await _requests.AddAsync(request, ct);
            await _requests.SaveChangesAsync(ct);

            return Map(request);
        }

        public async Task<FriendRequestResponse> AcceptAsync(Guid userId, Guid requestId, CancellationToken ct = default)
        {
            var req = await _requests.GetByIdAsync(requestId, ct) ?? throw new KeyNotFoundException("Request not found.");
            if (req.ToUserId != userId) throw new UnauthorizedAccessException("Cannot accept a request not addressed to you.");
            if (req.Status != FriendRequestStatus.Pending) throw new InvalidOperationException("Request has already been handled.");

            req.Status = FriendRequestStatus.Accepted;
            req.RespondedAt = DateTime.UtcNow;
            await _requests.UpdateAsync(req, ct);
            await _requests.SaveChangesAsync(ct);
            return Map(req);
        }

        public async Task<FriendRequestResponse> RejectAsync(Guid userId, Guid requestId, CancellationToken ct = default)
        {
            var req = await _requests.GetByIdAsync(requestId, ct) ?? throw new KeyNotFoundException("Request not found.");
            if (req.ToUserId != userId) throw new UnauthorizedAccessException("Cannot reject a request not addressed to you.");
            if (req.Status != FriendRequestStatus.Pending) throw new InvalidOperationException("Request has already been handled.");

            req.Status = FriendRequestStatus.Rejected;
            req.RespondedAt = DateTime.UtcNow;
            await _requests.UpdateAsync(req, ct);
            await _requests.SaveChangesAsync(ct);
            return Map(req);
        }

        public async Task<List<FriendRequestResponse>> ListAsync(Guid userId, bool pendingOnly, CancellationToken ct = default)
        {
            var list = pendingOnly
                ? await _requests.GetPendingForUserAsync(userId, ct)
                : await _requests.GetAllForUserAsync(userId, ct);

            return list.Select(Map).ToList();
        }

        public async Task<List<FriendEntry>> GetFriendsAsync(Guid userId, CancellationToken ct = default)
        {
            var friends = await _users.GetFriendsAsync(userId, ct);
            return friends.Select(f => new FriendEntry
            {
                UserId = f.Id,
                UserName = f.UserName,
                DisplayName = f.DisplayName
            }).ToList();
        }

        public Task<bool> AreFriendsAsync(Guid a, Guid b, CancellationToken ct = default) => _users.IsFriendAsync(a, b, ct);

        private static FriendRequestResponse Map(FriendRequest fr) => new()
        {
            Id = fr.Id,
            FromUserId = fr.FromUserId,
            ToUserId = fr.ToUserId,
            Status = fr.Status.ToString(),
            CreatedAt = fr.CreatedAt,
            RespondedAt = fr.RespondedAt
        };
    }
}
