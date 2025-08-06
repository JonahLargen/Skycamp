using System.Diagnostics;

namespace Skycamp.ApiService.Common.Telemetry;

public static class TelemetryConstants
{
    public const string ActivitySourceName = "Skycamp.ApiService";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}