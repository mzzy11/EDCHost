using dotenv.net;
using dotenv.net.Utilities;
using Serilog;

namespace EdcHost;

internal class Program
{
    internal static void Main()
    {
        // Setup logger using default settings before calling dotenv.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            SetupDotEnv();
            SetupSerilog();

            EdcHost edcHost = new();

        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "encountered an unhandled exception");
        }
    }

    /// <summary>
    /// Load environment variables from .env file.
    /// </summary>
    internal static void SetupDotEnv()
    {
        DotEnv.Load();
    }

    /// <summary>
    /// Setup Serilog logger.
    /// </summary>
    internal static void SetupSerilog()
    {
        // Get logging level from environment variables
        if (EnvReader.TryGetStringValue("LOGGING_LEVEL", out string? loggingLevelString) == false)
        {
            Log.Warning("LOGGING_LEVEL not set, using default value: Information");
            loggingLevelString = "Information";
        }

        // Configure Serilog
        Log.Logger = loggingLevelString switch
        {
            "Debug" => new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger(),
            "Information" => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger(),
            "Warning" => new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .CreateLogger(),
            "Error" => new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .CreateLogger(),
            "Fatal" => new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .WriteTo.Console()
                .CreateLogger(),
            _ => throw new ArgumentOutOfRangeException(nameof(loggingLevelString), loggingLevelString, "invalid logging level")
        };
    }
}
