namespace Domain.Ports;

public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogException(Exception exception, string message);
}