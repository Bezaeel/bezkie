using bezkie.application.Common.Models;
using bezkie.application.Features.Catalogue.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static bezkie.application.Features.Catalogue.Queries.SearchCatalogue;

namespace bezkie.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private IMediator _mediator;

        public CatalogueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// This allows a librarian to add book to catalogue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddBook([FromBody] AddBookToCatalogueRequest request)
        {
            var result = await _mediator.Send(request);
            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// search catalogue
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchCatalogueQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }
    }
}
