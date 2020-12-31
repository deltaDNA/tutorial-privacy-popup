using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA;

public class Tutorial : MonoBehaviour
{
    // Determines if we start the DDNA SDK with a new userID or use the existing one
    // This is for testing purposes you should NEVER do this in a real app!
    public bool RandomiseUserID;

    #region UI Stuff
    // UI Debug console will be used to let player see what the game and SDKs are doing.
    private GameObject txtConsole;
    private int numConsoleLines = 20;
    private List<string> console = new List<string>();
    

    void Awake()
    {        
        // This will allow us to display Unity Debug.Log in the UI
        Application.logMessageReceived += PrintToConsole;
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        txtConsole = GameObject.Find("txtConsole");

        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);

        // Decide whether to use existing userID or a new random one
        // This is just for testing, you wouldn';t do this in a real app!
        string userID = null;
        if (RandomiseUserID) userID = System.Guid.NewGuid().ToString();

        DDNA.Instance.StartSDK(userID);
        
        UpdateHud();
    }


    #region UI Stuff
    private void UpdateHud()
    {
        // Prints some handy things on the UI
        var txtUserID = GameObject.Find("txtUserID");
        txtUserID.GetComponent<Text>().text = "UserID : " + DDNA.Instance.UserID;

        var txtSdkVersion = GameObject.Find("txtSdkVersion");
        txtSdkVersion.GetComponent<Text>().text = "DDNA Sdk Version: " + Settings.SDK_VERSION;

        var txtUnityVersion = GameObject.Find("txtUnityVersion");
        txtUnityVersion.GetComponent<Text>().text = "Unity Version : " + Application.unityVersion;

        var txtLocale = GameObject.Find("txtLocale");
        txtLocale.GetComponent<Text>().text = "Locale : " + ClientInfo.Locale;
    }


   
    private void PrintToConsole(string logString, string stackTrace, LogType type)
    {
        // Prints Debug info to the UI
        console.Add(string.Format("{0}::{1}\n", System.DateTime.Now.ToString("h:mm:ss tt"), logString));
        if (console.Count > numConsoleLines)
        {
            console.RemoveRange(0, console.Count - numConsoleLines);
        }
        txtConsole.GetComponent<Text>().text = "";
        console.ForEach(i => txtConsole.GetComponent<Text>().text += i);
    }
    #endregion
}