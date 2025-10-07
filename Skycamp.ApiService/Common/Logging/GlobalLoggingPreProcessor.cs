using FastEndpoints;
using Skycamp.ApiService.Common.Reflection;

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
        var requestType = TypeNameHelper.GetFriendlyName(context.Request?.GetType());

        _logger.LogInformation("Handling request {RequestType}: {@Request}", requestType, context.Request);

        return Task.CompletedTask;
    }
}