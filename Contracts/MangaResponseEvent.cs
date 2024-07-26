namespace Contracts;

public record MangaResponseEvent
{
    public Guid Id { get; set; }
    public DateTime ResponsedAt { get; set; }
}