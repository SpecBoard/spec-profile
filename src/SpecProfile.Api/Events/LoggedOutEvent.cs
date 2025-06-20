using STrain;

namespace SpecProfile.Events
{
	public record LoggedOutEvent : Event
	{
		public required string User { get; init; }
	}
}
