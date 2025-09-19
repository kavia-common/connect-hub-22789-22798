using FrndAppBackend.DTOs;
using FrndAppBackend.Services;
using FrndAppBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrndAppBackend.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    [Tags("Profiles")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profiles;

        public ProfileController(IProfileService profiles)
        {
            _profiles = profiles;
        }

        /// Gets the currently authenticated user's profile.
        /// Summary: Get my profile
        /// Returns the profile of the current user.
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<ProfileResponse>> GetMe(CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _profiles.GetMyProfileAsync(me, ct);
            return Ok(resp);
        }

        /// Updates the current user's profile fields.
        /// Summary: Update my profile
        [HttpPut("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<ProfileResponse>> UpdateMe([FromBody] UpdateProfileRequest request, CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _profiles.UpdateMyProfileAsync(me, request, ct);
            return Ok(resp);
        }

        /// Gets another user's public profile by ID.
        /// Summary: Get profile by id
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<ProfileResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var resp = await _profiles.GetByIdAsync(id, ct);
            if (resp == null) return NotFound();
            return Ok(resp);
        }

        /// Searches users by username, display name, or email.
        /// Summary: Search users
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<UserSummary>), StatusCodes.Status200OK)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<List<UserSummary>>> Search([FromQuery] string q, [FromQuery] int take = 20, [FromQuery] int skip = 0, CancellationToken ct = default)
        {
            var results = await _profiles.SearchAsync(q ?? string.Empty, take, skip, ct);
            return Ok(results);
        }
    }
}
