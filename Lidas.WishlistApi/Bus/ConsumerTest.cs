using Lidas.WishlistApi.Entities;
using MassTransit;

namespace Lidas.WishlistApi.Bus;

public class ConsumerTest: IConsumer<Wish>
{
    private readonly ILogger<ConsumerTest> _logger;
    public ConsumerTest(ILogger<ConsumerTest> logger)
    {
        _logger = logger;
    }


    public Task Consume(ConsumeContext<Wish> context)
    {
        _logger.LogInformation($"Prossesing id: {context.MessageId}, mess {context.Message}");

        return Task.CompletedTask;
    }
}
