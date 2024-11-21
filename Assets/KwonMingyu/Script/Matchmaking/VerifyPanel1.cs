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
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("인증메일 전송 취소/실패 됨.");
                return;
            }

            Debug.Log("인증메일 전송 성공.");
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

        Debug.Log("이메일 인증 성공 메인메뉴로.");
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
