using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LoginPanel1 : MonoBehaviour
{
    [SerializeField] TMP_InputField emaillInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] VerifyPanel1 verifyPanel;

    private void OnEnable()
    {
        // 저장된 이메일 불러오기
        // 저장된 파일이 없다면 기본값(string.Empty)
        GameManager.UserSetting.LoadSetting();
        emaillInput.text = GameManager.UserSetting.Data.email;
    }

    public void Login()
    {
        PopUp1.Instance.PopUpOpen(true, "로그인 중");
        string email = emaillInput.text;
        string password = passwordInput.text;
        BackendManager1.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                PopUp1.Instance.PopUpOpen(false, "로그인 실패");
                return;
            }
            if (BackendManager1.Auth.CurrentUser.IsEmailVerified)
            {
                // 로그인 성공시 이메일 저장
                GameManager.UserSetting.Data.email = email;
                GameManager.UserSetting.SaveSetting();

                // TODO: 중복 로그인 방지 기능
                BackendManager1.Instance.userUidDataRef = BackendManager1.Database.RootReference.Child("UserData").Child(BackendManager1.Auth.CurrentUser.UserId);

                // 현재 로그인한 디바이스를 DB에 등록 후 값 변경시 비교
                DatabaseReference deviceDataRef = GameManager.Backend.userUidDataRef.Child("Device");
                deviceDataRef.SetValueAsync(SystemInfo.deviceUniqueIdentifier);
                deviceDataRef.ValueChanged += TEST_DeviceDataRef_ValueChanged;


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

    static private void TEST_DeviceDataRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"current:{SystemInfo.deviceUniqueIdentifier}, old:{e.Snapshot.Value.ToString()}");
    }

    static 

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
