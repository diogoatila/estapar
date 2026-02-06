using Estapar.Api.Middlewares;
using Estapar.Application.Garage;
using Estapar.Application.Revenue;
using Estapar.Application.Webhook;
using Estapar.Infrastructure.Bootstrap;
using Estapar.Infrastructure.Persistence;
using Estapar.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddHostedService<GarageSeedHostedService>();

//Services
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();
builder.Services.AddScoped<IGarageQueryService, GarageQueryService>();


// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(cs);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();