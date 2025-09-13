using FastEndpoints;
using System.Diagnostics;

namespace Skycamp.ApiService.Common.Tracing;

public class GlobalTracingPreProcessor : IGlobalPreProcessor
{
    public const string ActivityKey = "__GlobalTracingPreProcessor_Activity";

    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var requestName = context.Request?.GetType()?.Name ?? "UnknownRequest";
        var requestType = context.Request?.GetType()?.FullName ?? "UnknownRequestType";
        var activity = TelemetryConstants.ActivitySource.StartActivity(requestName, ActivityKind.Server);

        if (activity != null)
        {
            activity.SetTag("otel.library.name", TelemetryConstants.ActivitySourceName);
            activity.SetTag("component", "endpoint");
            activity.SetTag("endpoint.type", requestType);
            activity.SetTag("endpoint.name", requestName);
            activity.AddEvent(new ActivityEvent("Endpoint processing started"));

            context.HttpContext.Items[ActivityKey] = activity;
        }

        return Task.CompletedTask;
    }
}