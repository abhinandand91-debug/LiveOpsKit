using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveOpsKit.Editor
{
    // Two events of the same type running at once usually means a copy-paste date mistake.
    public class EventOverlapRule : ConfigRule
    {
        public override IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            var timed = config.events.Where(e => !string.IsNullOrEmpty(e.type) && e.StateAt(now) != EventState.Invalid).ToList();

            for (int i = 0; i < timed.Count; i++)
                for (int j = i + 1; j < timed.Count; j++)
                    if (timed[i].type == timed[j].type && Overlap(timed[i], timed[j]))
                        yield return ConfigIssue.Warning(Label(config, timed[j]), $"Overlaps '{Label(config, timed[i])}', which has the same type '{timed[i].type}'.");
        }

        static bool Overlap(LiveOpsEvent a, LiveOpsEvent b)
        {
            a.TryGetWindow(out var aStart, out var aEnd);
            b.TryGetWindow(out var bStart, out var bEnd);
            return aStart < bEnd && bStart < aEnd;
        }
    }
}
