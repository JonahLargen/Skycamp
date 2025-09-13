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
        _logger.LogInformation("Executing command: {name}", command.GetType().Name);

        var result = await next();

        _logger.LogInformation("Got result: {@value}", result);

        return result;
    }
}