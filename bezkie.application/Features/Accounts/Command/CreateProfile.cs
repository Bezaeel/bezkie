using bezkie.application.Common.Models;
using bezkie.core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace bezkie.application.Features.Profile.Command;

public class CreateProfileRequest : IRequest<BaseResponse>
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    public User ToEntity()
    {
        var it = new User 
        { 
            Email = Email, 
            Name = Name,
            UserName = Email
        };
        return it;
    }
}

public class CreateProfileValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileValidator()
    {
        RuleFor(s => s.Email).NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("A valid email is required");
        RuleFor(x => x.Password).NotEmpty().Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-,.]).{8,}$");
    }
}

public class CreateProfileHandler : IRequestHandler<CreateProfileRequest, BaseResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CreateProfileHandler> _logger;
    public CreateProfileHandler(UserManager<User> userManager, ILogger<CreateProfileHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    public async Task<BaseResponse> Handle(CreateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                return new BaseResponse(false, "user already exists");
            }
            var result = await _userManager.CreateAsync(request.ToEntity(), request.Password);
            if (result.Errors.Any())
            {
                throw new Exception("unable to create user");
            }

            return new BaseResponse(true, "Success");

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return new BaseResponse(false, "Operation failed");
        }
    }
}
