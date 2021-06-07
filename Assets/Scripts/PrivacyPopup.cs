using System;
using System.Collections;
using UnityEngine;
using DeltaDNA;

public class PrivacyPopup : MonoBehaviour
{

    enum State
    {
        WAITING, DONE
    }

    private State privacy;

    // Start is called before the first frame update
    void Start()
    {
        DDNA.Instance.OnImageCachePopulated += CheckPrivacyPopup;
        privacy = State.WAITING;               
    }

    private void CheckPrivacyPopup()
    {
        // Make DeltaDNA Decision Point Request looking for Privacy PopUp
        // The campaign logic on deltaDNA will ensure it is only displayed to the player once.
        if (privacy == State.WAITING)
        {
            privacy = State.DONE;
            Debug.Log("Checking for Privacy Popup");

            DDNA.Instance.EngageFactory.RequestImageMessage("privacyPolicyCheck", null, (imageMessage) =>
            {

                // Check we got an engagement with a valid Privacy Policy image message.
                if (imageMessage != null)
                {
                    Debug.Log("Engage returned a valid privacy policy image message.");

                    // Show the image as soon as the background and button images have been downloaded.
                    imageMessage.OnDidReceiveResources += () =>
                    {
                        Debug.Log("Image Message loaded resources.");                        
                        DDNA.Instance.OnImageCachePopulated -= CheckPrivacyPopup;
                        imageMessage.Show();


                    };

                    // Add a handler for the 'dismiss' action.
                    imageMessage.OnDismiss += (ImageMessage.EventArgs obj) =>
                    {
                        Debug.Log("Image Message dismissed by " + obj.ID);
                        RecordPrivacyPolicyPopupEvent(0);                        
                    };

                    // Add a handler for the 'action' action.
                    imageMessage.OnAction += (ImageMessage.EventArgs obj) =>
                    {
                        Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);
                        RecordPrivacyPolicyPopupEvent(1);                        
                    };

                    // Download the image message resources.
                    imageMessage.FetchResources();
                }
                else
                {
                    Debug.Log("Engage didn't return an image message.");
                    DDNA.Instance.OnImageCachePopulated -= CheckPrivacyPopup;
                    DestroyPrivacyPolicyObject();
                }
            });
        }
    }



    private void DestroyPrivacyPolicyObject()
    {
        // Destroy the privacy policy object.
        if (gameObject != null)
        {
            Debug.Log("Destroying PrivacyPolicy Popup");            
            Destroy(gameObject);
        }
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