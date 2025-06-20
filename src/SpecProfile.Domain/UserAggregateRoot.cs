using SpecProfile.Events;
using SpecProfile.Models;
using STrain.Eventing.Api;

namespace SpecProfile.Domain
{
	public interface IAggregateRoot<T>
	{
		T State { get; }
		IEnumerable<IEvent> UncommitedEvents { get; }
	}

	public class UserAggregateRoot : IAggregateRoot<UserState>
	{
		private readonly UserState _state = new();

		private readonly List<IEvent> _events = [];
		IEnumerable<IEvent> IAggregateRoot<UserState>.UncommitedEvents => _events;

		UserState IAggregateRoot<UserState>.State => _state;

		public void Create(string username)
		{
			if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Cannot be null or empty", nameof(username));

			var @event = new UserCreatedEvent { User = username };
			_events.Add(@event);
			Apply(@event);
		}

		public void LogIn()
		{
			if (string.IsNullOrWhiteSpace(_state.Name)) throw new InvalidOperationException("User must be created before log-in");

			var @event = new LoggedInEvent { User = _state.Name };
			_events.Add(@event);
			Apply(@event);
		}

		public void LogOut()
		{
			if (_state.Status != UserStatus.Online) return;

			var @event = new LoggedOutEvent { User = _state.Name };
			_events.Add(@event);
			Apply(@event);
		}

		public void Apply(LoggedInEvent @event)
		{
			_state.Status = UserStatus.Online;
		}
		public void Apply(LoggedOutEvent @event)
		{
			_state.Status = UserStatus.Offline;
		}
		public void Apply(UserCreatedEvent @event)
		{
			_state.Name = @event.User;
		}
	}

	public record UserState
	{
		public string Name { get; set; } = null!;
		public UserStatus Status { get; set; } = UserStatus.Unknown;
	}
}
