using FrndAppBackend.DTOs;
using FrndAppBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FrndAppBackend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _users;
        private readonly FrndAppBackend.Data.AppDbContext _db;

        public ProfileService(IUserRepository users, FrndAppBackend.Data.AppDbContext db)
        {
            _users = users;
            _db = db;
        }

        public async Task<ProfileResponse> GetMyProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var u = await _users.GetByIdAsync(userId, ct) ?? throw new KeyNotFoundException("User not found.");
            return Map(u);
        }

        public async Task<ProfileResponse> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
        {
            var u = await _users.GetByIdAsync(userId, ct) ?? throw new KeyNotFoundException("User not found.");
            u.DisplayName = request.DisplayName ?? u.DisplayName;
            u.Bio = request.Bio ?? u.Bio;
            await _users.UpdateAsync(u, ct);
            await _users.SaveChangesAsync(ct);
            return Map(u);
        }

        public async Task<ProfileResponse?> GetByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var u = await _users.GetByIdAsync(userId, ct);
            return u == null ? null : Map(u);
        }

        public async Task<List<UserSummary>> SearchAsync(string query, int take = 20, int skip = 0, CancellationToken ct = default)
        {
            query = query.Trim().ToLowerInvariant();
            var results = await _db.Users
                .Where(u => u.Email.ToLower().Contains(query) || u.UserName.ToLower().Contains(query) || (u.DisplayName ?? "").ToLower().Contains(query))
                .OrderBy(u => u.UserName)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

            return results.Select(u => new UserSummary
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                DisplayName = u.DisplayName
            }).ToList();
        }

        private static ProfileResponse Map(FrndAppBackend.Models.User u) => new()
        {
            Id = u.Id,
            Email = u.Email,
            UserName = u.UserName,
            DisplayName = u.DisplayName,
            Bio = u.Bio,
            CreatedAt = u.CreatedAt
        };
    }
}
