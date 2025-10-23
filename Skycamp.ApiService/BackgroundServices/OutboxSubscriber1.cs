using Azure.Messaging.ServiceBus;

namespace Skycamp.ApiService.BackgroundServices;

public class OutboxSubscriber1 : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<OutboxSubscriber1> _logger;

    public OutboxSubscriber1(ServiceBusClient client, ILogger<OutboxSubscriber1> logger)
    {
        _processor = client.CreateProcessor("outbox", "outbox-subscription-1", new ServiceBusProcessorOptions());
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += OnMessageReceived;
        _processor.ProcessErrorAsync += OnError;
        return _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        _logger.LogInformation("Received message from OutboxSubscriber1: {MessageBody}", body);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message in OutboxSubscriber1: {ErrorSource}", args.ErrorSource);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}