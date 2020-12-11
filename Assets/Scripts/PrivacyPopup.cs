using System;
using System.Collections;
using UnityEngine;
using DeltaDNA;

public class PrivacyPopup : MonoBehaviour
{
    private DateTime retryUntil;
    private const int retryDurationSeconds = 60;
    private bool waiting = false;
    private bool privacyPopUpVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        retryUntil = DateTime.Now.AddSeconds(retryDurationSeconds);
        
    }

    private void Update()
    {
        if (retryUntil > DateTime.Now)
        {
            if (!waiting)
            {
                StartCoroutine(WaitForDDNA());
            }
        }
        else if (!privacyPopUpVisible)
        {
            Debug.Log("Destroying PrivacyPolicy Popup - Retry Duration Exceeded");
            Destroy(gameObject);
        }
    }

    IEnumerator WaitForDDNA()
    {
        waiting = true;
        // Check if SDK is running
        if (DDNA.Instance.HasStarted)
        {
            CheckPrivacyPopup();
        }
        else
        {

            //Print the time of when the function is first called.
            Debug.Log("Privacy Policy Popup Waiting for deltaDNA SDK : " + Time.time.ToString("0"));

            //yield on a new YieldInstruction that waits for 1 seconds.
            yield return new WaitForSeconds(1);
            waiting = false;
        }

    }
     

    private void CheckPrivacyPopup()
    {
        // Make DeltaDNA Decision Point Request looking for Privacy PopUp
        // The campaign logic on deltaDNA will ensure it is only displayed
        // to the player once.
        Debug.Log("Checking for Privacy Popup");

        DDNA.Instance.EngageFactory.RequestImageMessage("privacyPolicyCheck", null, (imageMessage) => {

            // Check we got an engagement with a valid Privacy Policy image message.
            if (imageMessage != null)
            {
                Debug.Log("Engage returned a valid privacy policy image message.");

                // Show the image as soon as the background
                // and button images have been downloaded.
                imageMessage.OnDidReceiveResources += () => {
                    Debug.Log("Image Message loaded resources.");
                    privacyPopUpVisible = true; 
                    imageMessage.Show();
                };

                // Add a handler for the 'dismiss' action.
                imageMessage.OnDismiss += (ImageMessage.EventArgs obj) => {
                    Debug.Log("Image Message dismissed by " + obj.ID);
                    RecordPrivacyPolicyPopupEvent(0);
                    DestroyPrivacyPolicyObject();
                };

                // Add a handler for the 'action' action.
                imageMessage.OnAction += (ImageMessage.EventArgs obj) => {
                    Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);
                    RecordPrivacyPolicyPopupEvent(1);
                    DestroyPrivacyPolicyObject();
                };

                // Download the image message resources.
                imageMessage.FetchResources();
            }
            else
            {
                Debug.Log("Engage didn't return an image message.");
                DestroyPrivacyPolicyObject();
            }
        });
    }



    private void DestroyPrivacyPolicyObject()
    {
        // Destroy the privacy policy object. 
        Debug.Log("Destroying PrivacyPolicy Popup");
        Destroy(gameObject);
    }



    private void RecordPrivacyPolicyPopupEvent(int policyAccepted)
    {
        // Record an event that indicates that the privacy policy has been accepted
        // or not. The privacyPolicyTimestamp will be stored as a metric
        // for analysis or segmentation for future policy updates.
        GameEvent privacyPolicyEvent = new GameEvent("privacyPolicyViewed")
            .AddParam("privacyPolicyAccepted", policyAccepted)
            .AddParam("privacyPolicyTimestamp", DateTime.UtcNow);

            DDNA.Instance.RecordEvent(privacyPolicyEvent);

        Debug.Log("Sending privacyPolicyViewed Event");
    }      
}