using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DVAH;
using Ultility;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Scrollbar loadingbar;
    private float deltaScrolling = 0f;
    private float timeScrolling = 30f;
    private bool isShowLoading = false;
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        UMPManager.Instance.InitUMP(() =>
        {
            Time.timeScale = 1;
            isShowLoading = true;
            MasterLib.Instant.InitChildLib();
            StartCoroutine(WaitAndPrint());
        }, () =>
        {
            Time.timeScale = 0;
        }, () =>
        {
            Time.timeScale = 1;
        });

    }

    private IEnumerator WaitAndPrint()
    {
        yield return new WaitUntil(() => AdManager.Instant.AdsOpenIsLoaded(0));
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForEndOfFrame();

        loadingbar.size = 1;
        isShowLoading = false;
        AdManager.Instant.ShowAdOpen(0, true, (isSuccess) =>
        {
            //Advertisements.Instance.startCountAppOpen();
            if (Advertisements.Instance.is_fisrt_show_app_open)
            {
                waitAdOpen();
                Advertisements.Instance.is_fisrt_show_app_open = false;
            }
            else
            {
                waitAdOpen();
            }
        });
    }

    public void btnDebugMode()
    {
        MaxSdk.ShowMediationDebugger();
    }

    public void waitAdOpen()
    {
        common.is_open_game = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isShowLoading == false)
        {
            return;
        }
        else
        {
            deltaScrolling += Time.deltaTime;
            var sizeScroll = deltaScrolling / timeScrolling;
            if (sizeScroll <= 1)
                loadingbar.size = sizeScroll;
            else
            {
                deltaScrolling = 0f;
                isShowLoading = false;
                common.is_open_game = true;
                gameObject.SetActive(false);
            }
        }
    }
}
