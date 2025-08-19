using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//using Unity.Burst.CompilerServices;

namespace DVAH
{
    public class RateController : MonoBehaviour
    {
        [SerializeField] float _delayTimeShowNoButton;
        [SerializeField] GameObject _noThankButton;
        [SerializeField] Transform _starManTrans;
        [SerializeField] List<GameObject> _starManager = new List<GameObject>();

        int _starRate = 5;



        Coroutine _waitShowNoThank;

        private void Awake()
        {
            for (int i = 0; i < _starManTrans.transform.childCount; i++)
            {
                _starManTrans.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                _starManager.Add(_starManTrans.transform.GetChild(i).GetChild(0).gameObject);
            }
        }


        private void OnEnable()
        {
            _waitShowNoThank = StartCoroutine(WaitShowNoThank());
        }


        private void OnDisable()
        {
            if(_waitShowNoThank != null)
                StopCoroutine(_waitShowNoThank);
        }

        IEnumerator WaitShowNoThank()
        {
            yield return new WaitForSeconds(_delayTimeShowNoButton);
            _noThankButton.SetActive(true);
        }

        public void ClickChoose(Transform t)
        {


            for (int i = 0; i < _starManager.Count; i++)
            {
                if (i <= t.GetSiblingIndex())
                    _starManager[i]?.SetActive(true);
                else
                    _starManager[i]?.SetActive(false);
            }



            _starRate = t.GetSiblingIndex();


        }

        public void submitRate()
        {
            if (_starRate >= 4)
            {
#if UNITY_ANDROID
                OpenUrl();
                PlayerPrefs.SetInt(CONSTANT.RATE_CHECK, 1);
#elif UNITY_EDITOR
            this.gameObject.SetActive(false);
#endif
            }
        }


        public void OpenUrl()
        {
#if UNITY_ANDROID

#if UNITY_EDITOR
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#else
        Application.OpenURL("market://details?id=" + Application.identifier);
#endif
#endif
#if UNITY_IOS
            Application.OpenURL("https://itunes.apple.com/app/id"+Application.identifier);
#endif
        }


    }

}