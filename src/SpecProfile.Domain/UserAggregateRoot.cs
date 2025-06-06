using SpecProfile.Events;
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

			var @event = new UserCreatedEvent { UserName = username };
			_events.Add(@event);
			Apply(@event);
		}

		public void LogIn()
		{
			if (string.IsNullOrWhiteSpace(_state.Name)) throw new InvalidOperationException("User must be created before log-in");

			var @event = new LoggedInEvent { UserName = _state.Name };
			_events.Add(@event);
			Apply(@event);
		}

		public void Apply(LoggedInEvent @event) { }
		public void Apply(UserCreatedEvent @event)
		{
			_state.Name = @event.UserName;
		}
	}

	public record UserState
	{
		public string Name { get; set; } = null!;
	}
}
