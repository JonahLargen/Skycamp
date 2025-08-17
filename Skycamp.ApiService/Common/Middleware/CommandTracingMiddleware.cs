using FastEndpoints;
using Newtonsoft.Json;
using Skycamp.ApiService.Common.Telemetry;
using System.Diagnostics;

namespace Skycamp.ApiService.Common.Middleware;

public class CommandTracingMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public CommandTracingMiddleware()
    {
        
    }

    public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity(command.GetType().Name, ActivityKind.Internal);

        activity?.SetTag("otel.library.name", TelemetryConstants.ActivitySourceName);
        activity?.SetTag("command.type", command.GetType().FullName);
        activity?.SetTag("command.name", command.GetType().Name);
        activity?.SetTag("result.type", typeof(TResult).FullName);

        try
        {
            activity?.AddEvent(new ActivityEvent("Command executing"));

            var result = await next();

            activity?.AddEvent(new ActivityEvent("Command executed"));
            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (ValidationFailureException vex)
        {
            activity?.AddEvent(new ActivityEvent("Command validation failed"));
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.SetTag("error.type", "ValidationFailure");
            activity?.SetTag("validation.errors", vex.Failures != null ? JsonConvert.SerializeObject(vex.Failures) : "");
            throw;
        }
        catch (Exception ex)
        {
            activity?.AddEvent(new ActivityEvent("Command failed"));
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.SetTag("error.type", "Exception");
            activity?.SetTag("exception.type", ex.GetType().FullName ?? "");
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace ?? "");
            throw;
        }
    }
}