namespace PuzzleBox.CodeContract
{
  public class LogEvent
  {
    public LogSeverity Severity { get; set; }
    public string Message { get; set; }

    public LogEvent() { }
    public LogEvent(LogSeverity severity, string message)
    {
      Severity = severity;
      Message = message;
    }

    public override string ToString()
    {
      return $"{Severity}: {Message}";
    }
  }

  public enum LogSeverity { Debug, Information, Warning, Error }
}