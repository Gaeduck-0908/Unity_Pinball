using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if EASY_MOBILE
using EasyMobile;
#endif

public class AndroidBackButtonHandler : MonoBehaviour
{
    #if UNITY_ANDROID && EASY_MOBILE
    [Header("Exit Confirmation Dialog")]
    public string title = "Exit Game";
    public string message = "Are you sure you want to exit?";
    public string yesButton = "Yes";
    public string noButton = "No";

    void Update()
    {
        // Exit on Android Back button
        if (Input.GetKeyUp(KeyCode.Escape))
        {   

            NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert(
                                            title,
                                            message,
                                            yesButton, 
                                            noButton
                                        );

            if (alert != null)
            {
                alert.OnComplete += (int button) =>
                {
                    switch (button)
                    {
                        case 0: // Yes
                            Application.Quit();
                            break;
                        case 1: // No
                            break;
                    }
                };
            }     
        }
    }
    #endif
}
