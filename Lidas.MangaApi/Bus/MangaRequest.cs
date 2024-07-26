using Contracts;
using Lidas.MangaApi.Persist;
using MassTransit;

namespace Lidas.MangaApi.Bus;

public sealed class MangaRequestConsumer : IConsumer<MangaRequestEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<MangaRequestConsumer> _logger;
    public MangaRequestConsumer(AppDbContext context, ILogger<MangaRequestConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MangaRequestEvent> context)
    {
        var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == context.Message.Id);

        if (manga == null)
        {
            _logger.LogInformation("Manga not found");
            return Task.CompletedTask;
        } 
        else
        {
            _logger.LogInformation($"Manga selected: Id: {manga.Id}, Title: {manga.Name}");
            return Task.CompletedTask;
        }
    }
}
