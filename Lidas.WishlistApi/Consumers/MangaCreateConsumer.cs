using Contracts;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using MassTransit;

namespace Lidas.WishlistApi.Consumers;

public class MangaCreateConsumer : IConsumer<MangaCreateEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<MangaCreateConsumer> _logger;
    public MangaCreateConsumer(AppDbContext context ,ILogger<MangaCreateConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }
    public Task Consume(ConsumeContext<MangaCreateEvent> context)
    {
        _logger.LogInformation($"Received message: {context.Message.MangaId}");

        var wishItem = new WishItem(context.Message.MangaId);

        _context.Wishitems.Add(wishItem);
        _context.SaveChanges();

        _logger.LogInformation("Finished create wishitem");

        return Task.CompletedTask;
    }
}
