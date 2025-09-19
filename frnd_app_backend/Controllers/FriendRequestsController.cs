using FrndAppBackend.DTOs;
using FrndAppBackend.Services;
using FrndAppBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrndAppBackend.Controllers
{
    [ApiController]
    [Route("api/friends")]
    [Authorize]
    [Tags("Friend Requests")]
    public class FriendRequestsController : ControllerBase
    {
        private readonly IFriendService _friends;

        public FriendRequestsController(IFriendService friends)
        {
            _friends = friends;
        }

        /// Sends a friend request to another user.
        /// Summary: Send friend request
        [HttpPost("requests")]
        [ProducesResponseType(typeof(FriendRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<FriendRequestResponse>> Send([FromBody] FriendRequestCreate request, CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _friends.SendRequestAsync(me, request.ToUserId, ct);
            return Ok(resp);
        }

        /// Lists friend requests for the current user.
        /// Summary: List friend requests
        [HttpGet("requests")]
        [ProducesResponseType(typeof(List<FriendRequestResponse>), StatusCodes.Status200OK)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<List<FriendRequestResponse>>> List([FromQuery] bool pendingOnly = false, CancellationToken ct = default)
        {
            var me = User.GetUserId();
            var list = await _friends.ListAsync(me, pendingOnly, ct);
            return Ok(list);
        }

        /// Accepts a friend request addressed to the current user.
        /// Summary: Accept friend request
        [HttpPost("requests/{id:guid}/accept")]
        [ProducesResponseType(typeof(FriendRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<FriendRequestResponse>> Accept([FromRoute] Guid id, CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _friends.AcceptAsync(me, id, ct);
            return Ok(resp);
        }

        /// Rejects a friend request addressed to the current user.
        /// Summary: Reject friend request
        [HttpPost("requests/{id:guid}/reject")]
        [ProducesResponseType(typeof(FriendRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<FriendRequestResponse>> Reject([FromRoute] Guid id, CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _friends.RejectAsync(me, id, ct);
            return Ok(resp);
        }

        /// Gets the list of accepted friends for the current user.
        /// Summary: List friends
        [HttpGet]
        [ProducesResponseType(typeof(List<FriendEntry>), StatusCodes.Status200OK)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<List<FriendEntry>>> GetFriends(CancellationToken ct)
        {
            var me = User.GetUserId();
            var friends = await _friends.GetFriendsAsync(me, ct);
            return Ok(friends);
        }
    }
}
