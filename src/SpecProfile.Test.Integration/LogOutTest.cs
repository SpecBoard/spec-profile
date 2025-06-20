using SpecProfile.Test.Integration.Drivers;
using Xunit.Abstractions;

namespace SpecProfile.Test.Integration
{
	public class LogOutTest
	{
		private readonly SpecProfileDriver _driver = new();

		public LogOutTest(ITestOutputHelper output)
		{
			output.WriteLine("Starting test context: {0}", _driver.Context.RunId.ToString());
		}

		[Trait("Feature", "LogOut")]
		[Fact(DisplayName = "[INTEGRATION][LGO-001]: LogOut")]
		public async Task LogOut()
		{
			// Arrange
			var loggedOut = false;

			await _driver.LoginAsync();

			await _driver.SubscribeAsync($"profile-{_driver.User}", (_, @event, __) =>
			{
				loggedOut = @event.Event.EventType == "SpecProfile.Events.LoggedOutEvent, SpecProfile.Api";
				return Task.CompletedTask;
			});

			// Act
			await _driver.LogOutAsync();

			// Assert
			await Task.Delay(1000);
			Assert.True(loggedOut);
		}
	}
}
