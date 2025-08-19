using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MaxSdkBase;
using System.Threading.Tasks;
using Ultility;

namespace DVAH
{
    public enum InterVideoState
    {
        None,
        Open,
        Closed
    }
    public enum RewardVideoState
    {
        None,
        Open,
        Watched
    }

    public class AdManager : Singleton<AdManager>, IChildLib
    {
        #region Lib Properties

        [SerializeField]
        string _paid_ad_revenue = "paid_ad_impression_value";
#if UNITY_EDITOR
        public string paid_ad_revenue
        {
            get
            {
                return _paid_ad_revenue;
            }

            set
            {
                _paid_ad_revenue = value;
            }
        }
#endif
        [SerializeField]
        bool _isBannerAutoShow = false;

        #region control AD is Allow
        bool _isBannerCurrentlyAllow = false, _isMrecCurrentlyAllow = false,
            _isOffReward = false, _isOffAdsOpen = false, _isOffAdsResume = false;
        public bool _isOffBanner = false, _isOffMrec = false, _isOffInter = false;
        public bool isAdBanner => _isBannerCurrentlyAllow;
        #endregion

        [SerializeField]
        MaxSdkBase.BannerPosition _bannerPosition = MaxSdkBase.BannerPosition.BottomCenter;
        public MaxSdkBase.BannerPosition BannerPosition => _bannerPosition;

        [SerializeField] GameObject _popUpNoAd;


        [SerializeField] bool isShowMaxBanner = false;

        [Header("---ID---")]
        [Space(10)]
        [SerializeField]
        private string _MaxSdkKey = "6MU1WWsSf2_Nio5yuc7BZvr_Xh3KZ2PORnhzNSz8Efx22DBnKrzTkgKlVqU6sES_xBkFMEDvClCfzgKEAuIczP";

        [SerializeField]
        private string _BannerAdUnitID = "ca1c493fb05425c9",
            _MrecAdUnitID = "ca1c493fb05425c9",
            _InterstitialAdUnitID = "598cac6b4b9f2c4b",
            _RewardedAdUnitID = "7603fe4fa72df9bf";

        [SerializeField]  List<string> _OpenAdUnitIDs = new List<string>();


#if UNITY_EDITOR
        public string MaxSdkKey
        {
            get
            {
                return _MaxSdkKey;
            }
            set
            {
                _MaxSdkKey = value;
            }
        }
        public string BannerAdUnitID
        {
            get
            {
                return _BannerAdUnitID;
            }
            set
            {
                _BannerAdUnitID = value;
            }
        }

        public string InterstitialAdUnitID
        {
            get
            {
                return _InterstitialAdUnitID;
            }
            set
            {
                _InterstitialAdUnitID = value;
            }
        }

        public string RewardedAdUnitID
        {
            get
            {
                return _RewardedAdUnitID;
            }
            set
            {
                _RewardedAdUnitID = value;
            }
        }

        public List<string> OpenAdUnitIDs
        {
            get
            {
                return _OpenAdUnitIDs;
            }
            set
            {
                _OpenAdUnitIDs = value;
            }
        }

#endif

        private int bannerRetryAttempt,
            mrecRetryAttempt,
            interstitialRetryAttempt,
            rewardedRetryAttempt;

        List<int> AdOpenRetryAttemp = new List<int>();

        #region CallBack
        private Action<InterVideoState> _callbackInter = null;
        private Action<RewardVideoState> _callbackReward = null;
        public Action<bool> _callbackOpenAD = null;

        private Action _bannerClickCallback = null;
        private Action _mRecClickCallback = null;
        private Action _interClickCallback = null;
        private Action _rewardClickCallback = null;
        private Action _adOpenClickCallback = null;

        #endregion




        private bool isShowingAd = false, _isSDKMaxInitDone = false, _isSDKAdMobInitDone = false, _isBannerInitDone = false, _isMrecInitDone = false;

        #endregion


        #region CUSTOM PROPERTIES
        #endregion


        #region Lib Method

