using STrain;

namespace SpecProfile.Commands
{
    public record LogInCommand : Command
    {
        public required string User { get; init; }
    }
}
