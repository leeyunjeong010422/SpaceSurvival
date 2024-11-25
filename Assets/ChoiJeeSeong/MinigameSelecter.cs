using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSelecter : SingletonScriptable<MinigameSelecter>
{
    public enum Minigame
    {
        COIN_COLLECTER,
        ICY_SLIDING,
        LAST_MAN,
        TPS_SHOOTING,
    }

    [System.Serializable]
    private struct SceneData
    {
        public Minigame minigame;
        public string sceneName;
    }

    /// <summary>
    /// 직렬화를 위한 필드
    /// </summary>
    [SerializeField] SceneData[] sceneDatas;

    /// <summary>
    /// 스크립트로 사용할 씬 데이터
    /// </summary>
    private Dictionary<Minigame, Scene> sceneDataDic;
    public Dictionary<Minigame, Scene> SceneDataDic => sceneDataDic;

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
        minigames = new List<Minigame>(sceneDataDic.Keys);
        
        // 미니게임 목록을 무작위 정렬
        minigames.OrderBy(_ => Random.value);
    }

    private void OnEnable()
    {
        if (false == Application.isPlaying)
            return;

        sceneDataDic = new Dictionary<Minigame, Scene>(sceneDatas.Length << 1);

        // 씬 이름 검사, 중복 검사 및 인덱스 가져오기
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            Scene scene = SceneManager.GetSceneByName(sceneDatas[i].sceneName);
            if (false == scene.IsValid())
            {
                Debug.LogWarning($"씬 이름({sceneDatas[i].sceneName})이 잘못되었거나 Build 대상으로 등록되지 않음");
                return;
            }

            if (sceneDataDic.ContainsKey(sceneDatas[i].minigame))
            {
                Debug.LogWarning($"중복된 키 값({sceneDatas[i].minigame})이 존재함");
                return;
            }

            sceneDataDic.Add(sceneDatas[i].minigame, scene);
        }

        ResetRandomList();
    }

    private void OnDisable()
    {
        Debug.Log("MinigameSceneData SO disabled");
    }
}
