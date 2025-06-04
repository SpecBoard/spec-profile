using SpecProfile.Api;
using STrain.Eventing.Api;

namespace SpecProfile.Domain
{
	public class UserAggregateRoot
	{
		private readonly UserState _state = new();

		private readonly List<IEvent> _events = [];
		public IEnumerable<IEvent> UncommitedEvents => _events;

		public void LogIn(string username)
		{
			_events.Add(new LoggedInEvent() { Username = username });
		}

		public void Apply(LoggedInEvent loggedInEvent)
		{
			_state.UserName = loggedInEvent.Username;
		}
	}

	public record UserState
	{
		public string UserName { get; set; } = null!;
	}
}
