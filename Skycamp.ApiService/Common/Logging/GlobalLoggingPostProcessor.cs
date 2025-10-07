using FastEndpoints;
using Skycamp.ApiService.Common.Reflection;

namespace Skycamp.ApiService.Common.Logging;

public class GlobalLoggingPostProcessor : IGlobalPostProcessor
{
    private readonly ILogger<GlobalLoggingPostProcessor> _logger;

    public GlobalLoggingPostProcessor(ILogger<GlobalLoggingPostProcessor> logger)
    {
        _logger = logger;
    }

    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        if (context.HasValidationFailures)
        {
            _logger.LogInformation("Processing failed with {ValidationErrors}", context.ValidationFailures);
        }
        else
        {
            var responseType = TypeNameHelper.GetFriendlyName(context.Response?.GetType());

            _logger.LogInformation("Processed response with {ResponseType}: {@Response}", responseType, context.Response);
        }

        return Task.CompletedTask;
    }
}