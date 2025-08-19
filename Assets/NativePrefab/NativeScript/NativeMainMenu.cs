using UnityEngine;
using System.Collections;
using Ultility;
using System;

public class NativeMainMenu : MonoBehaviour
{
    public static NativeMainMenu instance;
    public GameObject _native1;
    public GameObject _native2;
    public GameObject _native3;
    public GameObject _native4;
    public bool doneShowNative = false;

    private void Awake()
    {
        instance = this;
    }

    public bool checkHaveNativeShowing()
    {
        if (_native1 && _native1.activeSelf)
            return true;
        if (_native2 && _native2.activeSelf)
            return true;
        if (_native3 && _native3.activeSelf)
            return true;
        if (_native4 && _native4.activeSelf)
            return true;
        return false;
    }

    //public bool checkNativeTopShow()
    //{
    //    return _nativeTop.activeSelf;
    //}

    public void showNative(bool isShow)
    {
        if (common.native_level_select_on_off == false)
            return;

        if (Advertisements.Instance.is_offNative == true)
        {
            _native1.SetActive(false);
            _native2.SetActive(false);
            _native3.SetActive(false);
            _native4.SetActive(false);
            return;
        }
       
        try
        {
            StartCoroutine(ShowNative(isShow));
        }
        catch (Exception e)
        {
            Debug.LogError("failed_show_native");
        }
    }

    IEnumerator ShowNative(bool isShow)
    {
        yield break;
        //if (isShow == true)
        //{
        //    if(Advertisements.Instance.nativeMenuFirstOpenDone == true)
        //    {
        //        yield return new WaitUntil(() => Advertisements.Instance.IsNativeReady() == true);
        //        yield return new WaitForEndOfFrame();
        //        _native1.SetActive(isShow);
        //        Advertisements.Instance.setNativeObject(_native1);
        //        Advertisements.Instance.SetTextureAndDetail();

        //        yield return new WaitForSecondsRealtime(1.5f);
        //        Advertisements.Instance.startCountRequestNativeNow();
        //    }

        //    yield return new WaitUntil(() => Advertisements.Instance.IsNativeReady() == true);
        //    yield return new WaitForEndOfFrame();
        //    if (Advertisements.Instance.nativeMenuFirstOpenDone == false)
        //        Advertisements.Instance.nativeMenuFirstOpenDone = true;
        //    _native2.SetActive(isShow);
        //    Advertisements.Instance.setNativeObject(_native2);
        //    Advertisements.Instance.SetTextureAndDetail();

        //    yield return new WaitForSecondsRealtime(1.5f);
        //    Advertisements.Instance.startCountRequestNativeNow();
        //    yield return new WaitUntil(() => Advertisements.Instance.IsNativeReady() == true);
        //    yield return new WaitForEndOfFrame();
        //    _native3.SetActive(isShow);
        //    Advertisements.Instance.setNativeObject(_native3);
        //    Advertisements.Instance.SetTextureAndDetail();

        //    yield return new WaitForSecondsRealtime(1.5f);
        //    Advertisements.Instance.startCountRequestNativeNow();
        //    yield return new WaitUntil(() => Advertisements.Instance.IsNativeReady() == true);
        //    yield return new WaitForEndOfFrame();
        //    _native4.SetActive(isShow);
        //    Advertisements.Instance.setNativeObject(_native4);
        //    Advertisements.Instance.SetTextureAndDetail();

        //    yield return new WaitForSeconds(1.5f);
        //    Advertisements.Instance.startCountRequestNativeNow();
        //    yield return new WaitForSeconds(1f);
        //    doneShowNative = true;
        //}
        //else
        //{
        //    _native1.SetActive(isShow);
        //    _native2.SetActive(isShow);
        //    _native3.SetActive(isShow);
        //    _native4.SetActive(isShow);
        //    Advertisements.Instance.startCountRequestNativeNow();
        //}
    }
}
