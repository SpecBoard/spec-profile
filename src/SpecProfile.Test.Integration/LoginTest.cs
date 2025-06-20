using SpecProfile.Test.Integration.Drivers;
using Xunit.Abstractions;

namespace SpecProfile.Test.Integration
{
	public class LoginTest
	{
		private readonly SpecProfileDriver _driver = new();

		public LoginTest(ITestOutputHelper output)
		{
			output.WriteLine("Starting test context: {0}", _driver.Context.RunId.ToString());
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[INTEGRATION][LGN-001]: First LogIn")]
		public async Task FirstLogin()
		{
			// Arrange
			var created = false;
			var loggedIn = false;
			await _driver.SubscribeAsync($"profile-{_driver.User}", (_, @event, __) =>
			{
				created = created || @event.Event.EventType == "SpecProfile.Events.UserCreatedEvent, SpecProfile.Api";
				loggedIn = loggedIn || @event.Event.EventType == "SpecProfile.Events.LoggedInEvent, SpecProfile.Api";
				return Task.CompletedTask;
			});

			// Act
			await _driver.LoginAsync();

			// Assert
			await Task.Delay(1000);
			Assert.True(created && loggedIn);
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[INTEGRATION][LGN-002]: LogIn")]
		public async Task Login()
		{
			// Arrange
			var loggedIn = false;

			await _driver.LoginAsync();
			await Task.Delay(200);

			await _driver.SubscribeAsync($"profile-{_driver.User}", (_, @event, __) =>
			{
				loggedIn = loggedIn || @event.Event.EventType == "SpecProfile.Events.LoggedInEvent, SpecProfile.Api";
				return Task.CompletedTask;
			});

			// Act
			await _driver.LoginAsync();

			// Assert
			await Task.Delay(1000);
			Assert.True(loggedIn);
		}
	}
}
