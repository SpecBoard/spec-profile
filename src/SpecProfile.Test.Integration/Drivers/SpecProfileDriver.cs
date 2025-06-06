using Bogus;
using KurrentDB.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpecProfile.Commands;
using SpecProfile.Test.Integration.Helpers;
using System.Net.Http.Json;

namespace SpecProfile.Test.Integration.Drivers
{
	public class SpecProfileDriver : IDisposable
	{
		private readonly WebApplicationFactory<Program> _host = new();

		public TestContext Context { get; } = new TestContext();
		public string User { get; } = new Faker().Internet.UserName();

		public SpecProfileDriver()
		{
			_host = _host.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("Test");

				builder.ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(
				[
					new("Serilog:Properties:RunId", Context.RunId.ToString())
				]));
			});
		}

		public async Task SubscribeAsync(string stream, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> handler)
		{
			var context = _host.RunContext(Context);
			await _host.Services.GetRequiredService<KurrentDBClient>().SubscribeToStreamAsync(stream, FromStream.End, handler);
		}

		public async Task LoginAsync()
		{
			var client = _host.CreateClient();
			var command = new LogInCommand { UserName = User };

			var context = _host.RunContext(Context);

			client.DefaultRequestHeaders.Add("request-type", $"{command.GetType().FullName}, {command.GetType().Assembly.GetName().Name}");
			var response = await client.PostAsJsonAsync("api", command);

			response.EnsureSuccessStatusCode();
		}

		public void Dispose()
		{
			_host.Dispose();
		}
	}

	file static class SpecProfileDriverExtensions
	{
		public static IDisposable? RunContext(this WebApplicationFactory<Program> host, TestContext context)
		{
			var logger = host.Services.GetRequiredService<ILogger<SpecProfile.Application.Repositories.UserRepository>>();
			return logger.BeginScope(new Dictionary<string, object>
			{
				["RunId"] = context.RunId
			});
		}
	}
}
