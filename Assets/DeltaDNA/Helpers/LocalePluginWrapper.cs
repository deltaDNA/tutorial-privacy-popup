using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;



namespace DeltaDNA
{
    public class LocalePluginWrapper
    {

        //private static AndroidJavaObject javaClass;
        private AndroidJavaObject javaClass;
        // Start is called before the first frame update
        void Start()
        {

        }

        public string GetAndroidLocale()
        {
            javaClass = new AndroidJavaObject("com.example.ddnalocalehelper.LocaleClass");
            return javaClass.CallStatic<string>("getAndroidCountryCode");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
