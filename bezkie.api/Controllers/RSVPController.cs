using bezkie.application.Common.Models;
using bezkie.application.Features.RSVPs.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace bezkie.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "DEFAULT")]
    public class RSVPController : ControllerBase
    {
        private IMediator _mediator;

        public RSVPController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// This allows customer to rsvp a book
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> RSVP([FromBody] CreateRSVPRequest request)
        {
            var result = await _mediator.Send(request);
            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }
    }
}
