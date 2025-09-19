using FrndAppBackend.DTOs;
using FrndAppBackend.Services;
using FrndAppBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrndAppBackend.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Authorize]
    [Tags("Messaging")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messages;

        public MessagesController(IMessageService messages)
        {
            _messages = messages;
        }

        /// Sends a message to a friend.
        /// Summary: Send message
        [HttpPost]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<MessageResponse>> Send([FromBody] SendMessageRequest request, CancellationToken ct)
        {
            var me = User.GetUserId();
            var resp = await _messages.SendAsync(me, request, ct);
            return Ok(resp);
        }

        /// Gets message history between the current user and another user.
        /// Summary: Get conversation
        [HttpGet("with/{otherUserId:guid}")]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        // PUBLIC_INTERFACE
        public async Task<ActionResult<List<MessageResponse>>> GetConversation([FromRoute] Guid otherUserId, [FromQuery] int take = 50, [FromQuery] int skip = 0, CancellationToken ct = default)
        {
            var me = User.GetUserId();
            var resp = await _messages.GetConversationAsync(me, otherUserId, take, skip, ct);
            return Ok(resp);
        }
    }
}
