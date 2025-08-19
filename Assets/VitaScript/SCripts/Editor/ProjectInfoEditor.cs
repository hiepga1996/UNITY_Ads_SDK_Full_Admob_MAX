/* * * * *
 * A simple helper for Deneb checklist
 * ------------------------------
 * Written by nhh1681996
 *
 * 19-08-2025
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) Hiep Hoang Nguyen (nhh1681996)
 *
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GoogleMobileAds.Editor;
using System;
using DVAH;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

class ProjectInfoEditor : EditorWindow
{
    Vector2 scrollPos;
    bool isShowKeyStorePass = false, isShowAliasPass = false;

    GoogleMobileAdsSettings gg = null;

    DVAH.AdManager adManager = null;
    AppLovinSettings max = null;

    FireBaseManager fireBaseManager;


    string fbAppID = null, fbClientToken = null ,fbKeyStore = null;
    static EditorWindow wnd;
    GUIStyle TextRedStyles, TextGreenStyles, ButtonTextStyles;

    int numberAddOpenAdID = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("3rdLib/Checklist Deneb",priority = 0)]
    public static void InitWindowEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        wnd = GetWindow<ProjectInfoEditor>();
        wnd.titleContent = new GUIContent("3rdLib - Deneb version!");

        if (EditorBuildSettings.scenes.Count() > 0)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
        }
    }

    void OnGUI()
    {
        if (TextRedStyles == null)
        {
            TextRedStyles = new GUIStyle(EditorStyles.label);
            TextRedStyles.normal.textColor = Color.red;
        }

        if (TextGreenStyles == null)
        {
            TextGreenStyles = new GUIStyle(EditorStyles.label);
            TextGreenStyles.normal.textColor = Color.green;
        }

        if (ButtonTextStyles == null)
        {
            ButtonTextStyles = new GUIStyle(GUI.skin.button);
            ButtonTextStyles.normal.textColor = Color.green;
        }

      
        if (!wnd || EditorApplication.isPlaying)
        {
            Close();
            GUIUtility.ExitGUI();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scene:", TextGreenStyles);
        if(EditorGUILayout.DropdownButton(content: new GUIContent(EditorSceneManager.GetActiveScene().path), FocusType.Passive) && EditorBuildSettings.scenes.Count() > 0)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                AddMenuItemForScenes(menu,scene.path , scene, SceneManager.GetActiveScene().path.Equals(scene.path));
            }
            
            
            menu.ShowAsContext();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(wnd.position.width), GUILayout.Height(wnd.position.height-20));
        if (!adManager)
        {
            adManager = GameObject.FindObjectOfType<DVAH.AdManager>();
            if (adManager)
            {
                numberAddOpenAdID = adManager.OpenAdUnitIDs.Count;
            }
        }
        #region EDITOR
        EditorGUILayout.LabelField("Build Version:", TextGreenStyles);

        PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Version Code", PlayerSettings.Android.bundleVersionCode);

        EditorGUILayout.BeginHorizontal();
        PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
        PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);
        EditorGUILayout.EndHorizontal();

        PlayerSettings.bundleVersion = EditorGUILayout.TextField("App Version", PlayerSettings.bundleVersion);

        EditorGUILayout.BeginHorizontal();
        string applicationIdentifier = EditorGUILayout.TextField("Package Name", PlayerSettings.applicationIdentifier);
        
        EditorGUILayout.LabelField("Package name should in form \"com.X.Y\" other can cost a build error!", TextRedStyles);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, applicationIdentifier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        PlayerSettings.Android.useCustomKeystore = EditorGUILayout.Toggle("Custom KeyStore", PlayerSettings.Android.useCustomKeystore);
        if (PlayerSettings.Android.useCustomKeystore)
        {
            if (EditorGUILayout.LinkButton("Select"))
            {
                string path = EditorUtility.OpenFilePanel("Select keystore file", "", "keystore");
                if (path.Length != 0)
                {
                    PlayerSettings.Android.keystoreName = path;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("KeyStore Path:                      "+ PlayerSettings.Android.keystoreName);

            KeyStoreInfo();
        }
        else
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("KeyStore Path:                      Debug keystore!!!");
        }
        
        #endregion

        #region ADJUST

       
        #endregion

        #region GOOGLE ADS SETTING
       

        if (gg)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Google:", TextGreenStyles);
            gg.GoogleMobileAdsAndroidAppId = EditorGUILayout.TextField("Android AD ID", gg.GoogleMobileAdsAndroidAppId);
            if (adManager)
            {
                adManager.paid_ad_revenue = EditorGUILayout.TextField("Event Paid AD", adManager.paid_ad_revenue);
                PrefabUtility.RecordPrefabInstancePropertyModifications(adManager);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(gg);
            EditorUtility.SetDirty(gg);
        }
        else
        {
            string[] ggSetting = UnityEditor.AssetDatabase.FindAssets("t:GoogleMobileAdsSettings");
            if (ggSetting.Length != 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ggSetting[0]);
                gg = UnityEditor.AssetDatabase.LoadAssetAtPath<GoogleMobileAdsSettings>(path);
            }
            else
            {
                EditorGUILayout.LabelField("Can not find GoogleMobileAdsSettings!");
            }
        }
        #endregion

      
        #region APPLOVIN
       
        if (max != null)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("AppLovin:", TextGreenStyles);
            max.SdkKey = EditorGUILayout.TextField("MaxSdk key", max.SdkKey);
            if (adManager)
                adManager.MaxSdkKey = max.SdkKey;
            if (gg != null)
            {
                max.AdMobAndroidAppId = gg.GoogleMobileAdsAndroidAppId;
                EditorGUILayout.LabelField("Android AD ID:                       " + max.AdMobAndroidAppId);
            }
            else
            {
                max.AdMobAndroidAppId = EditorGUILayout.TextField("Android AD ID", max.AdMobAndroidAppId);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(max);
            EditorUtility.SetDirty(max);
        }
        else
        {
            string[] maxSetting = UnityEditor.AssetDatabase.FindAssets("t:AppLovinSettings");
            if (maxSetting.Length != 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(maxSetting[0]);
                max = UnityEditor.AssetDatabase.LoadAssetAtPath<AppLovinSettings>(path);
            }
            else
            {
                Debug.LogError("[3rdLib]:Can not find MaxSdkSetting!");
            }
        }

        #endregion

        #region AD ID SETTING
        if (adManager)
        {
    
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("AD IDs:", TextGreenStyles);
            adManager.BannerAdUnitID = EditorGUILayout.TextField("Banner ID", adManager.BannerAdUnitID);
            adManager.InterstitialAdUnitID = EditorGUILayout.TextField("Inter ID", adManager.InterstitialAdUnitID);
            adManager.RewardedAdUnitID = EditorGUILayout.TextField("Reward ID", adManager.RewardedAdUnitID);

            EditorGUILayout.BeginHorizontal();
            numberAddOpenAdID = EditorGUILayout.IntField("AppOpen AD ID number", numberAddOpenAdID);

            if (numberAddOpenAdID > adManager.OpenAdUnitIDs.Count)
            {
                adManager.OpenAdUnitIDs.AddRange(new string[numberAddOpenAdID - adManager.OpenAdUnitIDs.Count]); 
            }

            if (numberAddOpenAdID < adManager.OpenAdUnitIDs.Count)
            {
                int numberRemove = adManager.OpenAdUnitIDs.Count - numberAddOpenAdID;
                adManager.OpenAdUnitIDs.RemoveRange(numberAddOpenAdID, numberRemove); 
            }

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < adManager.OpenAdUnitIDs.Count; i++)
            {
                adManager.OpenAdUnitIDs[i] = EditorGUILayout.TextField("Ad ID " + (i + 1), adManager.OpenAdUnitIDs[i]);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal(); 

            PrefabUtility.RecordPrefabInstancePropertyModifications(adManager);
        }
#endregion

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Check google-services.json"))
        {
            MenuEditor.CheckFirebaseJson();
        }
        if (GUILayout.Button("Check google-services.xml"))
        {
            MenuEditor.FixGoogleXml();
        }

        if (GUILayout.Button("Fix AndroidManifest FbID"))
        {
            EditorUtility.DisplayDialog("Attention Pleas?",
                  "This will change your AndroidManifest for match FBID!!", "Ok");
            MenuEditor.FixAndroidManifestFB();
        }
         
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
       
        if (GUILayout.Button("Build"))
        {
            if (!PlayerSettings.applicationIdentifier.StartsWith("com."))
            {
                EditorUtility.DisplayDialog("Attention Pleas?",
                   "Your package name should in form \"com.\". This can make you can't build your project, consider change it ASAP!!", "Ok");
            }

            if (PlayerSettings.applicationIdentifier.Split('.').Count() < 3)
            {
                EditorUtility.DisplayDialog("Attention Pleas?",
                   "Your package name is not in format 'com.X.Y' . This can make you can't build your project, consider change it ASAP!!", "Ok");
            }

            EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Close"))
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Close();
            GUIUtility.ExitGUI();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void KeyStoreInfo()
    {
        EditorGUILayout.BeginHorizontal();
        if(!isShowKeyStorePass)
            PlayerSettings.Android.keystorePass = EditorGUILayout.PasswordField("Keystore Pass", PlayerSettings.Android.keystorePass);
        else
            PlayerSettings.Android.keystorePass = EditorGUILayout.TextField("Keystore Pass", PlayerSettings.Android.keystorePass);
        isShowKeyStorePass = EditorGUILayout.Toggle("Show", isShowKeyStorePass);
        EditorGUILayout.EndHorizontal();

        PlayerSettings.Android.keyaliasName = EditorGUILayout.TextField("Keystore Alias", PlayerSettings.Android.keyaliasName);
        EditorGUILayout.BeginHorizontal();
        if (!isShowAliasPass)
            PlayerSettings.Android.keyaliasPass = EditorGUILayout.PasswordField("Keystore Pass", PlayerSettings.Android.keyaliasPass);
        else
            PlayerSettings.Android.keyaliasPass = EditorGUILayout.TextField("Keystore Pass", PlayerSettings.Android.keyaliasPass);
        isShowAliasPass = EditorGUILayout.Toggle("Show", isShowAliasPass);
        EditorGUILayout.EndHorizontal();
    }

    void AddMenuItemForScenes(GenericMenu menu, string menuPath, EditorBuildSettingsScene value, bool isSelected = false)
    {
        // the menu item is marked as selected if it matches the current value of m_Color
        menu.AddItem(new GUIContent(menuPath), isSelected, OnDropBoxSceneItemClick, value);
    }

    void OnDropBoxSceneItemClick(object item)
    {
        EditorSceneManager.OpenScene(((EditorBuildSettingsScene)item).path);
    }

    void OnDropBoxAdjustItemClick(object item)
    {
   
    }
}
#endif