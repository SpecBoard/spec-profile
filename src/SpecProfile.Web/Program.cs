using Serilog;
using SpecProfile.Web.Wireups;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseLightInject();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.UseCQS();
builder.UseEventing();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
