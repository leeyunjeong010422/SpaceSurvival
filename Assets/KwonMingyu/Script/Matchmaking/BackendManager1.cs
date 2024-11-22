using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;

public enum UserDatas1
{
    name, level
}

public class UserData1
{
    public string name;
    public long level;
}

public class BackendManager1 : MonoBehaviour
{
    public static BackendManager1 Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App => Instance.app;

    private FirebaseAuth auth;
    public static FirebaseAuth Auth => Instance.auth;

    private FirebaseDatabase database;
    public static FirebaseDatabase Database => Instance.database;

    private DatabaseReference userUidDataRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;
                Debug.Log("파이어베이스 연결 성공");
                userUidDataRef = Database.RootReference.Child("UserData").Child(Auth.CurrentUser.UserId);
            }
            else
            {
                Debug.LogError($"파이어베이스 연결 실패: {task.Result}");
                app = null;
                auth = null;
                database = null;
            }
        });
    }
    /// <summary>
    /// 유저 DB에서 정보를 가져오는 함수
    /// 지금은 포톤 닉네임 설정에 사용된다.
    /// </summary>
    public async Task<UserData1> GetPlayerData()
    {
        UserData1 userData = new();
        await userUidDataRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("값 가져오기 취소/실패 됨");
                userData = null;
                return;
            }

            if (task.Result.Value == null)
            {
                userData.name = $"Player {Random.Range(1000, 9999)}";
                userData.level = 1;

                string json = JsonUtility.ToJson(userData);
                userUidDataRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(x => x);
            }
            else
            {
                userData.name = task.Result.Child("name").Value.ToString();
                userData.level = (long)task.Result.Child("level").Value;
            }
        });
        return userData;
    }
    /// <summary>
    /// 유저 DB name, Photon NickName 변경 함수.
    /// </summary>
    public void SetPlayerName(string name)
    {
        userUidDataRef.Child("name").SetValueAsync(name).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("이름 변경 취소/실패 됨");
                return;
            }
        });
        Debug.Log($"닉네임 변경 성공{PhotonNetwork.LocalPlayer.NickName} => {name}");
        PhotonNetwork.LocalPlayer.NickName = name;
    }
}
