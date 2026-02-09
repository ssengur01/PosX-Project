using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace BuildingBlocks.Logging;

/// <summary>
/// Provides static methods to configure Serilog logging
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configures Serilog with standard settings for the application
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="serviceName">Name of the service for log enrichment</param>
    /// <returns>Configured LoggerConfiguration</returns>
    public static LoggerConfiguration ConfigureSerilog(IConfiguration configuration, string serviceName)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: $"logs/{serviceName}-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30);

        // Add Seq if configured
        var seqUrl = configuration["Serilog:SeqUrl"];
        if (!string.IsNullOrEmpty(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }

        return loggerConfiguration;
    }

    /// <summary>
    /// Creates and configures a Serilog logger
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="serviceName">Name of the service for log enrichment</param>
    /// <returns>Configured ILogger</returns>
    public static ILogger CreateLogger(IConfiguration configuration, string serviceName)
    {
        return ConfigureSerilog(configuration, serviceName).CreateLogger();
    }
}
