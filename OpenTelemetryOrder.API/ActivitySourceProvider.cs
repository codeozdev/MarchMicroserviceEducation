using System.Diagnostics;

namespace OpenTelemetryOrder.API;

public static class ActivitySourceProvider
{
    public static ActivitySource ActivitySource { get; } = new(name: "MyActivitySource");
}