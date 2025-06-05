using SpecProfile.Test.Integration.Drivers;

namespace SpecProfile.Test.Integration
{
    public class LoginTests : IClassFixture<SpecProfileDriver>
    {
        private readonly SpecProfileDriver _driver;

        public LoginTests(SpecProfileDriver driver)
        {
            _driver = driver;
        }

        [Fact(DisplayName = "[INTEGRATION][LGN-001]: Login")]
        public async Task Login()
        {
            // Arrange
            var raised = false;
            await _driver.SubscribeAsync($"profile-{_driver.User}", (_, @event, __) =>
            {
                raised = true;
                return Task.CompletedTask;
            });

            // Act
            await _driver.LoginAsync();

            // Assert
            await Task.Delay(1000);
            Assert.True(raised);
        }
    }
}
