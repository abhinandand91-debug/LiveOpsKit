namespace LiveOpsKit.Editor
{
    // One problem found in the LiveOps config. EventId is empty for config-wide problems.
    public class ConfigIssue
    {
        public Severity Severity { get; }
        public string EventId { get; }
        public string Message { get; }

        public ConfigIssue(Severity severity, string eventId, string message)
        {
            Severity = severity;
            EventId = eventId;
            Message = message;
        }

        public static ConfigIssue Error(string eventId, string message) => new ConfigIssue(Severity.Error, eventId, message);
        public static ConfigIssue Warning(string eventId, string message) => new ConfigIssue(Severity.Warning, eventId, message);
    }
}
