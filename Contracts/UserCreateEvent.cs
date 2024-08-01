namespace Contracts;

public record UserCreateEvent
{
    public Guid UserId { get; set; }
}
