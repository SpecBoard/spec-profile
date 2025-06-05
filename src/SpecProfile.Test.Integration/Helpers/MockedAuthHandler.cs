using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SpecProfile.Test.Integration.Helpers
{
    internal class MockedAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string User { get; set; } = null!;
    }

    internal class MockedAuthHandler : AuthenticationHandler<MockedAuthenticationSchemeOptions>
    {
        public const string SCHEME = "MockedScheme";

        public MockedAuthHandler(IOptionsMonitor<MockedAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        public string User { get; set; } = null!;

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, User) };
            var identity = new ClaimsIdentity(claims, "Mocked");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SCHEME);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    internal static class MockedAuthHandlerExtensions
    {
        public static void AddMockedAuthHandler(this IServiceCollection services, Action<MockedAuthenticationSchemeOptions> configure)
        {
            services.AddAuthentication(defaultScheme: MockedAuthHandler.SCHEME)
                .AddScheme<MockedAuthenticationSchemeOptions, MockedAuthHandler>(MockedAuthHandler.SCHEME, configure);
        }
    }
}
