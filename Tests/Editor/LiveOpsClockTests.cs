using System;
using NUnit.Framework;

namespace LiveOpsKit.Tests
{
    public class LiveOpsClockTests
    {
        TimeSpan savedOffset;

        [SetUp]
        public void SetUp() => savedOffset = LiveOpsClock.Offset;

        [TearDown]
        public void TearDown() => LiveOpsClock.Offset = savedOffset;

        [Test]
        public void TravelBy_MovesGameTime()
        {
            LiveOpsClock.BackToRealTime();
            LiveOpsClock.TravelBy(TimeSpan.FromDays(2));

            var difference = LiveOpsClock.UtcNow - DateTime.UtcNow;
            Assert.AreEqual(2, difference.TotalDays, 0.001);
            Assert.IsTrue(LiveOpsClock.IsTimeTravelling);
        }

        [Test]
        public void TravelTo_LandsOnDate()
        {
            var target = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            LiveOpsClock.TravelTo(target);
            Assert.AreEqual(0, (LiveOpsClock.UtcNow - target).TotalSeconds, 1);
        }

        [Test]
        public void EventState_FollowsTime()
        {
            var evt = new LiveOpsEvent { startUtc = "2026-10-25T00:00:00Z", endUtc = "2026-11-01T00:00:00Z" };

            Assert.AreEqual(EventState.Upcoming, evt.StateAt(new DateTime(2026, 10, 24, 0, 0, 0, DateTimeKind.Utc)));
            Assert.AreEqual(EventState.Live, evt.StateAt(new DateTime(2026, 10, 28, 0, 0, 0, DateTimeKind.Utc)));
            Assert.AreEqual(EventState.Ended, evt.StateAt(new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc)));
        }
    }
}
