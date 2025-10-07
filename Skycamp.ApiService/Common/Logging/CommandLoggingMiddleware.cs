using FastEndpoints;
using Skycamp.ApiService.Common.Reflection;

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
        var commandType = TypeNameHelper.GetFriendlyName(command.GetType());

        _logger.LogInformation("Handling command {CommandType}: {@Command}", commandType, command);

        var result = await next();

        var resultType = TypeNameHelper.GetFriendlyName(result?.GetType());

        _logger.LogInformation("Handled command with result {ResultType}: {@Result}", resultType, result);

        return result;
    }
}