        public void Init(Action _onInitDone = null)
        {
            Debug.Log("[3rdLib]==========> Ad start Init! <==========");

            InitMAX();
            Advertisements.Instance.initAdmod();
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

            _onInitDone?.Invoke();
        }


#region ClickCallBack
        public AdManager AssignClickCallBackBanner(Action callback)
        {
            _bannerClickCallback = callback;
            return this;
        }

        public AdManager AssignClickCallBackInter(Action callback)
        {
            _interClickCallback = callback;
            return this;
        }

        public AdManager AssignClickCallBackReward(Action callback)
        {
            _rewardClickCallback = callback;
            return this;
        }

        public AdManager AssignClickCallBackAdOpne(Action callback)
        {
            _adOpenClickCallback = callback;
            return this;
        }
#endregion

        void InitMAX()
        {
            var checkOff = PlayerPrefs.GetInt(common.REMOVE_ADS_KEY, 0);
            if (checkOff != 0)
            {
                _isOffBanner = true;
                _isOffInter = true;
                _isOffMrec = true;

                Advertisements.Instance.is_offBanner = true;
                Advertisements.Instance.is_offInter = true;
                Advertisements.Instance.is_offMrec = true;
                Advertisements.Instance.is_offNative = true;
            }

            AdOpenRetryAttemp = new List<int>(new int[_OpenAdUnitIDs.Count]);

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("[3rdLib]==> MAX SDK Initialized <==");
                _isSDKMaxInitDone = true;
                InitAdOpen();
                if (!_isOffInter)
                    InitializeInterstitialAds();

                if(!_isOffMrec)
                    _ = InitializeMrecAds();

                if (!_isOffReward)
                    InitializeRewardedAds();

                if(!_isOffBanner) 
                    _ = InitializeBannerAds();

            };
            MaxSdk.SetSdkKey(_MaxSdkKey);
            MaxSdk.InitializeSdk();
        }

        public bool initMaxDone()
        {
            return _isSDKMaxInitDone;
        }

#region Banner Ad Methods

        public async Task InitializeBannerAds()
        {
            if (common.banner_on_off == false)
                return;

            while (!_isSDKMaxInitDone)
            {
                Debug.LogWarning("[3rdLib]==>Waiting Max SDK init done!<==");
                await Task.Delay(500);
            }

            UnityMainThread.wkr.AddJob(() =>
            {
                if (_isOffBanner)
                    return;
                if (string.IsNullOrWhiteSpace(_BannerAdUnitID))
                    return;
                Debug.Log("[3rdLib]==> Init banner <==");
                FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.banner, adState: AD_STATE.load);
                // Attach Callbacks
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
                MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
                {
                    Debug.Log("[3rdLib]==> Banner ad revenue paid <==");
                    TrackAdRevenue(adInfo);
                };

                // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
                // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
                MaxSdk.CreateBanner(_BannerAdUnitID, _bannerPosition);
                MaxSdk.SetBannerExtraParameter(_BannerAdUnitID, "adaptive_banner", "false");
                // Set background or background color for banners to be fully functional.
                MaxSdk.SetBannerBackgroundColor(_BannerAdUnitID, new Color(1, 1, 1, 0));
                if (_isBannerAutoShow) _ = ShowBanner();

                _isBannerInitDone = true;
            });
        }

        void ManuallyLoadBanner()
        {
            MaxSdk.StopBannerAutoRefresh(_BannerAdUnitID);
            MaxSdk.LoadBanner(_BannerAdUnitID);
        }

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Banner ad loaded " + adUnitId+" <==");
            MaxSdk.StartBannerAutoRefresh(_BannerAdUnitID);

            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.banner, adState: AD_STATE.load_done, adNetwork: adInfo.NetworkName);
        }

        private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Banner ad failed to load. MAX will automatically try loading a new ad internally.
            Debug.LogError("[3rdLib]==>Banner ad failed to load with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.banner, adState: AD_STATE.load_fail);
            bannerRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));

            //Invoke("ManuallyLoadBanner", (float)retryDelay);
            if(common.switch_banner_when_false == true)
            {
                if (common.ads_banner_type == 3)
                    return;

                Advertisements.Instance.offBannerAdmob = false;
                _isOffBanner = true;
                Advertisements.Instance.ShowBanner();
            }
            else
                Invoke("ManuallyLoadBanner", (float)retryDelay);

        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Banner ad clicked <==");
            FireBaseManager.Instant.LogEventClickAds(ad_type: AD_TYPE.banner, adNetwork: adInfo.NetworkName);
            try
            {
                _bannerClickCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> invoke banner click callback error: " + e.ToString() + " <==");
            }
            _bannerClickCallback = null;
        }


