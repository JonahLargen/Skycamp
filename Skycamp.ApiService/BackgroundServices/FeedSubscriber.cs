using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Skycamp.ApiService.Hubs;

namespace Skycamp.ApiService.BackgroundServices;

public class FeedSubscriber : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<FeedSubscriber> _logger;
    private readonly IHubContext<FeedHub> _hubContext;

    public FeedSubscriber(ServiceBusClient client, ILogger<FeedSubscriber> logger, IHubContext<FeedHub> hubContext)
    {
        _processor = client.CreateProcessor("outbox", "outbox-subscription-feed", new ServiceBusProcessorOptions());
        _logger = logger;
        _hubContext = hubContext;
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

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", body);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message in FeedSubscriber: {ErrorSource}", args.ErrorSource);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}