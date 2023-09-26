using bezkie.application.Common.Models;
using bezkie.core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace bezkie.application.Features.Profile.Command;

public class LoginRequest : IRequest<BaseResponse<LoginResponse>>
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(s => s.Email).NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("A valid email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}

public class LoginHandler : IRequestHandler<LoginRequest, BaseResponse<LoginResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginHandler> _logger;
    public LoginHandler(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<BaseResponse<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email.ToUpper());

            if (user == null)
                return new BaseResponse<LoginResponse>(false, $"Invalid Credentials");


            var passwordChecker = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!passwordChecker.Succeeded)
                return new BaseResponse<LoginResponse>(false, "Invalid Credentials");

            List<Claim> roleClaims = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var key = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var EXPIRY_DURATION_MINUTES = double.Parse(_configuration["JwtSettings:ExpiryMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, roleClaims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new BaseResponse<LoginResponse>
            {

                Status = true,
                Message = "Success",
                Data = new LoginResponse
                {
                    Token = token,
                }
            };

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return new BaseResponse<LoginResponse>(false, "Operation failed");
        }
    }
}
