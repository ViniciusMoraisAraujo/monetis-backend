using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Monetis.API.BackgroundServices;
using Monetis.API.Middlewares;
using Monetis.Infrastructure;
using Monetis.Application;
using Monetis.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddScoped<UserContext>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddHostedService<OverdueExpenseProcessorService>();

builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("GlobalPolicy", opt => {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseMiddleware<UserContextMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();