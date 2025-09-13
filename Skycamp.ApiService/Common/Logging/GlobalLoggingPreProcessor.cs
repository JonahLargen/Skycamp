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
        var requestType = context.Request?.GetType();
        var requestName = requestType != null ? LoggablePropertyHelper.GetFriendlyTypeName(requestType) : "UnknownRequest";
        var isLoggable = requestType != null && Attribute.IsDefined(requestType, typeof(LoggableAttribute));

        if (isLoggable)
        {
            var loggableProps = LoggablePropertyHelper.GetLoggableProperties(context.Request);

            _logger.LogInformation("Processing request: {RequestName} - Data: {@Request}", requestName, loggableProps);
        }
        else
        {
            _logger.LogInformation("Processing request: {RequestName} - Data omitted", requestName);
        }

        return Task.CompletedTask;
    }
}