using Firebase.Extensions;
using Photon.Pun;
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
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("로그인 취소/실패 됨.");
                return;
            }
            if (BackendManager1.Auth.CurrentUser.IsEmailVerified)
            {
                // TODO: 중복 로그인 방지 기능
                // 게임 진행은 문제가 없지만 닉네임을 서버에서 받기 때문에
                Debug.Log("로그인 성공 마스터 서버로 연결");
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log("로그인 성공 인증 화면으로 이동");
                verifyPanel.gameObject.SetActive(true);
            }
        });
    }
}
