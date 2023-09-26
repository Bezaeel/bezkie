using bezkie.api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text;

namespace bezkie.api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var defaultTokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = true,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"])),
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication()
        .AddJwtBearer("DEFAULT", options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = defaultTokenValidationParameters;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("DEFAULT", new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("DEFAULT")
                .Build());
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });

        services.AddAuthorization();

        services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddServer(new OpenApiServer
            {
                Url = builder.Configuration["Swagger:Server"],
                Description = builder.Environment.EnvironmentName,
            });

            c.SwaggerDoc("API", new OpenApiInfo { Title = $"{builder.Environment.ApplicationName}", Version = "v1" });
            var location = Assembly.GetEntryAssembly()!.Location;
            c.IncludeXmlComments(Path.ChangeExtension(location, "xml"));
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            c.CustomSchemaIds(x => x.FullName);
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
            });
        });
        services.AddCors(options =>
        {
            options.AddPolicy("corsPolicy", corsOption =>
            {
                corsOption.AllowAnyMethod();
                corsOption.AllowAnyHeader();
                corsOption.AllowCredentials();
                corsOption.SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrWhiteSpace(origin)) return false;
                    if (origin.ToLower().StartsWith("http://localhost") && builder.Environment.IsDevelopment()) return true;
                    return false;
                }); // allow any origin
            });
        });

        return services;
    }

}
