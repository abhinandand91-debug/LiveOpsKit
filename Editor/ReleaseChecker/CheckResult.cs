using System;

namespace LiveOpsKit.Editor
{
    public enum Severity { Pass, Warning, Error }

    // One finding from a release check, with an optional one-click fix.
    public class CheckResult
    {
        public string Check { get; internal set; }
        public Severity Severity { get; }
        public string Message { get; }
        public Action Fix { get; }

        public CheckResult(Severity severity, string message, Action fix = null)
        {
            Severity = severity;
            Message = message;
            Fix = fix;
        }

        public static CheckResult Pass(string message) => new CheckResult(Severity.Pass, message);
        public static CheckResult Warning(string message, Action fix = null) => new CheckResult(Severity.Warning, message, fix);
        public static CheckResult Error(string message, Action fix = null) => new CheckResult(Severity.Error, message, fix);
    }
}
