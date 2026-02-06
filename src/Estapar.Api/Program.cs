using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Estapar.Application.Garage;
using Estapar.Infrastructure.Clients;
using Estapar.Infrastructure.HostedServices;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<GarageBootstrapHostedService>();
builder.Services.AddHttpClient<IGarageClient, GarageClient>(client =>
{
    var baseUrl = builder.Configuration["GarageSimulator:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("GarageSimulator:BaseUrl is not configured.");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});


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

app.UseHttpsRedirection();
app.MapControllers();

app.Run();