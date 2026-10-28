using System;
using System.Collections.Generic;

namespace LiveOpsKit.Editor
{
    // Base class for a config rule. Any subclass in the project is picked up automatically.
    public abstract class ConfigRule
    {
        public abstract IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now);

        // The event id, or its position when the id is missing.
        protected static string Label(LiveOpsConfig config, LiveOpsEvent evt) =>
            string.IsNullOrEmpty(evt.id) ? $"event #{config.events.IndexOf(evt) + 1}" : evt.id;
    }
}
