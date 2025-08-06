using FastEndpoints;

namespace Skycamp.ApiService.Common.Middleware;

public class LoggingGlobalPreProcessor : IGlobalPreProcessor
{
    private readonly ILogger<LoggingGlobalPreProcessor> _logger;

    public LoggingGlobalPreProcessor(ILogger<LoggingGlobalPreProcessor> logger)
    {
        _logger = logger;
    }

    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        _logger.LogInformation("Processing request: {@request}", context.Request);

        return Task.CompletedTask;
    }
}