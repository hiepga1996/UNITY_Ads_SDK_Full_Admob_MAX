using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using ConfigValue = Firebase.RemoteConfig.ConfigValue;
using Firebase.RemoteConfig;
using System.Linq;
using System.Text.RegularExpressions;
using Ultility;
using UnityEngine.SocialPlatforms.Impl;

#if NOT_ADJUST
#else
#endif

namespace DVAH
{
    public class FireBaseManager : Singleton<FireBaseManager>, IChildLib
    {

        [SerializeField]
        private string  _adValue, _adjsutLevelAchived;

#if UNITY_EDITOR
        public string ADValue
        {
            get
            {
                return _adValue;
            }

            set
            {
                _adValue = value;
            }
        }

        public string Level_Achived
        {
            get
            {
                return _adjsutLevelAchived;
            }

            set
            {
                _adjsutLevelAchived = value;
            }
        }

       
#endif

        #region For AD event

        AD_TYPE _adTypeLoaded = AD_TYPE.open;
        [HideInInspector]
        public AD_TYPE adTypeShow = AD_TYPE.resume;

#endregion

        private bool _isFetchDone = false;

        public bool isFetchDOne => _isFetchDone;

        [SerializeField]
        private List<string> _keyConfigs = new List<string>();

        public void Init(Action _onActionDone)
        {
            Debug.Log("[3rdLib]==========> Firebase start Init! <==========");

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    InitializeFirebase();
                    //FirebaseMessaging.DeleteTokenAsync();
                    //FirebaseMessaging.GetTokenAsync();

                    //Example Topics
                    //FirebaseMessaging.SubscribeAsync("/topics/reward_noti");

                    _onActionDone?.Invoke();

                    common.request_permission_notify = PlayerPrefs.GetInt(common.REQUEST_PERMISSION_NOTIFY, 0);
                    try
                    {
                        if (common.request_permission_notify == 0)
                        {
                            askRequst();
                            common.request_permission_notify = 1;
                            PlayerPrefs.GetInt(common.REQUEST_PERMISSION_NOTIFY, 1);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("init_permisson_fail");
                    }

                    try
                    {
                        InitMessage();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("init_message_fail");
                    }

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "[3rdLib]==> Could not resolve all Firebase dependencies: {0} <==", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
 
                    _onActionDone?.Invoke();
                }
            });
        }

        void InitMessage()
        {
            if (UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
        }

        void askRequst()//call this function to ask request
        {
            if (UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                print("permission granted!!");
            }
            else
            {
                //UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");

                var callbacks = new UnityEngine.Android.PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", callbacks);
            }
        }
        internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
        }
        internal void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
        }
        internal void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }

        void InitializeFirebase()
        {
            System.Collections.Generic.Dictionary<string, object> defaults =
                new System.Collections.Generic.Dictionary<string, object>();
            FetchFireBase();
        }

        public void FetchFireBase()
        {
            FetchDataAsync();
        }

        /// <summary>
        /// Wait to get a value from Firebase remote config
        /// </summary>
        /// <param name="key">key name on Firebase remote</param>
        /// <param name="waitOnDone">callback when get data success</param> 
        public async Task GetValueRemoteAsync(string key, Action<ConfigValue> waitOnDone) 
        {
     
            double countTime = 0;
            while (!_isFetchDone && countTime < 360000f)
            {
                countTime += 1000;
                await Task.Delay(1000);
            }

            if (countTime >= 360000f)
            {
                Debug.LogWarning(string.Format("[3rdLib]==>Fetch data {0} fail, becuz time out! Check your network please!<==", key));
                return;
            }

            if (!_keyConfigs.Contains(key))
            {
                Debug.LogWarning(string.Format("[3rdLib]==>Remote dont have key {0} !<==", key));
                return;
            }

            var obj = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            waitOnDone?.Invoke(obj);
        }

        // Start a fetch request.
        public Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");
            // FetchAsync only fetches new data if the current data is older than the provided
            // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
            // By default the timespan is 12 hours, and for production apps, this is a good
            // number.  For this example though, it's set to a timespan of zero, so that
            // changes in the console will always show up immediately.
            System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            return fetchTask.ContinueWith(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("[3rdLib]==> Fetch canceled. <==");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("[3rdLib]==> Fetch encountered an error <==");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("[3rdLib]==> Fetch completed successfully! <==");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    _isFetchDone = true;

                    _keyConfigs = FirebaseRemoteConfig.DefaultInstance.AllValues.Keys.ToList();
                    Debug.Log(String.Format("[3rdLib]==> Remote data loaded and ready (last fetch time {0}).<==",
                        info.FetchTime));

                    _ = GetValueRemoteAsync("ads_aoa_show", (value) =>
                    {
                        common.ads_aoa_show = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_spin_on_off", (value) =>
                    {
                        common.native_spin_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_level_start_on_off", (value) =>
                    {
                        common.native_level_start_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_level_select_on_off", (value) =>
                    {
                        common.native_level_select_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_shop_on_off", (value) =>
                    {
                        common.native_shop_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_game_play_on_off", (value) =>
                    {
                        common.native_game_play_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("mrec_coin_plus_on_off", (value) =>
                    {
                        common.mrec_coin_plus_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("mrec_challenge_on_off", (value) =>
                    {
                        common.mrec_challenge_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("mrec_game_over_on_off", (value) =>
                    {
                        common.mrec_game_over_on_off = value.BooleanValue;
                    });


                    _ = GetValueRemoteAsync("mrec_loading_on_off", (value) =>
                    {
                        common.mrec_loading_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("mrec_coin_effect_on_off", (value) =>
                    {
                        common.mrec_coin_effect_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("Mrec_Setting_On_Off", (value) =>
                    {
                        common.Mrec_Setting_On_Off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("Mrec_Dayly_On_Off", (value) =>
                    {
                        common.Mrec_Dayly_On_Off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("Mrec_RewardItem_On_Off", (value) =>
                    {
                        common.Mrec_RewardItem_On_Off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("banner_on_off", (value) =>
                    {
                        common.banner_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("aoa_admob_on_off", (value) =>
                    {
                        common.aoa_admob_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("resume_ads_on_off", (value) =>
                    {
                        common.resume_ads_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("switch_banner_when_false", (value) =>
                    {
                        common.switch_banner_when_false = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("native_on_off", (value) =>
                    {
                        common.native_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("coin_reward", (value) =>
                    {
                        common.coin_reward = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("multiple_reward_win", (value) =>
                    {
                        common.multiple_reward_win = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("coin_reward_win", (value) =>
                    {
                        common.coin_reward_win = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("win_game_coin_reward", (value) =>
                    {
                        common.win_game_coin_reward = (int)value.LongValue;
                    });

                    //_ = GetValueRemoteAsync("admob_native_ids_ads", (value) =>
                    //{
                    //    common.admob_native_ids_ads = value.StringValue;
                    //    Advertisements.Instance.GetIdsAds();
                    //});

                    //_ = GetValueRemoteAsync("admob_banner_ids_ads", (value) =>
                    //{
                    //    common.admob_banner_ids_ads = value.StringValue;
                    //    Advertisements.Instance.GetIdsAds();
                    //});

                    _ = GetValueRemoteAsync("link_game", (value) =>
                    {
                        common.link_game = value.StringValue;
                    });

                    _ = GetValueRemoteAsync("icon_game", (value) =>
                    {
                        common.icon_game = value.StringValue;
                        Advertisements.Instance.GetIdsAds();
                    });

                    _ = GetValueRemoteAsync("ads_aoa_open_count", (value) =>
                    {
                        common.ads_aoa_open_count = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("banner_admob_on_off", (value) =>
                    {
                        common.banner_admob_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("banner_adaptive_on_off", (value) =>
                    {
                        common.banner_adaptive_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("limit_level_tree_show", (value) =>
                    {
                        common.limit_level_tree_show = value.BooleanValue;
                    });
                    _ = GetValueRemoteAsync("time_capping_interstital_ingame", (value) =>
                    {
                        common.timeCappingInterstitalInGame = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("booster_style_config", (value) =>
                    {
                        common.styleBooster = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("score_limit_fix", (value) =>
                    {
                        common.score_limit_fix = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("level_limit_fix", (value) =>
                    {
                        common.level_limit_fix = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("time_loading_native", (value) =>
                    {
                        common.time_loading_native = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("first_time_spin", (value) =>
                    {
                        common.first_time_spin = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("ad_break_on_off", (value) =>
                    {
                        common.ad_break_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("ad_break_capping_time", (value) =>
                    {
                        common.ad_break_capping_time = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("timeCountDown", (value) =>
                    {
                        common.timeCountDown = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("percent_on_off", (value) =>
                    {
                        common.percent_on_off = value.BooleanValue;
                    });

                    _ = GetValueRemoteAsync("percent_number", (value) =>
                    {
                        common.percent_number = float.Parse(value.StringValue);
                        //Debug.LogError(common.percent_number);
                    });

                    _ = GetValueRemoteAsync("ads_inter_type", (value) =>
                    {
                        common.ads_inter_type = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("ads_banner_type", (value) =>
                    {
                        common.ads_banner_type = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("banner_show_level", (value) =>
                    {
                        common.banner_show_level = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("AOA_show_level", (value) =>
                    {
                        common.AOA_show_level = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("mrec_show_level", (value) =>
                    {
                        common.mrec_show_level = (int)value.LongValue;
                    });

                    _ = GetValueRemoteAsync("ads_reward_type", (value) =>
                    {
                            common.ads_reward_type = (int)value.LongValue;
                    });

                    //Debug.LogError("Loaded_config");
                    Advertisements.Instance.isInitFirebaseDone = true;
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    Advertisements.Instance.isInitFirebaseDone = true;
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("[3rdLib]==> Fetch failed for unknown reason <==");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("[3rdLib]==> Fetch throttled until " + info.ThrottledEndTime+" <==");
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("[3rdLib]==> Latest Fetch call still pending. <==");
                    break;
            }
        }

#region Firebase Logevent

        public string Checker(string str)
        {
            str = str.Replace(" ", "_");
            return Regex.Replace(str, "[^0-9A-Za-z_+-]", "");
        }

        public void LogEventWithOneParam(string eventName)
        {
            Debug.Log("[3rdLib]==> LogEvent " + eventName+" <==");
            _= this.LogEventWithParameter(eventName, new Hashtable() { { "value", 1 } });

        }

        /// <summary>
        /// Wait to log event to firebase analytics!
        /// </summary>
        /// <param name="event_name">name of event</param>
        /// <param name="hash">A hash table which contain value and parameter</param> 
        public async Task LogEventWithParameter(string event_name, Hashtable hash)
        {
            double countTime = 0;
            while (!_isFetchDone && countTime < 360000f)
            {
                countTime += 1000;
                await Task.Delay(1000);
            }

            if (countTime >= 360000f)
            {
                Debug.LogError(string.Format("[3rdLib]==>Logevent {0} fail, becuz time out! Check your network please!<==", event_name));
                return;
            }

            Firebase.Analytics.Parameter[] parameter = new Firebase.Analytics.Parameter[hash.Count];
            //List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
            if (hash != null && hash.Count > 0)
            {
                int i = 0;
                foreach (DictionaryEntry item in hash)
                {
                    if (item.Equals((DictionaryEntry)default)) continue;
                    string key = this.Checker(item.Key.ToString());
                    string value = this.Checker(item.Value.ToString());

                    parameter[i] = (new Firebase.Analytics.Parameter(key, value));
                    Debug.Log("[3rdLib]==> LogEvent " + event_name.ToString() + "- Key = " + key + " -  Value =" + value + " <==");
                    i++;
                }

                Firebase.Analytics.FirebaseAnalytics.LogEvent(
                           event_name,
                           parameter);
            }
        }


#endregion

#region FIREBASE CUSTOM EVENT

        /// <summary>
        ///   state: Trạng thái của level sau khi người chơi chơi qua
        /// </summary>
        public void LogEventLevel(int level, LEVEL_STATE_EVENT state)
        {
            _= this.LogEventWithParameter(state.ToString(), new Hashtable()
            {
                {"id_level", level}
            });

            if (string.IsNullOrEmpty(_adjsutLevelAchived))
                return;
#if NOT_ADJUST
#else
#endif
        }

        ///<param name="name">tên button</param>
        ///<param name="screen">vị trí màn hình của user</param>
        ///<param name="level">level hiện tại (nếu trong gameplay) hoặc đã pass (nếu ngoài gameplay) của user</param>
        ///<param name="customParam">Hậu tố nếu cần thêm</param>
        /// <summary>
        /// Log event when user click a button
        /// <code>
        ///  name: "tên button"
        ///  screen: "vị trí màn hình của user",
        ///  level: "level hiện tại (nếu trong gameplay) hoặc đã pass (nếu ngoài gameplay) của user"
        ///  customParam: "Hậu tố nếu cần thêm"
        /// </code>
        /// <example>
        /// For example:
        /// <code>
        ///     - add_new_melee_gameplay_4 : user thêm unit melee trong gameplay tại level 4
        ///     - claim_x2_speed_gameplay_15 : user click x2 speed trong gameplay tại level 15
        /// </code> 
        /// </example>
        /// </summary>
        public void LogClickBtnEvent(string name, string screen, int level = -1, string customParam = "")
        {
            _ = this.LogEventWithParameter("btn_click", new Hashtable()
            {
                {"id_click",string.Format("{0}_{1}",name, screen) + (level < 0 ?"":"_"+level) + (string.IsNullOrWhiteSpace(customParam)?"":"_"+customParam )}
            });
        }


        ///<param name="name">tên button</param>
        ///<param name="screen">vị trí màn hình của user</param>
        ///<param name="level">level hiện tại (nếu trong gameplay) hoặc đã pass (nếu ngoài gameplay) của user</param>
        ///<param name="customParam">Hậu tố nếu cần thêm</param>
        /// <summary>
        /// Log event when user click button AD
        /// <code>
        ///  name: "tên button"
        ///  screen: "vị trí màn hình của user",
        ///  level: "level hiện tại (nếu trong gameplay) hoặc đã pass (nếu ngoài gameplay) của user"
        ///  customParam: "Hậu tố nếu cần thêm"
        /// </code>
        /// <example>
        /// For example:
        /// <code>
        ///     - add_new_melee_gameplay_4 : user xem ad reward thêm unit melee trong gameplay tại level 4
        ///     - claim_x2_speed_gameplay_15 : user xem ad reward x2 speed trong gameplay tại level 15
        /// </code> 
        /// </example>
        /// </summary>
        public void LogClickRewardBtnEvent(string name, string screen, int level= -1, string customParam = "")
        {
            _ = this.LogEventWithParameter("reward_ad_on_click", new Hashtable()
            {
                 {"id_click",string.Format("{0}_{1}",name, screen) + (level < 0 ?"":"_"+level) + (string.IsNullOrWhiteSpace(customParam)?"":"_"+customParam )}
            });
        }

        public void LogADEvent(AD_TYPE adType, AD_STATE adState, string adNetwork = "")
        {
            _ = this.LogEventWithParameter("ad_event", new Hashtable()
            {
                 {string.Format("ad_{0}_load_stats", adNetwork),string.Format( "ad_{0}_{1}",adType.ToString(), adState.ToString() )}
            });
        }


        public void LogADResumeEvent(AD_STATE adState, string adNetwork = "")
        {
            AD_TYPE adType = this._adTypeLoaded;
              
            if (adState == AD_STATE.show)
            {
                if (adTypeShow == AD_TYPE.open)
                    adState = AD_STATE.show_open;
                else
                    adState = AD_STATE.show_resume;


                this._adTypeLoaded = AD_TYPE.resume;
                adTypeShow = AD_TYPE.resume;
            }

            if (adState == AD_STATE.show_fail)
            {
                if (adTypeShow == AD_TYPE.open)
                    adState = AD_STATE.show_open_fail;
                else
                    adState = AD_STATE.show_resume_fail;

                this._adTypeLoaded = AD_TYPE.resume;
                adTypeShow = AD_TYPE.resume;
            }

            LogADEvent(adType,adState,adNetwork); 
        }


        public void LogEventClickAds(AD_TYPE ad_type, string adNetwork)
        {
            _ = this.LogEventWithParameter("ad_event", new Hashtable()
            {
                 {string.Format("ad_{0}_load_stats", adNetwork),string.Format( "ad_{0}_click", ad_type.ToString() )}
            });
        }

        public void LogAdValueAdjust(double value)
        {
            if (string.IsNullOrEmpty(_adValue))
                return;
#if NOT_ADJUST
#else
        
#endif
        }

#endregion

    }

    [Serializable]
    struct LevelGroup : IEnumerable<Level>
    {
        public List<Level> Levels;

        public IEnumerator<Level> GetEnumerator()
        {
            return Levels?.GetEnumerator() ?? Enumerable.Empty<Level>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    struct Level
    {
        //public string name;
        public string level;
    }

    [Serializable]
    struct ScoreGroup : IEnumerable<Score>
    {
        public List<Score> Score;

        public IEnumerator<Score> GetEnumerator()
        {
            return Score?.GetEnumerator() ?? Enumerable.Empty<Score>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    struct Score
    {
        //public string name;
        public string star1;
        public string star2;
        public string star3;
    }
}


public enum LEVEL_STATE_EVENT
{
    start_level,
    fail_level,
    win_level
}

public enum AD_TYPE
{
    open,
    resume,
    banner,
    inter,
    reward,
    mrec,
    native
}

public enum AD_STATE
{
    load,
    load_done,
    load_fail,
    show,
    show_fail,
    show_open,
    show_open_fail,
    show_resume,
    show_resume_fail,
}