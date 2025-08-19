using UnityEngine;
using System;
using GoogleMobileAds.Api;
using Ultility;
using DVAH;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Advertisements : MonoBehaviour
{
    private static Advertisements _instant = null;

    public bool is_fisrt_show_app_open = true;
    public int TYPE_REWARD = 0;

    public bool is_offBanner = false, is_offInter = false, is_offMrec = false, is_offNative = false;

    [SerializeField] private string adUnitBannerAdaptive, adUnitAppOpen, adUnityReward;
    public bool isInitFirebaseDone = false;
    private RewardedAd _rewardedAd;
    //private bool start_countdown_collap = false; 
    //private float count_collap = 0f;

    public bool is_test = true;

    bool _initSdkAdmobDone = false;
    public bool offBannerAdmob = false;

    public bool nativeMenuFirstOpenDone = false;

    //public bool canShowCollap = true;
    //public bool collap_showed = false;

    public static Advertisements Instance
    {
        get
        {
            if (_instant == null)
            {
                _instant = FindObjectOfType<Advertisements>();
            }

            return _instant;
        }
    }

    void Awake()
    {
        if (_instant != null && _instant.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            Destroy(this.gameObject);
        else
            _instant = this.GetComponent<Advertisements>();

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        // Count time refresh collapsible
        //if (start_countdown_collap)
        //{
        //    count_collap += Time.deltaTime;
        //    if (count_collap >= PlayerDataManager.Instance.capping_collapse_banner)
        //    {
        //        count_collap = 0;
        //        canShowCollap = true;
        //    }
        //}

        // Inter
        if (can_show_inter == false && start_countdown_show_inter == true)
        {
            count_time_countdown_inter += Time.deltaTime;

            if (count_time_countdown_inter >= config_time_countdown_show_inter)
            {
                can_show_inter = true;
                count_time_countdown_inter = 0;
            }
        }

        //Fix Later
        //Count time request native next
        if (!canRequestNative && startCountRequestNative)
        {
            countNative += Time.deltaTime;
            if (countNative >= cappingNative)
            {
                startCountRequestNative = false;
                canRequestNative = true;
                countNative = 0f;
                try
                {
                    RequestNativeAd();
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("native_fail_request");
                }
            }
        }
    }

    public void GetIdsAds()
    {
        if (is_test)
        {
            adUnitBannerAdaptive = "ca-app-pub-3940256099942544/9214589741";
            NativeAdUnitId = "ca-app-pub-3940256099942544/2247696110";
            adUnitAppOpen = "ca-app-pub-3940256099942544/9257395921";
            adUnityReward = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
            adUnitBannerAdaptive = "ca-app-pub-2266966365312947/4529474151";
            NativeAdUnitId = "ca-app-pub-2266966365312947/1156650447";
            adUnitAppOpen = "ca-app-pub-2266966365312947/2395616478";
            adUnityReward = "ca-app-pub-2266966365312947/1366120152";
        }

        //NativeAdUnitId = common.admob_native_ids_ads;
    }

    public void initAdmod()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            _initSdkAdmobDone = true;
        });
    }

    public void RequestAfterInit()
    {
        RequestAndLoadAppOpenAd();
        LoadRewardedAd();
        try
        {
            RequestNativeAd();
            Debug.LogError("Request_native");
        }
        catch (Exception e)
        {
            Debug.LogError("faild_init_native");
        }
    }

    public bool InitAdmobDone()
    {
        return _initSdkAdmobDone;
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt(common.REMOVE_ADS_KEY, 1);
        AdManager.Instant._isOffBanner = true;
        AdManager.Instant._isOffInter = true;
        AdManager.Instant._isOffMrec = true;

        is_offBanner = true;
        is_offInter = true;
        is_offMrec = true;
        is_offNative = true;

        hideBannerAd();
        AdManager.Instant.DestroyBanner();
        HideMrec();
    }

    private void PrintStatus(string message)
    {
        Debug.Log(message);
    }

    #region Banner

    private BannerView bannerView;
    private bool _bannerShowing = false;
    private int bannerRetryAttempt;
    public bool isBannerShowed = false;
  
    public void RequestBannerAdaptive()
    {
        if (is_offBanner == true)
            return;
        //if (common.level_more_not_Ads > 0)
        //    return;
        try
        {
            CalledBanner();
        }
        catch (Exception e)
        {
            Debug.LogError("error_load_banner");
        }
    }

    public void CalledBanner()
    {
        if (common.banner_on_off == false)
            return;

        if (is_offBanner == true)
            return;

        if (offBannerAdmob == true)
            return;

        AdManager.Instant.DestroyBanner();

        //if (canShowCollap == false)
        //{
        //    bannerView.Show();
        //    return;
        //}
        //canShowCollap = false;
        //start_countdown_collap = true;
        string adUnitId;
        adUnitId = adUnitBannerAdaptive;


        if (bannerView != null)
            bannerView.Destroy();

        AdSize adaptiveSize = AdSize.Banner;

        if (common.banner_adaptive_on_off == true)
            adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        //PrintStatus("Requesting Banner ad.111");
        // Create a 320x50 banner at bottom of the screen
        //bannerView = new BannerView(adUnitId, AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Add Event Handlers
        bannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("Banner ad loaded.");
            _bannerShowing = true;
        };
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("Banner ad failed to load with error: " + error.GetMessage());

            _bannerShowing = false;
            bannerRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));
            if(common.switch_banner_when_false == true)
            {
                if (common.ads_banner_type == 3)
                    return;

                offBannerAdmob = true;
                StartCoroutine("InitMaxBanner");
            }
            else
                Invoke("CalledBanner", (float)retryDelay);
        };
        bannerView.OnAdImpressionRecorded += () =>
        {
            PrintStatus("Banner ad recorded an impression.");
        };
        bannerView.OnAdClicked += () =>
        {
            PrintStatus("Banner ad recorded a click.");
        };
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("Banner ad opening.");
            //AdManager.Instant.DestroyBanner();
            //Invoke("AwaitShowBannerMax", common.banner_CB_delay);
        };
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Banner ad closed.");
            _bannerShowing = false;
        };
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            var realRevenue = (double)adValue.Value / 1000000;

            var impressionParameters = new[] {
                  new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                  //new Firebase.Analytics.Parameter("ad_source", "Admob"),
                  //new Firebase.Analytics.Parameter("ad_unit_name", adUnitBannerCollapsive),
                  //new Firebase.Analytics.Parameter("ad_format", "Banner Collapsive"),
                  new Firebase.Analytics.Parameter("value", realRevenue),
                  new Firebase.Analytics.Parameter("currency", adValue.CurrencyCode),
                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        };

        if (adUnitId == null || adaptiveSize == null)
        {
            adUnitId = "ca-app-pub-2266966365312947/4529474151";
            adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);
        }
        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
        isBannerShowed = true;
    }

    IEnumerator InitMaxBanner()
    {
        yield return new WaitUntil(() => AdManager.Instant.initMaxDone() == true);
        if (bannerView != null)
            bannerView.Destroy();
        AdManager.Instant._isOffBanner = false;
        AdManager.Instant.ShowBanner();
    }

    public void ShowBanner()
    {
        if (is_offBanner == true)
            return;

        if (common.ads_banner_type == 2)
        {
            AdManager.Instant.ShowBanner();
            return;
        }
        else if (common.ads_banner_type == 3)
        {
           IRSController.instance.ShowBanner();
            return;
        }

        if (bannerView == null)
            RequestBannerAdaptive();
        else
            bannerView.Show();
    }

    public void hideBannerAd()
    {
        if (offBannerAdmob == true)
        {
            AdManager.Instant.DestroyBanner();
            return;
        }
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }

    void ManuallyLoadBanner()
    {
        try
        {
            CalledBanner();
        }
        catch (Exception e)
        {
            Debug.LogError("error_load_banner");
        }
    }

    #endregion

    #region AOA

    private AppOpenAd appOpenAd;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;

    public void RequestAndLoadAppOpenAd()
    {
        PrintStatus("Requesting App Open ad.");
        string adUnitId = adUnitAppOpen;

        // destroy old instance.
        if (appOpenAd != null)
        {
            DestroyAppOpenAd();
        }

        // Create a new app open ad instance.
        AppOpenAd.Load(adUnitId, CreateAdRequest
        (), (AppOpenAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    PrintStatus("App open ad failed to load with error: " +
                        loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    PrintStatus("App open ad failed to load.");
                    return;
                }

                PrintStatus("App Open ad loaded. Please background the app and return.");
                this.appOpenAd = ad;
                this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;


                ad.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("App open ad opened.");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("App open ad closed.");
                    StartCoroutine(waitLoadAdOpen(1f));
                    try
                    {
                        AdManager.Instant._callbackOpenAD?.Invoke(false);
                        AdManager.Instant._callbackOpenAD = null;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[3rdLib]==>Callback ad open error: " + e.ToString() + "<==");
                    }
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    PrintStatus("App open ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    PrintStatus("App open ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    PrintStatus("App open ad failed to show with error: " +
                        error.GetMessage());

                    StartCoroutine(waitLoadAdOpen(1f));

                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    var realRevenue = (double)adValue.Value / 1000000;

                    var impressionParameters = new[] {
                      new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                      //new Firebase.Analytics.Parameter("ad_source", "Admob"),
                      //new Firebase.Analytics.Parameter("ad_unit_name", adUnitAppOpen),
                      new Firebase.Analytics.Parameter("ad_format", "AOA_Admob"),
                      new Firebase.Analytics.Parameter("value", realRevenue),
                      new Firebase.Analytics.Parameter("currency", adValue.CurrencyCode),
                    };

                    Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
                };
        });
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }

    IEnumerator waitLoadAdOpen(float time)
    {
        yield return new WaitForSeconds(time);
        RequestAndLoadAppOpenAd();
    }

    public void DestroyAppOpenAd()
    {
        if (this.appOpenAd != null)
        {
            this.appOpenAd.Destroy();
            this.appOpenAd = null;
        }
    }

    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (appOpenAd != null
                    && appOpenAd.CanShowAd()
                    && DateTime.Now < appOpenExpireTime);
        }
    }

    public void ShowAppOpenAd()
    {
        if (!IsAppOpenAdAvailable)
        {
            return;
        }
        appOpenAd.Show();
    }

    #endregion

    #region Inter

    public int config_time_countdown_show_inter = 30;
    private float count_time_countdown_inter = 0f;
    public bool start_countdown_show_inter = false;
    public bool can_show_inter = false;

    public void ShowInterstitialAd()
    {
        //AdManager.Instant.ShowInterstitial();
        if (PlayerPrefs.GetInt("reward_ingame", 0) != 0)
        {
            PlayerPrefs.SetInt("reward_ingame", 0);
            return;
        }


        if (is_offInter)
            return;

        if (can_show_inter == false)
            return;

        //if (common.level_more_not_Ads > 0)
        //    return;

        // check choi trong ban choi 5ph moi hien
        if (!common.canShowInterstitial5MinuteInGame)
            return;

        // check choi 3 lvl thi hien
        //if (PlayerDataManager.Instance.can_show_inter == false)
        //{
        //    PlayerDataManager.Instance.count_level_show_inter++;
        //    if (PlayerDataManager.Instance.count_level_show_inter >= 1)
        //    {
        //        PlayerDataManager.Instance.can_show_inter = true;
        //        PlayerDataManager.Instance.count_level_show_inter = 0;
        //    }
        //    return;
        //}
        //else
        //{
        //    PlayerDataManager.Instance.can_show_inter = false;
        //    PlayerDataManager.Instance.count_level_show_inter = 0;

        try
        {
            if (common.ads_inter_type == 1 || common.ads_inter_type == 2)
            {
                AdManager.Instant.ShowInterstitial((status) =>
                {
                    can_show_inter = false;
                    start_countdown_show_inter = true;
                    if (status == InterVideoState.Closed)
                    {
                    }
                    else
                    {
                    }
                }, showNoAds: false);
            }
            else
            {
                IRSController.instance.ShowInter(() =>
                {
                    can_show_inter = false;
                    start_countdown_show_inter = true;
                });
            }
           
        }
        catch (Exception e) 
        { 

        }
    }


    #endregion

    #region Reward

    int rewardRetryAttempt = 0;

    void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adUnityReward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    rewardRetryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
                    Invoke("LoadRewardedAd", (float)retryDelay);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                rewardRetryAttempt = 0;
                _rewardedAd = ad;

                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad full screen content closed.");
                    LoadRewardedAd();
                };

                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content " +
                                       "with error : " + error);
                    rewardRetryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
                    Invoke("LoadRewardedAd", (float)retryDelay);
                    return;
                };

                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                        adValue.Value,
                        adValue.CurrencyCode));
                    var realRevenue = (double)adValue.Value / 1000000;

                    var impressionParameters = new[] {
                  new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                  //new Firebase.Analytics.Parameter("ad_source", "Admob"),
                  //new Firebase.Analytics.Parameter("ad_unit_name", adUnitBannerCollapsive),
                  new Firebase.Analytics.Parameter("ad_format", "Reward"),
                  new Firebase.Analytics.Parameter("value", realRevenue),
                  new Firebase.Analytics.Parameter("currency", adValue.CurrencyCode),
                };
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
                };

                // Raised when an impression is recorded for an ad.
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Rewarded ad recorded an impression.");
                };

                // Raised when a click is recorded for an ad.
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Rewarded ad was clicked.");
                };

                // Raised when an ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Rewarded ad full screen content opened.");
                };

            });
    }

    void ShowRewardAdmob(Action complele = null, Action fail = null)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                if(complele != null)
                    complele?.Invoke();
            });
        }
        else
        {
            if(fail != null)
                fail?.Invoke();
        }
    }

    public void ShowRewardedAd(Action complele = null, Action fail = null)
    {
        if (common.isStartGame)
            PlayerPrefs.SetInt("reward_ingame", 1);

        if (common.ads_reward_type == 1)
        {
            ShowRewardAdmob(complele, fail);
        }
        else if (common.ads_reward_type == 2)
        {
            AdManager.Instant.ShowRewardVideo((status) =>
            {
                //Do sth when user watched reward
                if (status == RewardVideoState.Watched)
                {
                    if (complele != null)
                    {
                        complele?.Invoke();
                    }
                }
                else
                {
                    if (fail != null)
                    {
                        fail?.Invoke();
                    }
                }
            }, showNoAds: false);
        }
        else
            IRSController.instance.ShowReward(complele, fail);
    }

    #endregion

    #region NATIVE ADS

    NativeAd _nativeAd;
    public string NativeAdUnitId;

    bool startCountRequestNative = false;
    bool canRequestNative = true;
    float countNative = 0f;
    float cappingNative = 1f;

    int maxRequestNative = 1;
    int countNativeTexture = 0;

    bool nativeAdsReady = false;
    List<GameObject> _native_object_list;
    List<NativeAd> _nativead_list = new List<NativeAd>();

    private void RequestNativeAd()
    {
        if (is_offNative == true)
            return;
        if (common.native_on_off == false)
            return;

        if (!canRequestNative)
            return;

        for (var i = 0; i < _nativead_list.Count; i++)
        {
            if (_nativead_list[i] != null)
            {
                _nativead_list[i].Destroy();
            }
        }

        countNativeTexture = 0;
        _nativead_list.Clear();

        Debug.LogError("Start Request Native ");
        AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
            .ForNativeAd()
            .SetNumberOfAdsToLoad(maxRequestNative)
            .Build();
        adLoader.OnNativeAdLoaded += HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        adLoader.LoadAd(new AdRequest());
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.LogError("Native ad failed to load: " + args.LoadAdError.GetMessage());
        Invoke("CallRequestNextNativeAd", 5f);
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.LogError("Native ad loaded.");
        //nativeAd = args.nativeAd;
        _nativead_list.Add(args.nativeAd);
        countNativeTexture++;
        nativeAdsReady = true;
        if (countNativeTexture >= maxRequestNative)
        {
            // luu lai request doi khi nao duoc goi show
            //SetTextureAndDetail();

            //if (CanvasNativeUI.Instants.checkHaveNativeShowing())
            //    SetTextureAndDetail();
        }
    }

    bool firstShowNative = false;
    public bool showNativeLoadingDone = false;

    public void setNativeObject(GameObject obj)
    {
        _native_object_list.Clear();
        _native_object_list.Add(obj);
    }

    public void startCountRequestNativeNow()
    {
        startCountRequestNative = true;
    }

    void ShowNativeLoadingDone()
    {
        showNativeLoadingDone = true;
    }

    public void SetTextureAndDetail()
    {
        Debug.LogError("SetTextureAndDetail.");
        if (_nativead_list.Count == 0)
            return;

        if (!canRequestNative)
            return;

        if (_native_object_list.Count <= 0)
            return;

        for (var i = 0; i < _native_object_list.Count; i++)
        {
            NativeAd nativeAd = _nativead_list[i];
            if (nativeAd == null)
                continue;

            var NADS = _native_object_list[i];
            if (NADS == null) continue;

            GameObject AdLoaded = NADS.transform.GetChild(0).gameObject;
            GameObject AdLoading = NADS.transform.GetChild(1).gameObject;
            GameObject close = NADS.transform.GetChild(2).gameObject;

            if (AdLoaded == null) continue;
            if (AdLoading == null) continue;
            if (close == null) continue;

            AdLoaded.gameObject.SetActive(false);
            AdLoading.gameObject.SetActive(false);

            RawImage AdIconTexture = AdLoaded.transform.GetChild(1).GetComponent<RawImage>();
            RawImage AdIconChoice = AdLoaded.transform.GetChild(2).GetComponent<RawImage>();

            Text AdHeadline = AdLoaded.transform.GetChild(3).GetComponent<Text>();
            Text Advertiser = AdLoaded.transform.GetChild(4).GetComponent<Text>();
            Text BodyNative = AdLoaded.transform.GetChild(5).GetComponent<Text>();
            Text CallActionText = AdLoaded.transform.GetChild(7).transform.GetChild(0).GetComponent<Text>();

            Texture2D iconTexture = nativeAd.GetIconTexture();
            Texture2D iconAdChoices = nativeAd.GetAdChoicesLogoTexture();

            if (iconTexture)
                AdIconTexture.texture = iconTexture;
            else
                AdIconTexture.gameObject.SetActive(false);

            if (iconAdChoices)
                AdIconChoice.texture = iconAdChoices;
            else
                AdIconChoice.gameObject.SetActive(false);

            AdHeadline.text = nativeAd.GetHeadlineText();
            BodyNative.text = nativeAd.GetBodyText();
            CallActionText.text = nativeAd.GetCallToActionText();
            Advertiser.text = nativeAd.GetAdvertiserText();

            //register gameobjects with native ads api
            if (!nativeAd.RegisterIconImageGameObject(AdIconTexture.gameObject))
            {
                Debug.Log("error registering AdIconTexture");
            }
            if (!nativeAd.RegisterAdChoicesLogoGameObject(AdIconChoice.gameObject))
            {
                Debug.Log("error registering AdIconChoice");
            }
            if (!nativeAd.RegisterHeadlineTextGameObject(AdHeadline.gameObject))
            {
                Debug.Log("error registering AdHeadline");
            }
            if (!nativeAd.RegisterBodyTextGameObject(BodyNative.gameObject))
            {
                Debug.Log("error registering BodyNative");
            }
            if (!nativeAd.RegisterCallToActionGameObject(CallActionText.gameObject))
            {
                Debug.Log("error registering CallActionText");
            }
            if (!nativeAd.RegisterAdvertiserTextGameObject(Advertiser.gameObject))
            {
                Debug.Log("error registering Advertiser");
            }

            _nativeAd = nativeAd;
            _nativeAd.OnPaidEvent += OnPaidEvent;

            //disable loading and enable ad object
            AdLoaded.gameObject.SetActive(true);
            AdLoading.gameObject.SetActive(false);
            close.SetActive(true);
            nativeAdsReady = false;
        }

        _native_object_list.Clear();
        canRequestNative = false;
    }

    public bool IsNativeReady()
    {
        return nativeAdsReady;
    }

    private void OnPaidEvent(object sender, AdValueEventArgs adValueEventArgs)
    {
        AdValue adValue = adValueEventArgs.AdValue;

        var trueValue = (double)adValue.Value / 1000000;

        string currencyCode = adValue.CurrencyCode;
        ResponseInfo responseInfo = _nativeAd.GetResponseInfo();
        string responseId = responseInfo.GetResponseId();

        AdapterResponseInfo loadedAdapterResponseInfo = responseInfo.GetLoadedAdapterResponseInfo();
        string adSourceId = loadedAdapterResponseInfo.AdSourceId;
        string adSourceInstanceId = loadedAdapterResponseInfo.AdSourceInstanceId;
        string adSourceInstanceName = loadedAdapterResponseInfo.AdSourceInstanceName;
        string adSourceName = loadedAdapterResponseInfo.AdSourceName;
        string adapterClassName = loadedAdapterResponseInfo.AdapterClassName;
        long latencyMillis = loadedAdapterResponseInfo.LatencyMillis;
        Dictionary<string, string> credentials = loadedAdapterResponseInfo.AdUnitMapping;

        var impressionParameters = new[] {
                    new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                    new Firebase.Analytics.Parameter("ad_source", adSourceName),
                    new Firebase.Analytics.Parameter("ad_unit_name", NativeAdUnitId),
                    new Firebase.Analytics.Parameter("ad_format", "NATIVE"),
                    new Firebase.Analytics.Parameter("value", trueValue),
                    new Firebase.Analytics.Parameter("currency", currencyCode), // All AppLovin revenue is sent in USD 
                };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }

    public void CallRequestNextNativeAd()
    {
        if (canRequestNative) RequestNativeAd();
    }

    #endregion

    #region Mrec

    public void showMrec()
    {
        if (is_offMrec == true)
            return;

        AdManager.Instant.ShowMrec();
    }

    public void HideMrec()
    {
        if (is_offMrec == true)
            return;

        AdManager.Instant.DestroyMrec();
    }

    #endregion
}