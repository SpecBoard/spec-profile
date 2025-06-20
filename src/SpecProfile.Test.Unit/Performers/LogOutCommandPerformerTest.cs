using AutoBogus;
using Microsoft.Extensions.Logging;
using Moq;
using SpecProfile.Application.Performers;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using SpecProfile.Commands.Validators;
using SpecProfile.Domain;
using SpecProfile.Events;
using Xunit.Abstractions;

namespace SpecProfile.Test.Unit.Performers
{
	public class LogOutCommandPerformerTest
	{

		private readonly ILogger<LogOutCommandPerformer> _logger;
		private Mock<IUserRepository> _repositoryMock;

		public LogOutCommandPerformerTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<LogOutCommandPerformer>();
		}

		private LogOutCommandPerformer CreateSUT()
		{
			_repositoryMock = new Mock<IUserRepository>();

			return new LogOutCommandPerformer(_repositoryMock.Object, _logger);
		}

		[Trait("Feature", "LogOut")]
		[Theory(DisplayName = "[UNIT][LGO-001] - User is empty")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public async Task LogOutCommand_ValidateAsync_UserIsEmpty(string? user)
		{
			// Arrange
			var sut = new LogOutCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new AutoFaker<LogOutCommand>().RuleFor(c => c.User, user).Generate());

			// Assert
			Assert.False(result.IsValid);
		}

		[Trait("Feature", "LogOut")]
		[Fact(DisplayName = "[UNIT][LGO-002] - LogOut")]
		public async Task LogOutCommandPerformer_PerformAsync_LogOut()
		{
			// Arrange
			var sut = CreateSUT();
			var command = new AutoFaker<LogOutCommand>().Generate();
			var aggregateRoot = new UserAggregateRoot();

			var state = (IAggregateRoot<UserState>)aggregateRoot;
			state.State.Name = command.User;
			state.State.Status = Models.UserStatus.Online;

			_repositoryMock.Setup(r => r.GetAsync(command.User, It.IsAny<CancellationToken>()))
				.ReturnsAsync(aggregateRoot);

			// Act
			await sut.PerformAsync(command, default);

			// Assert
			Assert.Single(state.UncommitedEvents, e => e is LoggedOutEvent);
			_repositoryMock.Verify(r => r.SaveAsync(It.Is<IAggregateRoot<UserState>>(us => us.State.Name == command.User), It.IsAny<CancellationToken>()));
		}
	}
}