#endregion

#region Mrec Ad Methods

        public async Task InitializeMrecAds()
        {
            while (!_isSDKMaxInitDone)
            {
                Debug.LogWarning("[3rdLib]==>Waiting Max SDK init done!<==");
                await Task.Delay(500);
            }

            UnityMainThread.wkr.AddJob(() =>
            {
                if (_isOffMrec)
                    return;
                if (string.IsNullOrWhiteSpace(_MrecAdUnitID))
                    return;
                Debug.Log("[3rdLib]==> Init Mrec <==");
                FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.mrec, adState: AD_STATE.load);
                // Attach Callbacks
                MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMrecAdLoadedEvent;
                MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMrecAdFailedEvent;
                MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMrecAdClickedEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
                {
                    Debug.Log("[3rdLib]==> Mrec ad revenue paid <==");
                    TrackAdRevenue(adInfo);
                };

                // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
                // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
                MaxSdk.CreateMRec(_MrecAdUnitID, AdViewPosition.TopCenter);
                MaxSdk.SetMRecExtraParameter(_MrecAdUnitID, "adaptive_mrec", "false");

                _isMrecInitDone = true;
            });
        }

        void ManuallyLoadMrec()
        {
            MaxSdk.StopMRecAutoRefresh(_MrecAdUnitID);
            MaxSdk.LoadMRec(_MrecAdUnitID);
        }

        private void OnMrecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (_isMrecCurrentlyAllow)
                _ = this.ShowMrec();
            Debug.Log("[3rdLib]==> Mrec ad loaded " + adUnitId + " <==");
            MaxSdk.StartMRecAutoRefresh(_MrecAdUnitID);

            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.mrec, adState: AD_STATE.load_done, adNetwork: adInfo.NetworkName);

        }

        private void OnMrecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Banner ad failed to load. MAX will automatically try loading a new ad internally.
            Debug.LogError("[3rdLib]==>Mrec ad failed to load with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.mrec, adState: AD_STATE.load_fail);
            mrecRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, mrecRetryAttempt));

            Invoke("ManuallyLoadMrec", (float)retryDelay);
        }

        private void OnMrecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Mrec ad clicked <==");
            FireBaseManager.Instant.LogEventClickAds(ad_type: AD_TYPE.mrec, adNetwork: adInfo.NetworkName);
            try
            {
                _mRecClickCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> invoke Mrec click callback error: " + e.ToString() + " <==");
            }
            _mRecClickCallback = null;
        }


#endregion

