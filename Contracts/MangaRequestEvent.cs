namespace Contracts;

public record MangaRequestEvent
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
}
