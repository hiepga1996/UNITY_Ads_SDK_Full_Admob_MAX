using Firebase.Analytics;
using UnityEngine;
//using com.unity3d.mediation;
using System;

public class IRSController : MonoBehaviour
{
    public static IRSController instance;

    public static string uniqueUserId = "demoUserUnity";

    [SerializeField] private string appKey = "1da4243b5";
    [SerializeField] string RewardedAdUnitId = "19201661";
    [SerializeField] string interstitialAdUnitId = "19201663";
    [SerializeField] string bannerAdUnitId = "19201665";

    bool _isDoneInitIRS = false;
    bool isBannerLoaded = false;
    int bannerRetryAttempt;
    int interRetryAttempt;
    int rewardRetryAttempt;

    //**If use LevelPlay uncommend this**

    //LevelPlayRewardedAd RewardedAd;
    //LevelPlayInterstitialAd interstitialAd;
    //LevelPlayBannerAd bannerAd;

    Action finishReward;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitIRS()
    {
        //**If use LevelPlay uncommend this**

        //LevelPlay.OnInitSuccess += SdkInitializationSuccessEvent;
        //LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        //LevelPlay.Init(appKey);

        ///This For Test Only
        //IronSource.Agent.setMetaData("is_test_suite", "enable"); 
    }

    public bool isDoneInitIRS()
    {
        return _isDoneInitIRS;
    }

    //**If use LevelPlay uncommend this**

    //void SdkInitializationFailedEvent(LevelPlayInitError error)
    //{

    //}

    //void SdkInitializationSuccessEvent(LevelPlayConfiguration configuration)
    //{
    //    InitAds();
    //    _isDoneInitIRS = true;
    //    Debug.Log("IRS_Mediation_Init_done");
    //}

    void InitAds()
    {
        RequestBanner();
        RequestReward();
        RequestInter();
        //IronSource.Agent.launchTestSuite();
    }

    public void OnEnableTest()
    {
        //**If use LevelPlay uncommend this**

        //IronSource.Agent.launchTestSuite();
    }

    #region Reward

    void RequestReward()
    {
        //**If use LevelPlay uncommend this**

        //RewardedAd = new LevelPlayRewardedAd(RewardedAdUnitId);
        //RewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        //RewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        //RewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
        //RewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
        //RewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
        //RewardedAd.OnAdClosed += RewardedOnAdClosedEvent;
        //RewardedAd.OnAdClicked += RewardedOnAdClickedEvent;
        LoadReward();
    }

    void LoadReward()
    {
        //**If use LevelPlay uncommend this**

        //RewardedAd.LoadAd();
    }

    //**If use LevelPlay uncommend this**

    #region ***Reward Event***

    //void RewardedOnAdRewardedEvent(LevelPlayAdInfo info, LevelPlayReward reward)
    //{
    //    if (finishReward != null)
    //    {
    //        finishReward?.Invoke();
    //        finishReward = null;
    //    }
    //}

    //void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    //{
    //    Debug.Log("IRS_Reward_loaded");
    //    rewardRetryAttempt = 0;
    //}

    //void RewardedOnAdLoadFailedEvent(LevelPlayAdError error)
    //{
    //    Debug.Log("IRS_Reward_failed_" + error);
    //    rewardRetryAttempt++;
    //    double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
    //    Invoke("LoadReward", (float)retryDelay);
    //}

    //void RewardedOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    //{
    //    var realRevenue = (double)adInfo.revenue.Value / 1000000;

    //    var impressionParameters = new[] {
    //                        new Parameter("ad_platform", "IronSource"),
    //                        new Parameter("ad_source", "IronSource"),
    //                        new Parameter("ad_unit_name", "Reward"),
    //                        new Parameter("ad_format", "Reward_IronSource"),
    //                        new Parameter("value", realRevenue),
    //                        new Parameter("currency", "USD"),
    //                      };

    //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    //}

    //void RewardedOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError) { }

    //void RewardedOnAdClosedEvent(LevelPlayAdInfo adInfo)
    //{
    //    LoadReward();
    //}

