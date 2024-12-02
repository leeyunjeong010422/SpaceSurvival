using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserSettingData : SingletonScriptable<UserSettingData>
{
    public SettingData Data => data;
    private SettingData data;

    [SerializeField] SettingData defaultData; // 기본값은 SO 애셋으로 입력

    [System.Serializable]
    public class SettingData
    {
        public string email = string.Empty;
        public float[] soundScale = new float[SoundManager.AudioGroupCount];
    }

    private string path;

    private void OnEnable()
    {
#if UNITY_EDITOR
        // 에디터 모드에서는 (프로젝트 폴더)/Temp 에 생성
    path = $"./Temp/UserSetting.json";
#else
    path = $"{Application.persistentDataPath}/UserSetting.json";
#endif
    }

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
        else
        {
            // 기본값 직렬화 복사
            data = JsonUtility.FromJson<SettingData>(JsonUtility.ToJson(defaultData, true));
        }
    }
}
