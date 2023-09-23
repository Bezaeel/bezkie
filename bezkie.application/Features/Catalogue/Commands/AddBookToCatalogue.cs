using bezkie.application.Common.Models;
using bezkie.core.Entities;
using bezkie.infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace bezkie.application.Features.Catalogue.Commands;

public class AddBookToCatalogueRequest : IRequest<BaseResponse>
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    public Book ToEntity()
    {
        var it = new Book
        {
            Name = Name,
        };

        return it;
    }
}

public class AddBookToCatalogueValidator : AbstractValidator<AddBookToCatalogueRequest>
{
    public AddBookToCatalogueValidator()
    {
    }
}

public class CreateRSVPHandler : IRequestHandler<AddBookToCatalogueRequest, BaseResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateRSVPHandler> _logger;
    public CreateRSVPHandler(IApplicationDbContext dbContext, ILogger<CreateRSVPHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<BaseResponse> Handle(AddBookToCatalogueRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var book = _dbContext.Books.FirstOrDefault(x => x.Name == request.Name);
            if (book != null)
            {
                return new BaseResponse(false, "book alreay in catalogue");
            }

            _dbContext.Books.Add(request.ToEntity());
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponse(true, "Success");

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return new BaseResponse(false, "Operation failed");
        }
    }
}