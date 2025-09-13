using FastEndpoints;

namespace Skycamp.ApiService.Common.Logging;

public class CommandLoggingMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ILogger<CommandLoggingMiddleware<TCommand, TResult>> _logger;

    public CommandLoggingMiddleware(ILogger<CommandLoggingMiddleware<TCommand, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
    {
        var commandType = command?.GetType();
        var commandName = commandType != null ? LoggablePropertyHelper.GetFriendlyTypeName(commandType) : "UnknownCommand";
        var isCommandLoggable = commandType != null && Attribute.IsDefined(commandType, typeof(LoggableAttribute));

        if (isCommandLoggable)
        {
            var loggableProps = LoggablePropertyHelper.GetLoggableProperties(command);
            _logger.LogInformation("Executing command: {CommandName} - Data: {@Command}", commandName, loggableProps);
        }
        else
        {
            _logger.LogInformation("Executing command: {CommandName} - Data omitted", commandName);
        }

        var result = await next();

        var resultType = result?.GetType();
        var resultName = resultType != null ? LoggablePropertyHelper.GetFriendlyTypeName(resultType) : "UnknownResult";
        var isResultLoggable = resultType != null && Attribute.IsDefined(resultType, typeof(LoggableAttribute));

        if (isResultLoggable)
        {
            var loggableResult = LoggablePropertyHelper.GetLoggableProperties(result);
            _logger.LogInformation("Got result: {ResultName} - Data: {@Result}", resultName, loggableResult);
        }
        else
        {
            _logger.LogInformation("Got result: {ResultName} - Data omitted", resultName);
        }

        return result;
    }
}