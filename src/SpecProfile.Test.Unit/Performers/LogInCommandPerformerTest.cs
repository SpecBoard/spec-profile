using Microsoft.Extensions.Logging;
using Moq;
using SpecProfile.Application.Performers;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using SpecProfile.Commands.Validators;
using SpecProfile.Domain;
using SpecProfile.Events;
using SpecProfile.Test.Unit.Fakers;
using Xunit.Abstractions;

namespace SpecProfile.Test.Unit.CommandHandlers
{
	public class LogInCommandPerformerTest
	{

		private readonly ILogger<LogInCommandPerformer> _logger;
		private Mock<IUserRepository> _repositoryMock;

		public LogInCommandPerformerTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<LogInCommandPerformer>();
		}

		private LogInCommandPerformer CreateSUT()
		{
			_repositoryMock = new Mock<IUserRepository>();

			return new LogInCommandPerformer(_repositoryMock.Object, _logger);
		}

		[Trait("Feature", "LogIn")]
		[Theory(DisplayName = "[UNIT][LIC-001] - UserName is empty")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public async Task LogInComment_ValidateAsync_UserNameIsEmpty(string? userName)
		{
			// Arrange
			var sut = new LogInCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new LogInCommandFaker().UserName(userName).Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[UNIT][LIC-002] - LogIn with New User")]
		public async Task LogInCommand_PerformAsync_LogInWithNewUser()
		{
			// Arrange
			var sut = CreateSUT();
			var command = new LogInCommandFaker().Generate();

			// Act
			await sut.PerformAsync(command, default);

			// Assert
			_repositoryMock.Verify(r => r.SaveAsync(It.Is<UserAggregateRoot>(uar => uar.Validate(command)), It.IsAny<CancellationToken>()), Times.Once);
		}
	}

	file static class LogInCommandPerformerTestExtensions
	{
		public static bool Validate(this UserAggregateRoot aggregateRoot, LogInCommand command)
		{
			var state = (IAggregateRoot<UserState>)aggregateRoot;
			return state.UncommitedEvents.First() is UserCreatedEvent userCreatedEvent && userCreatedEvent.UserName.Equals(command.UserName)
				&& state.UncommitedEvents.ElementAt(1) is LoggedInEvent loggedInEvent && loggedInEvent.UserName.Equals(command.UserName);
		}
	}
}
