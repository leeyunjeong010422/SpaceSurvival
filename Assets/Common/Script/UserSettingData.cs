using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserSettingData : SingletonScriptable<UserSettingData>
{
    public SettingData Data => data;
    [SerializeField] SettingData data;

    [System.Serializable]
    public class SettingData
    {
        public string email = string.Empty;
        public float bgmScale = 1f;
        public float sfxScale = 1f;
    }


#if UNITY_EDITOR
    // 에디터 모드에서는 (프로젝트 폴더)/Temp 에 생성
    private readonly string path = $"./Temp/UserSetting.json";
#else
    private readonly string path = $"{Application.persistentDataPath}/UserSetting.json";
#endif

    [ContextMenu("Test")]
    public void SaveSetting()
    {
        string jData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jData);
    }

    public void LoadSetting()
    {
        if (File.Exists(path))
        {
            string jData = File.ReadAllText(path);
            data = JsonUtility.FromJson<SettingData>(jData);
            Debug.Log(path);
        }
    }
}
