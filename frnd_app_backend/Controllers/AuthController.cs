using FrndAppBackend.DTOs;
using FrndAppBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrndAppBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        /// Registers a new user and returns a JWT token.
        /// Summary: Register user
        /// Description: Creates a new account with email, username and password. Returns JWT for subsequent requests.
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var resp = await _auth.RegisterAsync(request, ct);
            return Ok(resp);
        }

        /// Logs in an existing user and returns a JWT token.
        /// Summary: Login user
        /// Description: Login with email or username and password. Returns JWT.
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var resp = await _auth.LoginAsync(request, ct);
            return Ok(resp);
        }
    }
}
