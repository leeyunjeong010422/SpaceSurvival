using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SighUpPanel1 : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField passwordConfirmInputField;

    [SerializeField] Button sighUpButton;
    [SerializeField] VerifyPanel1 verifyPanel;

    public void SighUp()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;
        string confirm = passwordConfirmInputField.text;

        if (email.IsNullOrEmpty() || password != confirm) return;

        BackendManager1.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("계정 생성 취소/실패.");
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"계정 생성 성공: {result.User.DisplayName} ({result.User.UserId})");
            gameObject.SetActive(false);
            verifyPanel.gameObject.SetActive(true);
        });

    }
}
