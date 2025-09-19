using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrndAppBackend.DTOs;
using FrndAppBackend.Models;
using FrndAppBackend.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FrndAppBackend.Services
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "frnd-app";
        public string Audience { get; set; } = "frnd-app-clients";
        public string SigningKey { get; set; } = "CHANGE_ME_SUPER_SECRET_MIN32CHARS";
        public int ExpiryMinutes { get; set; } = 120;
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly JwtSettings _jwt;

        public AuthService(IUserRepository users, IOptions<JwtSettings> jwt)
        {
            _users = users;
            _jwt = jwt.Value;
        }

        // PUBLIC_INTERFACE
        public AuthResponse CreateToken(UserSummary user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwt.SigningKey);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Fallback NameIdentifier for HttpContextExtensions
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = handler.CreateToken(descriptor);
            return new AuthResponse
            {
                Token = handler.WriteToken(token),
                User = user
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var existingEmail = await _users.GetByEmailAsync(request.Email, ct);
            if (existingEmail != null) throw new InvalidOperationException("Email already registered.");

            var existingUserName = await _users.GetByUserNameAsync(request.UserName, ct);
            if (existingUserName != null) throw new InvalidOperationException("Username already in use.");

            var user = new User
            {
                Email = request.Email.Trim().ToLowerInvariant(),
                UserName = request.UserName.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.UserName.Trim() : request.DisplayName?.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            await _users.AddAsync(user, ct);
            await _users.SaveChangesAsync(ct);

            var summary = new UserSummary
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName
            };
            return CreateToken(summary);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            User? user = null;
            if (request.EmailOrUserName.Contains('@'))
                user = await _users.GetByEmailAsync(request.EmailOrUserName.Trim().ToLowerInvariant(), ct);
            else
                user = await _users.GetByUserNameAsync(request.EmailOrUserName.Trim(), ct);

            if (user == null) throw new UnauthorizedAccessException("Invalid credentials.");

            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!valid) throw new UnauthorizedAccessException("Invalid credentials.");

            var summary = new UserSummary
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName
            };
            return CreateToken(summary);
        }
    }
}
