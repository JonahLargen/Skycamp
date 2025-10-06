using FastEndpoints;

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
            _logger.LogInformation("Request processing failed due to validation errors: {Errors}", context.ValidationFailures);
        }
        else
        {
            var responseType = context.Response?.GetType();
            var responseName = responseType != null ? LoggablePropertyHelper.GetFriendlyTypeName(responseType) : "UnknownResponse";
            var isLoggable = responseType != null && Attribute.IsDefined(responseType, typeof(LoggableAttribute));

            if (isLoggable)
            {
                var loggableProps = LoggablePropertyHelper.GetLoggableProperties(context.Response);

                _logger.LogInformation("Processed response: {ResponseName} - Data: {@Response}", responseName, loggableProps);
            }
            else
            {
                _logger.LogInformation("Processed response: {ResponseName} - Data omitted", responseName);
            }
        }

        return Task.CompletedTask;
    }
}