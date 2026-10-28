using System;
using System.Collections.Generic;

namespace LiveOpsKit.Editor
{
    // Catches the classic extra zero in a reward amount.
    public class RewardRule : ConfigRule
    {
        public override IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            foreach (var evt in config.events)
            {
                if (evt.rewards == null) continue;

                foreach (var reward in evt.rewards)
                {
                    string label = Label(config, evt);
                    if (string.IsNullOrEmpty(reward.item))
                        yield return ConfigIssue.Error(label, "Has a reward with no item.");
                    else if (reward.amount <= 0)
                        yield return ConfigIssue.Error(label, $"Reward '{reward.item}' has amount {reward.amount}.");
                    else if (reward.amount > settings.maxRewardAmount)
                        yield return ConfigIssue.Error(label, $"Reward '{reward.item}' gives {reward.amount}, above the {settings.maxRewardAmount} limit.");
                }
            }
        }
    }
}
