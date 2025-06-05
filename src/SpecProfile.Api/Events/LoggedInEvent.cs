using STrain;

namespace SpecProfile.Events
{
    public record LoggedInEvent : Event
    {
        public required string Username { get; init; }
    }
}
