namespace Application.Ports;

public interface ILogging
{
    void LogDebug(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogException(Exception exception, string message);
}