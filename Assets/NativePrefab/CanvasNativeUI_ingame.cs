using UnityEngine;

public class CanvasNativeUI_ingame : MonoBehaviour
{
    public static CanvasNativeUI_ingame Instants;
    public GameObject _nativeTop;

    private void Awake()
    {
        Instants = this;
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

    public void showNativeTop(bool isShow)
    {

        //_nativeTop.SetActive(isShow);
        //if (isShow)
        //{
        //    Advertisements.Instance.setNativeObject(_nativeTop);
        //    Advertisements.Instance.SetTextureAndDetail();
        //}
        //else
        //{
        //    Advertisements.Instance.startCountRequestNativeNow();
        //}
    }
}
