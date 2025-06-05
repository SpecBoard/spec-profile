using Serilog;
using SpecProfile.Web.Wireups;
using STrain.CQS.NetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseLightInject();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.AddDependencies();
builder.AddCQS();
builder.AddEventing();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.MapGenericRequestController();

app.Run();

public partial class Program { }