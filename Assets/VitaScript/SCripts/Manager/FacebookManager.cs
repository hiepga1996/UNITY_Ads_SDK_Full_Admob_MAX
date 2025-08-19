using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager instance;

    // public GameObject Panel_Add;
    private Text FB_userName;
    private Image FB_useerDp;
    private GameObject friendstxtprefab;
    private GameObject GetFriendsPos;
    private static readonly string EVENT_PARAM_SCORE = "score";
    private static readonly string EVENT_NAME_GAME_PLAYED = "game_played";

    private void Awake()
    {
        instance = this;

        //FB.Init(SetInit, onHidenUnity);

        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    FB.Mobile.SetAdvertiserTrackingEnabled(true);
                    FB.Mobile.SetAutoLogAppEventsEnabled(true);
                    SetInit();
                    FB.LogAppEvent("AdImpression");
                }
                else
                    print("Couldn't initialize");
            },
            isGameShown =>
            {
                //if (!isGameShown)
                //    Time.timeScale = 0;
                //else
                //    Time.timeScale = 1;
                onHidenUnity(isGameShown);
            });
        }
        else
        {
            FB.ActivateApp();
            FB.Mobile.SetAdvertiserTrackingEnabled(true);
            FB.Mobile.SetAutoLogAppEventsEnabled(true);
            FB.LogAppEvent("AdImpression");
        }
    }

    public void LogMetaAdImpression(string adPlacement, string adFormat, float adRevenue = 0f)
    {
        var parameters = new Dictionary<string, object>
    {
        { "ad_platform", "AudienceNetwork" },
        { "ad_placement", adPlacement },
        { "ad_format", adFormat }
    };

        if (adRevenue > 0f)
            parameters["ad_revenue"] = adRevenue;

        FB.LogAppEvent("AdImpression", parameters: parameters);
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook is Login!");
            string s = "client token" + FB.ClientToken + "User Id" + AccessToken.CurrentAccessToken.UserId + "token string" + AccessToken.CurrentAccessToken.TokenString;
        }
        else
        {
            Debug.Log("Facebook is not Logged in!");
        }
        DealWithFbMenus(FB.IsLoggedIn);
    }

    void onHidenUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void FBLogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        //permissions.Add("user_friends");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    public void CallLogout()
    {
        StartCoroutine("FBLogout");
    }
    IEnumerator FBLogout()
    {
        FB.LogOut();
        while (FB.IsLoggedIn)
        {
            print("Logging Out");
            yield return null;
        }
        print("Logout Successful");
        FB_useerDp.sprite = null;
        FB_userName.text = "";
    }

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            Debug.Log("the raw" + result.RawResult);
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            foreach (var dict in friendsList)
            {
                GameObject go = Instantiate(friendstxtprefab);
                go.GetComponent<Text>().text = ((Dictionary<string, object>)dict)["name"].ToString();
                go.transform.SetParent(GetFriendsPos.transform, false);
            }
        });
    }

    public void FacebookSharefeed()
    {
        string url = "https://scontent.fhan17-1.fna.fbcdn.net/v/t39.30808-6/419853053_1117561269420435_4528163874479796420_n.jpg?_nc_cat=100&ccb=1-7&_nc_sid=c42490&_nc_ohc=tQP7qTe48qMAX-S3y18&_nc_ht=scontent.fhan17-1.fna&oh=00_AfDw_s3CbKdLMhrU72CADV0kSHOzanIHVQ2iL0vQPKSElQ&oe=65AA2B20";
        FB.ShareLink(
            new Uri(url),
            "Checkout Unity Share",
            "Mint.Mon",
            null,
            ShareCallback);
    }

    private static void ShareCallback(IShareResult result)
    {
        Debug.Log("ShareCallback");
        //SpentCoins(2, "sharelink");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);
    }

    // Start is called before the first frame update

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {

            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("Facebook is Login!");
                // Panel_Add.SetActive(true);
            }
            else
            {
                Debug.Log("Facebook is not Logged in!");
            }
            DealWithFbMenus(FB.IsLoggedIn);
        }
    }



    void DealWithFbMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
        else
        {
        }
    }

    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            string name = "" + result.ResultDictionary["first_name"];
            FB_userName.text = name;
            Debug.Log("" + name);
        }

        else
        {
            Debug.Log(result.Error);
        }
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            Debug.Log("Profile Pic");
            FB_useerDp.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    public static void SpentCoins(int coins, string item)
    {
        return;
        // setup parameters
        var param = new Dictionary<string, object>();
        param[AppEventParameterName.ContentID] = item;
        // log event
        FB.LogAppEvent(AppEventName.SpentCredits, (float)coins, param);
    }
}