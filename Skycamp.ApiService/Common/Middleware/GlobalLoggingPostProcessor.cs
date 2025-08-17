using FastEndpoints;

namespace Skycamp.ApiService.Common.Middleware;

public class GlobalLoggingPostProcessor : IGlobalPostProcessor
{
    private readonly ILogger<GlobalLoggingPostProcessor> _logger;

    public GlobalLoggingPostProcessor(ILogger<GlobalLoggingPostProcessor> logger)
    {
        _logger = logger;
    }

    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        var responseName = context.Response?.GetType()?.Name ?? "UnknownResponse";

        _logger.LogInformation("Processed response: {ResponseName} - Data: {@Response}", responseName, context.Response);

        return Task.CompletedTask;
    }
}