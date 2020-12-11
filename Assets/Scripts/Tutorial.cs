using System.Collections;
using System.IO;
using UnityEngine;
using DeltaDNA;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();
        
    }

    public void Reset()
    {

        // Reset UserID so we can see Privacy behaviour for a new player.
        Debug.Log("Resetting UserID");
        
        /*
        string cache = Application.temporaryCachePath + "/deltadna/image_messages/";

        if (Directory.Exists(cache))
        {
            Debug.Log("Deleting Image Cache");
        }*/
        DDNA.Instance.ClearPersistentData();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
        #else
                Application.Quit();
        #endif
    }
}