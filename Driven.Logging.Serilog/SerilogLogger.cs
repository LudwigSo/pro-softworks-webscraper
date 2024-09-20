using Application.Ports;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace Driven.Logging.Serilog;

public class SerilogLogger(IConfigurationRoot configuration) : ILogging
{
    private readonly Logger _logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .WriteTo.Console()
        .WriteTo.File("/app/logs/log.txt",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true)
        .CreateLogger();

    public void LogDebug(string message) => _logger.Debug(message);
    public void LogInformation(string message) => _logger.Information(message);
    public void LogError(string message) => _logger.Error(message);
    public void LogException(Exception exception, string message) => _logger.Fatal(exception, message);
    public void LogWarning(string message) => _logger.Warning(message);
}