using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SavedMind.Api.Middleware;
using SavedMind.Application.Common.Behaviors;
using SavedMind.Application.Features.Auth.Register;
using SavedMind.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add DbContext
builder.Services.AddDbContext<SavedMind.Infrastructure.Persistence.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));


// MediatR + FluentValidation Pipeline
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<RegisterCommand>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Auth endpoints: 10 requests per hour per IP
    options.AddFixedWindowLimiter("auth", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromHours(1);
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
