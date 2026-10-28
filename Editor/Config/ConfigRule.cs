using System;
using System.Collections.Generic;

namespace LiveOpsKit.Editor
{
    // Base class for a config rule. Any subclass in the project is picked up automatically.
    public abstract class ConfigRule
    {
        public abstract IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now);
    }
}
