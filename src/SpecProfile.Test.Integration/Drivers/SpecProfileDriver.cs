using Bogus;
using KurrentDB.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SpecProfile.Test.Integration.Helpers;

namespace SpecProfile.Test.Integration.Drivers
{
    public class SpecProfileDriver
    {
        private readonly WebApplicationFactory<Program> _host = new();

        public string User { get; } = new Faker().Internet.UserName();

        public SpecProfileDriver()
        {
            _host = _host.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMockedAuthHandler(options => options.User = User);
                });

                builder.UseEnvironment("Test");
            });
        }

        public async Task SubscribeAsync(string stream, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> handler)
        {
            await _host.Services.GetRequiredService<KurrentDBClient>().SubscribeToStreamAsync(stream, FromStream.End, handler);
        }

        public async Task LoginAsync()
        {
            var client = _host.CreateClient();

            var response = await client.PostAsync("login", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
