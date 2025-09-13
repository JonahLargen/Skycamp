using FastEndpoints;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Skycamp.ApiService.Common.Tracing;

public class GlobalTracingPostProcessor : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        if (context.HttpContext.Items.TryGetValue(GlobalTracingPreProcessor.ActivityKey, out var obj) && obj is Activity activity)
        {
            var requestName = context.Response?.GetType()?.Name ?? "UnknownRequest";
            var requestType = context.Response?.GetType()?.FullName ?? "UnknownRequestType";

            activity.SetTag("response.type", requestType);
            activity.SetTag("response.name", requestName);

            if (context.HasValidationFailures)
            {
                activity.AddEvent(new ActivityEvent("Request validation failed"));
                activity.SetStatus(ActivityStatusCode.Error);
                activity.SetTag("error.type", "ValidationFailure");
                activity.SetTag("validation.errors", context.ValidationFailures != null ? JsonConvert.SerializeObject(context.ValidationFailures) : "");
            }
            else if (context.HasExceptionOccurred && context.ExceptionDispatchInfo is not null)
            {
                var ex = context.ExceptionDispatchInfo.SourceException;

                activity.AddEvent(new ActivityEvent("Request failed"));
                activity.SetStatus(ActivityStatusCode.Error);
                activity.SetTag("error.type", "Exception");
                activity.SetTag("exception.type", ex.GetType().FullName ?? "");
                activity.SetTag("exception.message", ex.Message);
                activity.SetTag("exception.stacktrace", ex.StackTrace ?? "");

                // If you want to handle the exception here (optional)
                // context.MarkExceptionAsHandled();
            }
            else
            {
                activity.AddEvent(new ActivityEvent("Request processed"));
                activity.SetStatus(ActivityStatusCode.Ok);
            }

            activity.Dispose(); // End the activity
        }

        return Task.CompletedTask;
    }
}