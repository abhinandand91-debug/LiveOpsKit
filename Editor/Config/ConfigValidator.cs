using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public static class ConfigValidator
    {
        public static List<ConfigIssue> Validate(string json, ConfigSettings settings, DateTime now, out LiveOpsConfig config)
        {
            config = null;
            if (string.IsNullOrWhiteSpace(json))
                return new List<ConfigIssue> { ConfigIssue.Error("", "Config is empty.") };

            try
            {
                config = LiveOpsConfig.FromJson(json);
            }
            catch (ArgumentException e)
            {
                return new List<ConfigIssue> { ConfigIssue.Error("", $"Config is not valid JSON: {e.Message}") };
            }

            return Validate(config, settings, now);
        }

        public static List<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            var issues = new List<ConfigIssue>();
            config.events ??= new List<LiveOpsEvent>();
            if (config.events.Count == 0)
                issues.Add(ConfigIssue.Warning("", "Config has no events."));

            foreach (var rule in CreateRules())
                issues.AddRange(rule.Validate(config, settings, now));

            return issues.OrderByDescending(i => i.Severity).ToList();
        }

        static IEnumerable<ConfigRule> CreateRules() =>
            TypeCache.GetTypesDerivedFrom<ConfigRule>()
                .Where(t => !t.IsAbstract)
                .Select(t => (ConfigRule)Activator.CreateInstance(t));
    }
}
