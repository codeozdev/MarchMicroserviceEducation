using System.Diagnostics;

namespace OpenTelemetryStock.API;

public static class ActivitySourceProvider
{
    public static ActivitySource ActivitySource { get; } = new(name: "MyActivitySource");
}