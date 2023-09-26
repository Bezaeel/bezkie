using bezkie.application.Common.Models;
using bezkie.core.Entities;
using bezkie.core.Enums;
using bezkie.infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace bezkie.application.Features.RSVPs.Commands;

public class CreateRSVPRequest : IRequest<BaseResponse>
{
    [JsonIgnore]
    public long CustomerId { get; set; }

    [JsonPropertyName("book_id")]
    public long BookId { get; set; }

    [JsonIgnore]
    public RSVPStatus Status { get; set; } = RSVPStatus.RESERVED;

    public RSVP ToEntity()
    {
        var it = new RSVP
        {
            CustomerId = CustomerId,
            BookId = BookId,
            Status = Status
        };

        return it;
    }
}

public class CreateRSVPValidator : AbstractValidator<CreateRSVPRequest>
{
    public CreateRSVPValidator()
    {
    }
}

public class CreateRSVPHandler : IRequestHandler<CreateRSVPRequest, BaseResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateRSVPHandler> _logger;

    public CreateRSVPHandler(IApplicationDbContext dbContext, ILogger<CreateRSVPHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<BaseResponse> Handle(CreateRSVPRequest request, CancellationToken cancellationToken)
    {
		try
		{
            var status = false;
            var message = "Success";
            // check if book is available
            var rsvp = _dbContext.RSVPs.FirstOrDefault(x => x.BookId == request.BookId && x.UpdatedAt == null);
            // if no rsvp, book is available
            if (rsvp == null)
            {
                status = true;
                _dbContext.RSVPs.Add(request.ToEntity());
                await _dbContext.SaveChangesAsync();
                message = "reserved";
            }
            else
            {
                if (rsvp.Status == RSVPStatus.RESERVED)
                {
                    if (DateTime.UtcNow >= rsvp.StatusAt.AddHours(24))
                    {
                        status = true;
                        rsvp.Status = RSVPStatus.EXPIRED;
                        rsvp.UpdatedAt = DateTime.UtcNow;
                        _dbContext.RSVPs.Add(request.ToEntity());
                        await _dbContext.SaveChangesAsync();
                        message = "reserved";
                    }
                    else
                    {
                        _logger.LogInformation("book has current reservation with reader {customer} since {since}", rsvp.CustomerId, rsvp.StatusAt);
                        message = $"book has current reservation since {rsvp.StatusAt}";
                    }
                }

                if (rsvp.Status == RSVPStatus.BORROWED)
                {
                    // log rather than send info to customer
                    _logger.LogInformation("book currently with reader {customer} since {since}", rsvp.CustomerId, rsvp.StatusAt);
                    message = $"book currently with reader";
                }

                if (rsvp.Status == RSVPStatus.OTHER)
                {
                    message = $"book is currently not available, kindly check with librarian";
                }
            }
            return new BaseResponse(status, message);
        }
		catch (Exception ex)
		{
            _logger.LogCritical(ex.Message);
            return new BaseResponse(false, "Operation failed");
		}
    }
}
