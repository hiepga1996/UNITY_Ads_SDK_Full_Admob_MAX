using System.Collections.Generic;
using UnityEngine;
using Ultility;
using Unity.Notifications.Android;
using Firebase.Analytics;

public class BasicNotifications : MonoBehaviour
{
    public static BasicNotifications _instant;

    float timePlaying = 0;
    bool isPlaying = false;

    public static BasicNotifications Instant
    {
        get
        {
            if (_instant == null)
            {
                _instant = FindObjectOfType<BasicNotifications>();
            }

            return _instant;
        }
    }

    void Update()
    {
        if(isPlaying)
            timePlaying += Time.deltaTime;
    }

    private void Awake()
    {
        isPlaying = true;
        last_login_time();
        if (_instant != null && _instant.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            Destroy(this.gameObject);
        else
            _instant = this.GetComponent<BasicNotifications>();
        DontDestroyOnLoad(this.gameObject);

    }

    public void onTouchSendNativeNotification()
    {
        return;
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Debug.Log("permission granted!!");
        }
        else
        {
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");

            //if (common.request_permission_notify == 0)
            //    UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        // create new channel to show notification
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Chanel",
            Importance = Importance.Default,
            Description = "Generic notification",
        };

        List<string> lmsg = new List<string>()
        {
            "Hello my friend, you are getting free " + common.coin_reward_noti + " coins. Click to claim Reward"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Special Reward Just For You!";
        notification.Text = lmsg[0];

        /// Timed notification
        notification.FireTime = System.DateTime.Now.AddHours(0.05);

        // Send notification
        var identifier = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        {
            //MessageBox.instance.showMessage("SendNotification Scheduled");
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    public void session_start(string souce, string id)
    {
        FirebaseAnalytics.LogEvent("session_start",
            new Parameter("source:", souce),
            new Parameter("session_id:", id)
        );
    }

    public void session_end(float duration, string id)
    {
        FirebaseAnalytics.LogEvent("session_end",
            new Parameter("duration:", duration),
            new Parameter("session_id:", id)
        );
    }

    public void session_duration(float duration)
    {
        FirebaseAnalytics.LogEvent("session_duration",
            new Parameter("duration:", duration)
        );
    }

    public void last_login_time()
    { 
        var time = System.DateTime.Now;
        FirebaseAnalytics.LogEvent("last_login_time",
            new Parameter("duration:", time.ToString())
        );
    }

    public void in_app_purchase(string item_id, string item_name, int value, string currency)
    {
        FirebaseAnalytics.LogEvent("in_app_purchase",
           new Parameter("item_id:", item_id),
           new Parameter("item_name:", item_name),
           new Parameter("value:", value),
           new Parameter("currency:", currency)
       );
    }

    public void post_score(int score, int level, string mode)
    {
        FirebaseAnalytics.LogEvent("in_app_purchase",
           new Parameter("score:", score),
           new Parameter("level:", level),
           new Parameter("mode:", mode)
       );
    }

    private void OnApplicationQuit()
    {
        isPlaying = false;
        session_duration(timePlaying);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            session_end(timePlaying, "0000" + PlayerPrefs.GetInt("session_id:", 0));
        }
    }
}
