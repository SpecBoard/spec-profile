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

        public void LogIn(string username)
        {
            var @event = new LoggedInEvent { Username = username };
            _events.Add(@event);
            Apply(@event);
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
