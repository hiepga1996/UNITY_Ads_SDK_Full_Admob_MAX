using UnityEngine;
using System.Collections;
using System;

public class NativeGamePlay : MonoBehaviour
{
    public static NativeGamePlay instance;
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
        //yield break;
        if (isShow == true)
        {
            Debug.LogError("Native Showing.");
            yield return new WaitUntil(() => Advertisements.Instance.IsNativeReady() == true);
            yield return new WaitForEndOfFrame();
            _nativeTop.SetActive(isShow);
            Advertisements.Instance.setNativeObject(_nativeTop);
            Advertisements.Instance.SetTextureAndDetail();
            Debug.LogError("Native Showed.");
        }
        else
        {
            _nativeTop.SetActive(isShow);
            Advertisements.Instance.startCountRequestNativeNow();

        }
    }
}
