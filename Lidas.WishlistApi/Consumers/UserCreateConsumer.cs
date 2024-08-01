using Contracts;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using MassTransit;

namespace Lidas.WishlistApi.Consumers;

public sealed class UserCreateConsumer : IConsumer<UserCreateEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserCreateConsumer> _logger;
    public UserCreateConsumer(AppDbContext context, ILogger<UserCreateConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreateEvent> context)
    {
        _logger.LogInformation($"Received message: {context.Message.UserId}");

        var wishList = new Wishlist(context.Message.UserId);

        _context.Wishlists.Add(wishList);
        _context.SaveChanges();

        _logger.LogInformation("Finished create wishlist");

        return Task.CompletedTask;
    }
}
