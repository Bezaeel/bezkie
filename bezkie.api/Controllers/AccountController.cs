using bezkie.application.Common.Models;
using bezkie.application.Features.Profile.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace bezkie.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// This allow the user to register a new account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody]CreateProfileRequest request)
        {
            var result = await _mediator.Send(request);
            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }
    }
}
