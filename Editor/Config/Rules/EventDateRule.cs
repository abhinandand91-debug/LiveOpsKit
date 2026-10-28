using System;
using System.Collections.Generic;

namespace LiveOpsKit.Editor
{
    public class EventDateRule : ConfigRule
    {
        public override IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            foreach (var evt in config.events)
                foreach (var issue in Check(evt, Label(config, evt), settings, now))
                    yield return issue;
        }

        static IEnumerable<ConfigIssue> Check(LiveOpsEvent evt, string label, ConfigSettings settings, DateTime now)
        {
            bool hasStart = UtcDate.TryParse(evt.startUtc, out var start);
            bool hasEnd = UtcDate.TryParse(evt.endUtc, out var end);

            if (!hasStart) yield return ConfigIssue.Error(label, $"startUtc '{evt.startUtc}' is not a valid date.");
            if (!hasEnd) yield return ConfigIssue.Error(label, $"endUtc '{evt.endUtc}' is not a valid date.");
            if (!hasStart || !hasEnd) yield break;

            if (!UtcDate.HasTimeZone(evt.startUtc) || !UtcDate.HasTimeZone(evt.endUtc))
                yield return ConfigIssue.Warning(label, "Dates have no time zone. Add 'Z' so every player gets the same time.");

            if (end <= start)
            {
                yield return ConfigIssue.Error(label, "Event ends before it starts.");
                yield break;
            }

            double days = (end - start).TotalDays;
            if (days > settings.maxEventDays)
                yield return ConfigIssue.Warning(label, $"Event runs for {days:0} days, longer than the {settings.maxEventDays} day limit.");

            if (end <= now)
                yield return ConfigIssue.Warning(label, $"Event already ended on {end:yyyy-MM-dd HH:mm} UTC.");
        }
    }
}
