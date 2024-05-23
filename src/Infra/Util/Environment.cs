namespace Infra.Util;

public static class Environment
{
    public static string Name { get; } = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
}