#region Interstitial Ad Methods
        private void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += Interstitial_OnAdDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += Interstitial_OnAdClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
            {
                Debug.Log("[3rdLib]==> Interstitial revenue paid <==");
                TrackAdRevenue(adInfo);
            };

            // Load the first interstitial
            LoadInterstitial();
        }

        void LoadInterstitial()
        {
            Debug.Log("[3rdLib]==>Start load Interstitial " + _InterstitialAdUnitID+" <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.inter, adState: AD_STATE.load);
            MaxSdk.LoadInterstitial(_InterstitialAdUnitID);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("[3rdLib]==> Interstitial loaded <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.inter, adState: AD_STATE.load_done, adNetwork: adInfo.NetworkName);
            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Debug.LogError("[3rdLib]==> Interstitial failed to load with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.inter, adState: AD_STATE.load_fail);

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void Interstitial_OnAdDisplayedEvent(string arg1, AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Interstitial show! <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.inter, adState: AD_STATE.show, adNetwork: adInfo.NetworkName);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.LogError("[3rdLib]==> Interstitial failed to display with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.inter, adState: AD_STATE.show_fail, adNetwork: adInfo.NetworkName);
            LoadInterstitial();

            try
            {
                _callbackInter?.Invoke(InterVideoState.None);
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke callback inter, error: " + e.ToString() + " <==");
            }

            _callbackInter = null;
        }


        private void Interstitial_OnAdClickedEvent(string arg1, AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Inter ad clicked <==");
            FireBaseManager.Instant.LogEventClickAds(ad_type: AD_TYPE.inter, adNetwork: adInfo.NetworkName);
            try
            {
                _interClickCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke click inter callback, error: " + e.ToString() + " <==");
            }
            _interClickCallback = null;
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Interstitial dismissed <==");
            try
            {
                _callbackInter?.Invoke(InterVideoState.None);
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke callback inter, error: " + e.ToString() + " <==");
            }
            _callbackInter = null;
            LoadInterstitial();
            isShowingAd = false;
        }


#endregion

#region Reward Ad Methods
        private void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (adUnitId, adInfo) =>
            {
                Debug.Log("[3rdLib]==> Reward paid event! <==");
                TrackAdRevenue(adInfo);
            };


            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            Debug.Log("[3rdLib]==> Load reward Ad " + _RewardedAdUnitID+" ! <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.reward, adState: AD_STATE.load);
            MaxSdk.LoadRewardedAd(_RewardedAdUnitID);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'

            // Reset retry attempt
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.reward, adState: AD_STATE.load_done, adNetwork: adInfo.NetworkName);
            rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).

            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

            Debug.LogError("[3rdLib]==> Rewarded ad failed to load with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.reward, adState: AD_STATE.load_fail);
            Invoke("LoadRewardedAd", (float)retryDelay);

        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad

            Debug.LogError("[3rdLib]==> Rewarded ad failed to display with error code: " + errorInfo.Code + " <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.reward, adState: AD_STATE.show_fail, adInfo.NetworkName);
            LoadRewardedAd();
            try
            {
                _callbackReward?.Invoke(RewardVideoState.None);

            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke callback reward, error: " + e.ToString() + " <==");
            }

            _callbackReward = null;
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Reward display success! <==");
            FireBaseManager.Instant.LogADEvent(adType: AD_TYPE.reward, adState: AD_STATE.show, adInfo.NetworkName);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Reward clicked! <==");
            FireBaseManager.Instant.LogEventClickAds(ad_type: AD_TYPE.reward, adNetwork: adInfo.NetworkName);
            try
            {
                _rewardClickCallback?.Invoke();

            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke reward click callback, error: " + e.ToString() + " <==");
            }

            _rewardClickCallback = null;
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            LoadRewardedAd();
            isShowingAd = false;
            _callbackReward = null;
            Debug.Log("[3rdLib]==> Reward closed! <==");
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {

            try
            {
                _callbackReward?.Invoke(RewardVideoState.Watched);

            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==> Faild invoke callback reward, error: " + e.ToString() + " <==");
            }

            _callbackReward = null;
            Debug.Log("[3rdLib]==> Reward recived!! <==");
        }
#endregion


#region AdOpen Methods
        void InitAdOpen()
        {
            Debug.Log("[3rdLib]==> Ad open/resume init! <==");


            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += AppOpen_OnAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += AppOpenOnAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += AppOpen_OnAdDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += AppOpen_OnAdDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += AppOpen_OnAdClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
            {
                Debug.Log("[3rdLib]==> Ad open/resume paid event! <==");
                TrackAdRevenue(adInfo);
            };

            LoadAdOpen(0);
        }

        IEnumerator waitLoadAdOpen(float time, int ID)
        {
            yield return new WaitForSeconds(time);
            LoadAdOpen(ID);
        }

        public void LoadAdOpen(int ID = 0)
        {
            Debug.Log("[3rdLib]==> Start load ad open/resume! ID:" + _OpenAdUnitIDs[ID] + " <==");

            FireBaseManager.Instant.LogADResumeEvent(adState: AD_STATE.load);

            if (!MaxSdk.IsAppOpenAdReady(_OpenAdUnitIDs[ID]))
            {
                MaxSdk.LoadAppOpenAd(_OpenAdUnitIDs[ID]);
            }
        }


        private void AppOpen_OnAdLoadedEvent(string arg1, AdInfo arg2)
        {
            Debug.Log("[3rdLib]==>Load ad open/resume success! <==");

            FireBaseManager.Instant.LogADResumeEvent(adState: AD_STATE.load_done, adNetwork: arg2.NetworkName);

            int ID = _OpenAdUnitIDs.IndexOf(arg1);
            if (ID < 0)
                return;
            AdOpenRetryAttemp[ID] = 0;
        }

        private void AppOpen_OnAdDisplayedEvent(string arg1, AdInfo arg2)
        {
            Debug.Log("[3rdLib]==> Show ad open/resume success! <==");
            FireBaseManager.Instant.LogADResumeEvent(adState: AD_STATE.show, adNetwork: arg2.NetworkName);
            isShowingAd = true;

        }

        private void AppOpen_OnAdClickedEvent(string arg1, AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==>Click open/resume success! <==");
            FireBaseManager.Instant.LogEventClickAds(ad_type: AD_TYPE.open, adNetwork: adInfo.NetworkName);
            try
            {
                _adOpenClickCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==>Callback click ad open error: " + e.ToString() + "<==");
            }

            _adOpenClickCallback = null;
        }

        private void AppOpen_OnAdDisplayFailedEvent(string arg1, ErrorInfo arg2, AdInfo arg3)
        {
            Debug.LogError("[3rdLib]==> Show ad open/resume failed, code: " + arg2.Code + " <==");

            try
            {
                _callbackOpenAD?.Invoke(false);
                _callbackOpenAD = null;
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==>Callback ad open error: " + e.ToString() + "<==");
            }

            FireBaseManager.Instant.LogADResumeEvent(adState: AD_STATE.show_fail);

            int ID = _OpenAdUnitIDs.IndexOf(arg1);
            if (ID < 0)
                return;
            AdOpenRetryAttemp[ID]++;
            double retryDelay = Math.Pow(2, Math.Min(6, AdOpenRetryAttemp[ID]));
            isShowingAd = false;

            waitLoadAdOpen((float)retryDelay, ID); 

        }

        private void AppOpenOnAdLoadFailedEvent(string arg1, ErrorInfo arg2)
        {
            Debug.LogError("[3rdLib]==> Load ad open/resume failed, code: " + arg2.Code + " <==");
            FireBaseManager.Instant.LogADResumeEvent(adState: AD_STATE.load_fail);
            int ID = _OpenAdUnitIDs.IndexOf(arg1);
            if (ID < 0)
                return;
            AdOpenRetryAttemp[ID]++;
            double retryDelay = Math.Pow(2, Math.Min(6, AdOpenRetryAttemp[ID]));
            isShowingAd = false;

            waitLoadAdOpen((float)retryDelay, ID);
        }


        public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("[3rdLib]==> Ad open/resume close! <==");
            try
            {
                _callbackOpenAD?.Invoke(true);
                _callbackOpenAD = null;
            }
            catch (Exception e)
            {
                Debug.LogError("[3rdLib]==>Callback ad open error: " + e.ToString() + "<==");
            }
            isShowingAd = false;
            int ID = _OpenAdUnitIDs.IndexOf(adUnitId);
            if (ID < 0)
                return;
            AdOpenRetryAttemp[ID]++;
            double retryDelay = Math.Pow(2, Math.Min(6, AdOpenRetryAttemp[ID]));
            isShowingAd = false;
 
            LoadAdOpen(ID);
        }


#endregion

#region CheckAdLoaded

        public bool InterstitialIsLoaded()
        {
            return MaxSdk.IsInterstitialReady(_InterstitialAdUnitID);
        }

        public bool VideoRewardIsLoaded()
        {
            return MaxSdk.IsRewardedAdReady(_RewardedAdUnitID);
        }

        public bool AdsOpenIsLoaded(int ID = 0)
        {
            return MaxSdk.IsAppOpenAdReady(_OpenAdUnitIDs[ID]);
        }

        #endregion
        #endregion

        #region ShowAd

        /// <summary>
        /// Show AD Banner, It doesn't matter SDK init done or not
        /// <code>
        /// _= AdManager.Instant.ShowBanner();
        /// </code>
        /// </summary>
        public async Task ShowBanner()
        {
            if (common.banner_on_off == false)
                return;

            if (_isOffBanner)
                return;

            Advertisements.Instance.hideBannerAd();

            while (!_isBannerInitDone)
            {
                await Task.Delay(500);
            }

            Debug.Log("[3rdLib]==> show banner <==");
            _isBannerCurrentlyAllow = true;

            if (!string.IsNullOrWhiteSpace(_BannerAdUnitID))
                MaxSdk.ShowBanner(_BannerAdUnitID);
            isShowMaxBanner = true;
            Advertisements.Instance.isBannerShowed = true;
        }

        /// <summary>
        /// Hide AD Banner, It doesn't matter SDK init done or not
        /// <code>
        /// AdManager.Instant.DestroyBanner();
        /// </code>
        /// </summary>
        public void DestroyBanner()
        {
            _isBannerCurrentlyAllow = false;
            if (!isShowMaxBanner)
                return;
            Debug.Log("[3rdLib]==> destroy banner <==");

            if (!string.IsNullOrWhiteSpace(_BannerAdUnitID))
                MaxSdk.HideBanner(_BannerAdUnitID);
            isShowMaxBanner = false;
        }

        public async Task ShowMrec()
        {
            if (_isOffMrec == true)
                return;

            while (!_isMrecInitDone)
            {
                await Task.Delay(500);
            }

            Debug.Log("[3rdLib]==> show mrec <==");
            _isMrecCurrentlyAllow = true;

            if (!string.IsNullOrWhiteSpace(_MrecAdUnitID))
                MaxSdk.ShowMRec(_MrecAdUnitID);
        }

        public void DestroyMrec()
        {
            Debug.Log("[3rdLib]==> destroy mrec <==");
            _isMrecCurrentlyAllow = false;

            if (!string.IsNullOrWhiteSpace(_MrecAdUnitID))
                MaxSdk.HideMRec(_MrecAdUnitID);
        }

        /// <summary>
        /// Show AD inter, if user watch ad to get reward but ad not load done yet then you must show the popup "AD not avaiable", then set showNoAds = true
        /// <code>
        ///AdManager.Instant.ShowInterstitial((interState, true)=>{
        ///});
        /// </code>
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="showNoAds"></param>
        public void ShowInterstitial(Action<InterVideoState> callback = null, bool showNoAds = false)
        {
            if (_isOffInter)
                return;

            if (CheckInternetConnection() && InterstitialIsLoaded())
            {
                isShowingAd = true;
                _callbackInter = callback;
                MaxSdk.ShowInterstitial(_InterstitialAdUnitID);
            }
            else
            {
                try
                {
                    callback?.Invoke(InterVideoState.None);
                }
                catch (Exception e)
                {
                    Debug.LogError("[3rdLib]==> Faild invoke callback inter, error: " + e.ToString() + " <==");
                }
                if (_popUpNoAd && showNoAds) _popUpNoAd.SetActive(true);
            }
        }


        /// <summary>
        /// Show AD reward, if user watch ad to get reward but ad not load done yet then you must show the popup "AD not avaiable", then set showNoAds = true
        /// <code>
        ///AdManager.Instant.ShowRewardVideo((interState, true)=>{
        ///});
        /// </code>
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="showNoAds"></param>
        public void ShowRewardVideo(Action<RewardVideoState> callback = null, bool showNoAds = false)
        {
            if (_isOffReward)
                return;

            if (CheckInternetConnection() && VideoRewardIsLoaded())
            {
                isShowingAd = true;
                _callbackReward = callback;
                MaxSdk.ShowRewardedAd(_RewardedAdUnitID);
            }
            else
            {
                try
                {
                    callback?.Invoke(RewardVideoState.None);
                }
                catch (Exception e)
                {
                    Debug.LogError("[3rdLib]==> Faild invoke callback reward, error: " + e.ToString() + " <==");
                }
                if (_popUpNoAd && showNoAds) _popUpNoAd.SetActive(true);
            }
        }

        /// <summary>
        /// Show ad open or resume, if you need callback must check is ad show done or not
        /// <code>
        /// AdManager.Instant.ShowAdOpen(true,(isSuccess)=>{
        ///       if(isSuccess){
        ///         Debug.Log("Done!");
        ///       }else{
        ///         Debug.Log("Fail!");
        ///       }
        /// })
        /// </code>
        /// </summary>
        /// <param name="isAdOpen">Is Ads treated as an open AD</param>
        /// <param name="callback">Callback when adopen show done or fail pass true if ad show success and false if ad fail</param>
        public void ShowAdOpen(int ID = 0, bool isAdOpen = false, Action<bool> callback = null)
        {
            if (common.aoa_admob_on_off == true)
            {
                _callbackOpenAD = callback;
                Advertisements.Instance.ShowAppOpenAd();
                return;
            }
            if (isAdOpen && _isOffAdsOpen)
                return;

            if (!isAdOpen && _isOffAdsResume)
                return;

            if (isShowingAd)
            {
                FireBaseManager.Instant.adTypeShow = AD_TYPE.resume;
                return;
            }

            if (CheckInternetConnection() && AdsOpenIsLoaded(ID))
            {
                FireBaseManager.Instant.adTypeShow = isAdOpen ? AD_TYPE.open : AD_TYPE.resume;
                MaxSdk.ShowAppOpenAd(_OpenAdUnitIDs[ID]);
                _callbackOpenAD = callback;
            }
            else
            {
                FireBaseManager.Instant.adTypeShow = AD_TYPE.resume;
                try
                {
                    callback?.Invoke(false);
                }
                catch (Exception e)
                {
                    Debug.LogError("[3rdLib]==> Faild invoke callback adopen/resume, error: " + e.ToString() + " <==");
                }
            }
        }

        /// <summary>
        /// Show ad open or resume, if you need callback must check is ad show done or not
        /// <code>
        /// AdManager.Instant.ShowAdOpen((isSuccess)=>{
        ///       if(isSuccess){
        ///         Debug.Log("Done!");
        ///       }else{
        ///         Debug.Log("Fail!");
        ///       }
        /// })
        /// </code>
        /// </summary> 
        /// <param name="callback">Callback when adopen show done or fail pass true if ad show success and false if ad fail</param>
        public void ShowAdOpen(Action<bool> callback = null)
        {
            int ID = _OpenAdUnitIDs.Count - 1;
            if (ID < 0)
                return;
            ShowAdOpen(ID,false, callback);
        }

#region Track Revenue

        private void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
        {
            FireBaseManager.Instant.LogAdValueAdjust(adInfo.Revenue);
        }

        private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
        {
            double revenue = impressionData.Revenue;
            var impressionParameters = new[] {
              new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
              new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
              new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
              new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
              new Firebase.Analytics.Parameter("value", revenue),
              new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent(_paid_ad_revenue, impressionParameters);

            if (impressionData.NetworkName == "Facebook Audience Network")
            {
                string adFormat = impressionData.AdFormat; // "INTERSTITIAL", "REWARDED", etc.
                string placement = impressionData.Placement;

                FacebookManager.instance.LogMetaAdImpression(placement, adFormat, (float)revenue);
            }
        }

#endregion

        public bool CheckInternetConnection()
        {
            var internet = false;
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                internet = true;
            }
            else
            {
                internet = false;
            }
            return internet;
        }

        IEnumerator waitReloadAd(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay );

            callback?.Invoke();
        }

#if UNITY_EDITOR

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if(common.is_open_game)
                {
                    if (common.resume_ads_on_off == false)
                        return;

                    if (_OpenAdUnitIDs.Count == 0)
                        return;

                    this.ShowAdOpen(_OpenAdUnitIDs.Count - 1);
                }
            }
        }
#endif

        private void OnAppStateChanged(AppState state)
        {
            // Display the app open ad when the app is foregrounded. 
            if (state == AppState.Foreground)
            {
                if (common.is_open_game)
                {
                    if (common.resume_ads_on_off == false)
                        return;

                    if (_OpenAdUnitIDs.Count == 0)
                        return;

                    this.ShowAdOpen(_OpenAdUnitIDs.Count - 1);
                }
            }
        }

#endregion
    }
}


