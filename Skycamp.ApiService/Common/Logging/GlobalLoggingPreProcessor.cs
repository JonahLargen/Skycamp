using FastEndpoints;

namespace Skycamp.ApiService.Common.Logging;

public class GlobalLoggingPreProcessor : IGlobalPreProcessor
{
    private readonly ILogger<GlobalLoggingPreProcessor> _logger;

    public GlobalLoggingPreProcessor(ILogger<GlobalLoggingPreProcessor> logger)
    {
        _logger = logger;
    }

    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var requestName = context.Request?.GetType()?.Name ?? "UnknownRequest";

        _logger.LogInformation("Processing request: {RequestName} - Data: {@Request}", requestName, context.Request);

        return Task.CompletedTask;
    }
}