using Bogus;
using SpecProfile.Domain;
using SpecProfile.Events;
using SpecProfile.Models;
using STrain.Eventing.Api;

namespace SpecProfile.Test.Unit
{
	public class UserAggregateRootTest
	{
		private UserAggregateRoot CreateSUT()
		{
			return new UserAggregateRoot();
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[UNIT][UAR-001] - Create User")]
		public void UserAggregateRoot_CreateUser()
		{
			// Arrange
			var sut = CreateSUT();
			var userName = new Faker().Internet.UserName();

			// Act
			sut.Create(userName);

			// Assert
			var state = (IAggregateRoot<UserState>)sut;
			Assert.Collection(state.UncommitedEvents, s => s.IsUserCreatedEvent(userName));
		}

		[Trait("Feature", "LogIn")]
		[Theory(DisplayName = "[UNIT][UAR-002] - UserName is empty if create user")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void UserAggregateRoot_CreateUser_UserNameIsEmpty(string? userName)
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
			Assert.Throws<ArgumentException>(() => sut.Create(userName!));
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[UNIT][UAR-003] - LogIn User")]
		public void UserAggregateRoot_LogIn()
		{
			// Arrange
			var sut = CreateSUT();
			var state = ((IAggregateRoot<UserState>)sut);

			state.State.Name = new Faker().Internet.UserName();

			// Act
			sut.LogIn();

			// Assert
			Assert.Equal(UserStatus.Online, state.State.Status);
			Assert.Collection(state.UncommitedEvents, s => s.IsLoggedInEvent(state.State.Name));
		}

		[Trait("Feature", "LogIn")]
		[Fact(DisplayName = "[UNIT][UAR-004] - UserName is empty if login User")]
		public void UserAggregateRoot_LogIn_UserNameIsEmpty()
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
			Assert.Throws<InvalidOperationException>(() => sut.LogIn());
		}

		[Trait("Feature", "LogOut")]
		[Fact(DisplayName = "[UNIT][UAR-005] - Logout")]
		public void UserAggregateRoot_LogOut_LogOut()
		{
			// Arrange
			var sut = CreateSUT();
			var state = ((IAggregateRoot<UserState>)sut);

			state.State.Status = UserStatus.Online;
			state.State.Name = new Faker().Internet.UserName();

			// Act
			sut.LogOut();

			// Assert
			Assert.Equal(UserStatus.Offline, state.State.Status);
			Assert.Collection(state.UncommitedEvents, s => s.IsLoggedOutEvent(state.State.Name));
		}

		[Trait("Feature", "LogOut")]
		[Fact(DisplayName = "[UNIT][UAR-006] - Logout not Logged In User")]
		public void UserAggregateRoot_LogOut_LogOutNotLoggedInUser()
		{
			// Arrange
			var sut = CreateSUT();
			var state = ((IAggregateRoot<UserState>)sut);

			state.State.Status = UserStatus.Offline;
			state.State.Name = new Faker().Internet.UserName();

			// Act
			sut.LogOut();

			// Assert
			Assert.Equal(UserStatus.Offline, state.State.Status);
			Assert.Empty(state.UncommitedEvents);
		}
	}

	file static class UserAggragateRootTestExtensions
	{
		public static void IsUserCreatedEvent(this IEvent @event, string expected)
		{
			Assert.Equal(Assert.IsType<UserCreatedEvent>(@event).User, expected);
		}

		public static void IsLoggedInEvent(this IEvent @event, string expected)
		{
			Assert.Equal(Assert.IsType<LoggedInEvent>(@event).User, expected);
		}

		public static void IsLoggedOutEvent(this IEvent @event, string expected)
		{
			Assert.Equal(Assert.IsType<LoggedOutEvent>(@event).User, expected);
		}
	}
}
