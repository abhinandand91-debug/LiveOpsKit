using System;
using UnityEngine;

namespace LiveOpsKit
{
    // Game time for LiveOps. Read LiveOpsClock.UtcNow instead of DateTime.UtcNow so testers can time travel.
    public static class LiveOpsClock
    {
        const string OffsetKey = "LiveOpsKit.ClockOffsetTicks";
        static long? offsetTicks;

        public static DateTime UtcNow => DateTime.UtcNow + Offset;

        // Time travel only works in the editor and development builds. Release builds always use real time.
        public static bool CanTimeTravel => Application.isEditor || Debug.isDebugBuild;

        public static bool IsTimeTravelling => Offset != TimeSpan.Zero;

        public static TimeSpan Offset
        {
            get
            {
                if (!CanTimeTravel) return TimeSpan.Zero;
                offsetTicks ??= long.TryParse(PlayerPrefs.GetString(OffsetKey, "0"), out long ticks) ? ticks : 0;
                return TimeSpan.FromTicks(offsetTicks.Value);
            }
            set
            {
                if (!CanTimeTravel) return;
                offsetTicks = value.Ticks;
                PlayerPrefs.SetString(OffsetKey, value.Ticks.ToString());
                PlayerPrefs.Save();
            }
        }

        public static void TravelBy(TimeSpan amount) => Offset += amount;

        public static void TravelTo(DateTime utc) => Offset = utc.ToUniversalTime() - DateTime.UtcNow;

        public static void BackToRealTime() => Offset = TimeSpan.Zero;
    }
}
