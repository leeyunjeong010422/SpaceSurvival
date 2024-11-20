using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyPanel1 : MonoBehaviour
{
    private void OnEnable()
    {
        SendVerifyMail();
    }
    private void SendVerifyMail()
    {
        FirebaseUser user = BackendManager1.Auth.CurrentUser;
        if (user == null)
            return;

        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Email sent successfully.");
            coroutine = StartCoroutine(CheckVerifyRoutine());
        });

    }
    Coroutine coroutine;
    IEnumerator CheckVerifyRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(3f);

        while (!BackendManager1.Auth.CurrentUser.IsEmailVerified)
        {
            BackendManager1.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted) return;
            });
            yield return wait;
        }
        gameObject.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }
    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
