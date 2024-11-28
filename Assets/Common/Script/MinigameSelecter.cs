using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MinigameSelecter : SingletonScriptable<MinigameSelecter>
{
    public enum Minigame
    {
        COIN_COLLECTER,
        ICY_SLIDING,
        LAST_MAN,
        TPS_SHOOTING,
        TTANG,
    }

    [System.Serializable]
    public struct MinigameData
    {
        public Minigame minigame;
        public MiniGameInfoUI1 infoPrefab;
        public string sceneName;
        public int buildIndex;
    }

    /// <summary>
    /// 직렬화를 위한 필드
    /// </summary>
    [SerializeField] MinigameData[] sceneDatas;

    /// <summary>
    /// 스크립트로 사용할 씬 데이터
    /// </summary>
    private Dictionary<Minigame, MinigameData> sceneDataDic;

    public MinigameData GetData(Minigame minigame) => sceneDataDic[minigame];

    #region randomSelect
    /// <summary>
    /// 랜덤 맵 선택용 리스트
    /// </summary>
    private List<Minigame> minigames;

    /// <summary>
    /// 무작위로 정렬된 미니게임 목록에서 하나의 값을 꺼낸다
    /// </summary>
    public Minigame PopRandom()
    {
        if (minigames.Count == 0)
        {
            Debug.Log($"랜덤 리스트가 비어있어 초기화됨");
            ResetRandomList();
        }

        Minigame pop = minigames.Last();
        minigames.RemoveAt(minigames.Count - 1);
        return pop;
    }

    public int PopRandomSceneIndex() => sceneDataDic[PopRandom()].buildIndex;

    /// <summary>
    /// 무작위로 정렬된 미니게임 목록을 초기화한다
    /// </summary>
    public void ResetRandomList()
    {
        // 미니게임 목록을 무작위 정렬
        minigames = new List<Minigame>(sceneDataDic.Keys).OrderBy(_ => Random.value).ToList();
    }

    private void OnEnable() => Init();

    private void Init()
    {
        Debug.Log("MinigameSceneData SO 초기화");

        sceneDataDic = new Dictionary<Minigame, MinigameData>(sceneDatas.Length << 1);

        foreach (var sceneData in sceneDatas)
        {
            sceneDataDic.Add(sceneData.minigame, sceneData);
        }

        ResetRandomList();
    }
    #endregion

#if UNITY_EDITOR
    [ContextMenu("씬 이름으로부터 Index 설정(Play Mode)")]
    private void SetBuildIndexByName()
    {
        if (false == Application.isPlaying)
        {
            Debug.LogWarning("이 기능은 PlayMode에서만 작동함");
            return;
        }

        List<Minigame> duplicateCheck = new List<Minigame>(sceneDatas.Length);

        for (int i = 0; i < sceneDatas.Length; i++)
        {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneDatas[i].sceneName);
            if (-1 == buildIndex)
            {
                Debug.LogWarning($"씬 이름({sceneDatas[i].sceneName})이 잘못되었거나 Build 대상으로 등록되지 않음");
                continue;
            }

            if (duplicateCheck.Contains(sceneDatas[i].minigame))
            {
                Debug.LogWarning($"중복된 키 값({sceneDatas[i].minigame})이 존재함");
                continue;
            }
            duplicateCheck.Add(sceneDatas[i].minigame);

            sceneDatas[i].buildIndex = buildIndex;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Init();
    }
#endif // UNITY_EDITOR

    private void OnDisable()
    {
        Debug.Log("MinigameSceneData SO 비활성화");
    }
}
