using Contracts;
using Lidas.LikeApi.Database;
using Lidas.LikeApi.Entities;
using MassTransit;

namespace Lidas.LikeApi.Consumers;

public class MangaCreateConsumer : IConsumer<MangaCreateEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<MangaCreateConsumer> _logger;

    public MangaCreateConsumer(AppDbContext context, ILogger<MangaCreateConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MangaCreateEvent> context)
    {
        _logger.LogInformation($"Received message: {context.Message.MangaId} from LikeApi");

        var likeItem = new Likeitem(context.Message.MangaId);

        _context.Likeitems.Add(likeItem);
        _context.SaveChanges();

        _logger.LogInformation("Finished create like item");

        return Task.CompletedTask;
    }
}
