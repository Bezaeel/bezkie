using bezkie.application.Common.Models;
using bezkie.core.Enums;
using bezkie.infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace bezkie.application.Features.Catalogue.Queries
{
    public class SearchCatalogue
    {
        public class QueryResponse
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
        public class SearchCatalogueQuery : IRequest<BaseResponse<QueryResponse>>
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class SearchCatalogueQueryValidator : AbstractValidator<SearchCatalogueQuery>
        {
            public SearchCatalogueQueryValidator()
            {
            }
        }

        public class SearchCatalogueQueryHandler : IRequestHandler<SearchCatalogueQuery, BaseResponse<QueryResponse>>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly ILogger<SearchCatalogueQueryHandler> _logger;

            public SearchCatalogueQueryHandler(IApplicationDbContext dbContext, ILogger<SearchCatalogueQueryHandler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }
            public async Task<BaseResponse<QueryResponse>> Handle(SearchCatalogueQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var status = true;
                    var message = "Success";
                    var queryResponse = new QueryResponse { Name = query.Name };
                    // check if book is available
                    var book = _dbContext.Books
                                        .Include(x => x.RSVPs)
                                        .FirstOrDefault(x => x.Name == query.Name);
                    // if no rsvp, book is available
                    if (book == null)
                    {
                        status = false;
                        queryResponse.Message = "not found";
                    }
                    else
                    {
                        if (book.RSVPs == null)
                        {
                            queryResponse.Message = "book is available";
                        }
                        else
                        {
                            var rsvp = book.RSVPs.OrderByDescending(x => x.StatusAt).FirstOrDefault();
                            queryResponse.Message = Enum.GetName(typeof(RSVPStatus), rsvp.Status);
                        }

                    }
                    return new BaseResponse<QueryResponse>
                    {
                        Status = status,
                        Message = queryResponse.Message,
                        Data = queryResponse
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return new BaseResponse<QueryResponse>(false, "Operation failed");
                }
            }
        }
    }
}
