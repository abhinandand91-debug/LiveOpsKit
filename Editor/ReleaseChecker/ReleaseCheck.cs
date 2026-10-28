using System.Collections.Generic;

namespace LiveOpsKit.Editor
{
    // Base class for a release check. Any subclass in the project is picked up automatically.
    public abstract class ReleaseCheck
    {
        public abstract string Name { get; }
        public abstract IEnumerable<CheckResult> Run(ReleaseSettings settings);
    }
}
