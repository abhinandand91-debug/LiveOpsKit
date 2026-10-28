using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveOpsKit
{
    public enum EventState { Invalid, Upcoming, Live, Ended }

    // The LiveOps config the server sends: a list of timed events.
    [Serializable]
    public class LiveOpsConfig
    {
        public int version;
        public List<LiveOpsEvent> events = new List<LiveOpsEvent>();

        public static LiveOpsConfig FromJson(string json) => JsonUtility.FromJson<LiveOpsConfig>(json);
    }

    [Serializable]
    public class LiveOpsEvent
    {
        public string id;
        public string type;
        public string startUtc;
        public string endUtc;
        public string[] assets = new string[0];
        public Reward[] rewards = new Reward[0];

        public bool TryGetWindow(out DateTime start, out DateTime end)
        {
            bool hasStart = UtcDate.TryParse(startUtc, out start);
            bool hasEnd = UtcDate.TryParse(endUtc, out end);
            return hasStart && hasEnd;
        }

        public EventState StateAt(DateTime utc)
        {
            if (!TryGetWindow(out var start, out var end) || end <= start) return EventState.Invalid;
            if (utc < start) return EventState.Upcoming;
            return utc < end ? EventState.Live : EventState.Ended;
        }

        public bool IsLive => StateAt(LiveOpsClock.UtcNow) == EventState.Live;
    }

    [Serializable]
    public class Reward
    {
        public string item;
        public int amount;
    }
}
