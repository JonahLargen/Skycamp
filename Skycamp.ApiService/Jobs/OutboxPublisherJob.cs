using Azure.Messaging.ServiceBus;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Jobs;

public class OutboxPublisherJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<OutboxPublisherJob> _logger;

    public OutboxPublisherJob(ApplicationDbContext dbContext, ServiceBusClient serviceBusClient, ILogger<OutboxPublisherJob> logger)
    {
        _dbContext = dbContext;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [DisableConcurrentExecution(60)]
    public async Task PublishUnprocessedMessagesAsync()
    {
        var unprocessedMessages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(1000)
            .ToListAsync();

        var _serviceBusSender = _serviceBusClient.CreateSender("outbox");

        foreach (var message in unprocessedMessages)
        {
            try
            {
                var serviceBusMessage = new ServiceBusMessage(message.Payload)
                {
                    Subject = message.Type,
                    MessageId = message.Id.ToString()
                };

                await _serviceBusSender.SendMessageAsync(serviceBusMessage);

                message.ProcessedOnUtc = DateTime.UtcNow;

                _dbContext.OutboxMessages.Update(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message with ID {MessageId}", message.Id);
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}