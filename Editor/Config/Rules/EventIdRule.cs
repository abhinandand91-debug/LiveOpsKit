using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveOpsKit.Editor
{
    public class EventIdRule : ConfigRule
    {
        public override IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            foreach (var evt in config.events.Where(e => string.IsNullOrEmpty(e.id)))
                yield return ConfigIssue.Error(Label(config, evt), "Event has no id.");

            var duplicates = config.events
                .Where(e => !string.IsNullOrEmpty(e.id))
                .GroupBy(e => e.id)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicates)
                yield return ConfigIssue.Error(group.Key, $"Id is used by {group.Count()} events. Saved progress may mix between them.");
        }
    }
}
