using System;
using System.Linq;
using LiveOpsKit.Editor;
using NUnit.Framework;
using UnityEngine;

namespace LiveOpsKit.Tests
{
    public class ConfigValidatorTests
    {
        static readonly DateTime Now = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        ConfigSettings settings;

        [SetUp]
        public void SetUp() => settings = ScriptableObject.CreateInstance<ConfigSettings>();

        [TearDown]
        public void TearDown() => UnityEngine.Object.DestroyImmediate(settings);

        static LiveOpsEvent Event(string id, string start, string end, string type = "tournament") =>
            new LiveOpsEvent { id = id, type = type, startUtc = start, endUtc = end };

        static LiveOpsConfig Config(params LiveOpsEvent[] events) =>
            new LiveOpsConfig { events = events.ToList() };

        string[] Messages(LiveOpsConfig config) =>
            ConfigValidator.Validate(config, settings, Now).Select(i => i.Message).ToArray();

        [Test]
        public void ValidConfig_HasNoIssues()
        {
            var config = Config(Event("halloween", "2026-10-25T00:00:00Z", "2026-11-01T00:00:00Z"));
            CollectionAssert.IsEmpty(Messages(config));
        }

        [Test]
        public void BrokenJson_IsReported()
        {
            var issues = ConfigValidator.Validate("{ \"events\": [", settings, Now, out var config);
            Assert.IsNull(config);
            StringAssert.Contains("not valid JSON", issues[0].Message);
        }

        [Test]
        public void DuplicateIds_AreErrors()
        {
            var config = Config(
                Event("weekly", "2026-10-25T00:00:00Z", "2026-11-01T00:00:00Z", "a"),
                Event("weekly", "2026-11-01T00:00:00Z", "2026-11-08T00:00:00Z", "b"));
            Assert.IsTrue(Messages(config).Any(m => m.Contains("used by 2 events")));
        }

        [Test]
        public void EndBeforeStart_IsError()
        {
            var config = Config(Event("x", "2026-11-01T00:00:00Z", "2026-10-30T00:00:00Z"));
            Assert.IsTrue(Messages(config).Any(m => m.Contains("ends before it starts")));
        }

        [Test]
        public void MissingTimeZone_IsWarning()
        {
            var config = Config(Event("x", "2026-10-25 00:00", "2026-11-01 00:00"));
            Assert.IsTrue(Messages(config).Any(m => m.Contains("no time zone")));
        }

        [Test]
        public void UnreadableDate_IsError()
        {
            var config = Config(Event("x", "31/31/2026", "2026-11-01T00:00:00Z"));
            Assert.IsTrue(Messages(config).Any(m => m.Contains("not a valid date")));
        }

        [Test]
        public void SameTypeOverlap_IsWarning()
        {
            var config = Config(
                Event("a", "2026-10-25T00:00:00Z", "2026-11-01T00:00:00Z"),
                Event("b", "2026-10-31T00:00:00Z", "2026-11-05T00:00:00Z"));
            Assert.IsTrue(Messages(config).Any(m => m.Contains("Overlaps 'a'")));
        }

        [Test]
        public void HugeReward_IsError()
        {
            var evt = Event("x", "2026-10-25T00:00:00Z", "2026-11-01T00:00:00Z");
            evt.rewards = new[] { new Reward { item = "coins", amount = 5000000 } };
            Assert.IsTrue(Messages(Config(evt)).Any(m => m.Contains("above the")));
        }

        [Test]
        public void MissingAsset_IsError()
        {
            var evt = Event("x", "2026-10-25T00:00:00Z", "2026-11-01T00:00:00Z");
            evt.assets = new[] { "Events/DoesNotExist" };
            Assert.IsTrue(Messages(Config(evt)).Any(m => m.Contains("not in the project")));
        }
    }
}
