using UnityEngine;
using System.Collections;
using Ultility;
using System;

public class NativeSpin : MonoBehaviour
{
    public static NativeSpin instance;
    public GameObject _nativeTop;

    private void Awake()
    {
        instance = this;
    }

    public bool checkHaveNativeShowing()
    {
        if (_nativeTop && _nativeTop.activeSelf)
            return true;

        return false;
    }

    public bool checkNativeTopShow()
    {
        return _nativeTop.activeSelf;
    }

    public void showNative(bool isShow)
    {
        if (common.native_spin_on_off == false)
            return;

        if (Advertisements.Instance.is_offNative == true)
        {
            _nativeTop.SetActive(false);
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
    //    _nativeTop.SetActive(isShow);
    //    if (isShow == true)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        Advertisements.Instance.setNativeObject(_nativeTop);
    //        Advertisements.Instance.SetTextureAndDetail();
    //    }
    //    else
    //        Advertisements.Instance.startCountRequestNativeNow();
    }
}
