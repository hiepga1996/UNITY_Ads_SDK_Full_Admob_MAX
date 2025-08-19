using Ultility;
using UnityEngine;

public class CanvasNativeUI : MonoBehaviour
{
    public static CanvasNativeUI Instants;
    public GameObject _nativeLoading;

    private void Awake()
    {
        Instants = this;
    }

    public bool checkHaveNativeShowing()
    {
        if (_nativeLoading && _nativeLoading.activeSelf)
            return true;

        return false;
    }
    //public bool checkNativeLoadingShow()
    //{
    //    if (common.Native_Loading_On_Off != 0)
    //        return false;

    //    return _nativeLoading.activeSelf;
    //}
    public void showNativeLoading(bool isShow)
    {
        //if (common.Native_Loading_On_Off != 0)
        //    return;

        //_nativeLoading.SetActive(isShow);
        //if (isShow)
        //{
        //    Advertisements.Instance.setNativeObject(_nativeLoading);
        //    Advertisements.Instance.SetTextureAndDetail();
        //}
        //else
        //{
        //    Advertisements.Instance.startCountRequestNativeNow();
        //}
    }
}
