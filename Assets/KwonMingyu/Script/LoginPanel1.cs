using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginPanel1 : MonoBehaviour
{
    [SerializeField] TMP_InputField emaillInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] VerifyPanel1 verifyPanel;

    public void Login()
    {
        string email = emaillInput.text;
        string password = passwordInput.text;
        BackendManager1.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("로그인 성공");
            if (BackendManager1.Auth.CurrentUser.IsEmailVerified)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                verifyPanel.gameObject.SetActive(true);
            }
        });
    }
}
