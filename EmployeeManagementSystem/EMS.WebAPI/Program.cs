using AutoMapper;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using EMS.Infra.Data.Repository;
using EMS.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add AutoMapper (manual registration - works without deprecated packages)
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfiles>();
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EMS API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Handle JWT authentication errors with user-friendly messages
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Skip the default response
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = "You need to log in to access this resource.";
            var suggestion = "Please provide a valid authentication token.";

            if (!string.IsNullOrEmpty(context.ErrorDescription))
            {
                if (context.ErrorDescription.Contains("expired", StringComparison.OrdinalIgnoreCase))
                {
                    message = "Your session has expired.";
                    suggestion = "Please log in again to continue.";
                }
                else if (context.ErrorDescription.Contains("signature", StringComparison.OrdinalIgnoreCase))
                {
                    message = "Your authentication token is invalid.";
                    suggestion = "Please log in again to get a new token.";
                }
            }

            var response = new
            {
                success = false,
                title = "Authentication Required",
                message = message,
                suggestion = suggestion,
                traceId = context.HttpContext.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(response);
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                title = "Access Denied",
                message = "You don't have permission to access this resource.",
                suggestion = "Please contact your administrator if you need access.",
                traceId = context.HttpContext.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

//injecting DBContext
builder.Services.AddDbContext<EMSDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//injecting Repository
builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
builder.Services.AddScoped<IDepartmentRepository, SQLDepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
builder.Services.AddScoped<IRoleRepository, SQLRoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, SQLUserRoleRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


// Authentication
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Register Global Exception Handler Middleware first to catch all exceptions
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
