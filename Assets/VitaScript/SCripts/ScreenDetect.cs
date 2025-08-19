using UnityEngine;

public enum ENUM_Device_Type
{
    Tablet,
    Phone
}

public static class ScreenDetect
{
    public static bool isTablet;

    private static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        return diagonalInches;
    }

    public static ENUM_Device_Type GetDeviceType()
    {
#if UNITY_IOS
    bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
            {
                return ENUM_Device_Type.Tablet;
            }
            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            if (deviceIsIphone)
            {
                return ENUM_Device_Type.Phone;
            }
#elif UNITY_ANDROID

        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);

        if(CheckFold() == true)
        {
            float aspect = (float)Screen.width / Screen.height;

            if (DeviceDiagonalSizeInInches() > 6.5f)
                return ENUM_Device_Type.Tablet;
            else
                return ENUM_Device_Type.Phone;
        }

        if (isTablet)
            return ENUM_Device_Type.Tablet;
        else
            return ENUM_Device_Type.Phone;
#endif
    }

    public static bool CheckFold()
    {
        string deviceModel = SystemInfo.deviceModel;
        //Debug.LogError("device_name === " + deviceModel);
        if (deviceModel.Contains("Galaxy Z Fold") || deviceModel.Contains("SM-F9"))
        {
            //Debug.LogError("Detected a Galaxy Z Fold device.");
            // Add your code for Z Fold specific logic here
            return true;
        }
        else
        {
            //Debug.LogError("Not a Galaxy Z Fold device.");
            return false;
        }
    }
}