    //void RewardedOnAdClickedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    public void ShowReward(Action Completed,  Action fail = null)
    {
        //**If use LevelPlay uncommend this**

        //if (RewardedAd.IsAdReady() == true)
        //{
        //    RewardedAd.ShowAd();
        //    finishReward = Completed;
        //}
        //else
        //    fail.Invoke();
    }

    #endregion

    #region Inter

    void RequestInter()
    {
        //**If use LevelPlay uncommend this**

        //interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
        //interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        //interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        //interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        //interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        //interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        //interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        LoadInter();
    }

    void LoadInter()
    {
        //**If use LevelPlay uncommend this**

        //interstitialAd.LoadAd();
    }

    #region ***Inter Event***

    //void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    //{
    //    Debug.Log("IRS_Inter_loaded");
    //    interRetryAttempt = 0;
    //}

    //void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    //{
    //    Debug.Log("IRS_Inter_failed_" + error);
    //    interRetryAttempt++;
    //    double retryDelay = Math.Pow(2, Math.Min(6, interRetryAttempt));
    //    Invoke("LoadInter", (float)retryDelay);
    //}

    //void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    //{
    //    var realRevenue = (double)adInfo.revenue.Value / 1000000;

    //    var impressionParameters = new[] {
    //                        new Parameter("ad_platform", "IronSource"),
    //                        new Parameter("ad_source", "IronSource"),
    //                        new Parameter("ad_unit_name", "Interstitial"),
    //                        new Parameter("ad_format", "Inter_IronSource"),
    //                        new Parameter("value", realRevenue),
    //                        new Parameter("currency", "USD"),
    //                      };

    //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    //}

    //void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError) { }

    //void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }

    //void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    //{
    //    LoadInter();
    //}

    #endregion

    public void ShowInter(Action completed)
    {
        //**If use LevelPlay uncommend this**

        //if (interstitialAd.IsAdReady() == true)
        //{
        //    interstitialAd.ShowAd();
        //    completed?.Invoke();
        //}
    }

    #endregion

    #region Banner

    void RequestBanner()
    {
        //**If use LevelPlay uncommend this**

        //bannerAd = new LevelPlayBannerAd(bannerAdUnitId, LevelPlayAdSize.BANNER, LevelPlayBannerPosition.BottomCenter);
        //bannerAd.GetAdSize();
        //bannerAd.GetPosition();
        //bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        //bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        //bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        //bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        //bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        //bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        //bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        //bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        LoadBanner();
    }

    void LoadBanner()
    {
        //**If use LevelPlay uncommend this**

        //bannerAd.LoadAd();
        //bannerAd.ResumeAutoRefresh();
    }

    #region ***Banner Event***

    //void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    //{
    //    Debug.Log("IRS_Banner_loaded");
    //    isBannerLoaded = true;
    //    bannerRetryAttempt -= bannerRetryAttempt;
    //}

    //void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    //{
    //    Debug.Log("IRS_Banner_Failed_" + ironSourceError);
    //    bannerRetryAttempt++;
    //    double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));
    //    Invoke("LoadBanner", (float)retryDelay);

    //}

    //void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo) { }

    //void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    //{
    //    var realRevenue = (double)adInfo.revenue.Value / 1000000;

    //    var impressionParameters = new[] {
    //                        new Parameter("ad_platform", "IronSource"),
    //                        new Parameter("ad_source", "IronSource"),
    //                        new Parameter("ad_unit_name", "Banner"),
    //                        new Parameter("ad_format", "Banner_IronSource"),
    //                        new Parameter("value", realRevenue),
    //                        new Parameter("currency", "USD"),
    //                      };

    //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    //}

    //void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError) { }

    //void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) { }

    //void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { }

    //void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    public void ShowBanner()
    {
        //**If use LevelPlay uncommend this**

        //if (isBannerLoaded == true)
        //{
        //    bannerAd?.ShowAd();
        //    isBannerLoaded = false;
        //}
    }

    public void HideBanner()
    {
        //**If use LevelPlay uncommend this**

        //bannerAd?.HideAd();
    }

    #endregion

}
