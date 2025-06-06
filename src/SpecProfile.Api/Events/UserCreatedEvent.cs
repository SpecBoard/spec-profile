using STrain;

namespace SpecProfile.Events
{
	public record UserCreatedEvent : Event
	{
		public required string UserName { get; init; }
	}
}
