using System.Linq;
using LiveOpsKit;
using UnityEngine;

// Shows a banner only while its event is live. Time travel moves it on and off.
public class EventBanner : MonoBehaviour
{
    [SerializeField] TextAsset config;
    [SerializeField] string eventId = "halloween_2026";
    [SerializeField] GameObject banner;

    LiveOpsEvent liveOpsEvent;

    void Start()
    {
        liveOpsEvent = LiveOpsConfig.FromJson(config.text).events.FirstOrDefault(e => e.id == eventId);
    }

    void Update()
    {
        banner.SetActive(liveOpsEvent != null && liveOpsEvent.IsLive);
    }
}
