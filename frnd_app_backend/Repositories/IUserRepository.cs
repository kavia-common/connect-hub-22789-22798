using FrndAppBackend.Models;

namespace FrndAppBackend.Repositories
{
    /// PUBLIC_INTERFACE
    /// <summary>
    /// Abstraction over user data operations used across services.
    /// Provides retrieval by id/email/username, updates, friendship checks, and save operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Gets a user by id or null if not found.</summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>Gets a user by email or null if not found.</summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>Gets a user by username or null if not found.</summary>
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);

        /// <summary>Adds a new user to the context.</summary>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>Updates an existing user.</summary>
        Task UpdateAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Checks if two users are friends (there exists an accepted friend request between them).
        /// </summary>
        Task<bool> IsFriendAsync(Guid userId, Guid otherUserId, CancellationToken ct = default);

        /// <summary>Returns the list of accepted friends for a user.</summary>
        Task<List<User>> GetFriendsAsync(Guid userId, CancellationToken ct = default);

        /// <summary>Persists changes to the database.</summary>
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
