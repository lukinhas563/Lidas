using Contracts;
using Lidas.LikeApi.Database;
using Lidas.LikeApi.Entities;
using MassTransit;

namespace Lidas.LikeApi.Consumers;

public class UserCreateLikeConsumer : IConsumer<UserCreateEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserCreateLikeConsumer> _logger;
    public UserCreateLikeConsumer(AppDbContext context, ILogger<UserCreateLikeConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreateEvent> context)
    {
        _logger.LogInformation($"Received message: {context.Message.UserId} from LikeApi");

        var likeList = new Likelist(context.Message.UserId);

        _context.Likelists.Add(likeList);
        _context.SaveChanges();

        _logger.LogInformation("Finished create Likelist");

        return Task.CompletedTask;
    }
}
