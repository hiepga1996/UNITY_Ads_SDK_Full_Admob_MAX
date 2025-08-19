using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using SimpleJSON;
using System;
using UnityEngine.UI;

namespace DVAH
{
    public class DVAH3rdLib : Singleton<DVAH3rdLib>
    {
        [SerializeField] bool _isDontDestroyOnLoad = false;
        [SerializeField] GameObject _noInternetDebug;
        MasterLib _masterLib;
        int _devTapCount = 0;

        [Header("------------LIB-------------")]
        [SerializeField]
        bool _isAutoInit = false;
        public bool isAutoInit => _isAutoInit;
        [SerializeField]
        bool _isInitByOrder = false;
        public bool isInitByOrder => _isInitByOrder;

        [SerializeField] List<GameObject> _childLibs;
        public List<GameObject> ChildLibs => _childLibs;

        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (_isDontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);

#if UNITY_EDITOR
            this.CheckFirebaseJS();
            this.CheckFirebaseXml();
#endif
        }

        void Update()
        {
            //if (_noInternetDebug)
            //{
            //    _noInternetDebug.SetActive(!AdManager.Instant.CheckInternetConnection());
            //}
        }

        public void GotoMarket()
        {
            Application.OpenURL("market://details?id="+Application.identifier);
        }

        public void CloseApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void OnDrawGizmosSelected()
        {
            if (!_masterLib)
                _masterLib = this.GetComponentInChildren<MasterLib>();

            //CheckFirebaseJS();
        }

        public void GetSubLib()
        {
            if (!_masterLib)
                _masterLib = this.GetComponentInChildren<MasterLib>();
            for (int i = 0; i < _masterLib.transform.childCount; i++)
            {
                Transform Ichild = _masterLib.transform.GetChild(i);
                if (Ichild.GetComponent<IChildLib>() == null)
                    DestroyImmediate(Ichild.gameObject);
            }

            this._childLibs = new List<GameObject>();
            IChildLib[] childLib = this.GetComponentsInChildren<IChildLib>();

            for (int i = 0; i < childLib.Count(); i++)
            {

                _childLibs.Add(_masterLib.transform.GetChild(i).gameObject);
            }
           
        }

        public bool CheckFirebaseJS()
        {

            string[] files = Directory.GetFiles(Application.dataPath, "*.json*", SearchOption.AllDirectories)
                                .Where(f => f.EndsWith("google-services.json")).ToArray();
            if (files.Length == 0)
            {
                Debug.LogError("==>Project doesnt contain google-services.json. Firebase may not work!!!!!<==");
                return false;
            }

            if (files.Length > 1)
            {
                Debug.LogError(string.Format("==>Project contain more than one file google-services.json: \n{0} \n{1} . Firebase may not work wrong!!!!!<==", files[0], files[1]));
                return false;
            }

            return true;
        }

        public string CheckFirebaseXml()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "*google-services.xml", SearchOption.AllDirectories).ToArray();
            if (files.Length == 1)
            {
                return files[0];
            }

            Debug.LogError("==>Project error google-services.xml. Firebase may not work wrong!!!!!<==");
            return null;
        }


    }
}

