using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] LayoutGroup scoreLayoutGroup;
    [SerializeField] ScoreLayoutElement layoutElementPrefab;

    private Dictionary<int, ScoreLayoutElement> scoreLayoutElements = new Dictionary<int, ScoreLayoutElement>();

    public void AddPlayer(Player newPlayer)
    {
        if(scoreLayoutElements.ContainsKey(newPlayer.ActorNumber))
        {
            Debug.LogWarning("이미 추가된 플레이어");
            return;
        }

        ScoreLayoutElement instance = Instantiate(layoutElementPrefab, scoreLayoutGroup.transform);
        scoreLayoutElements.Add(newPlayer.ActorNumber, instance);
        instance.NicknameText = newPlayer.NickName;
        instance.ScoreText = "0";
    }

    public void RemovePlayer(Player removedPlayer)
    {
        if (false == scoreLayoutElements.ContainsKey(removedPlayer.ActorNumber))
        {
            Debug.LogWarning("이미 제거되었거나 존재하지 않는 플레이어");
            return;
        }

        Destroy(scoreLayoutElements[removedPlayer.ActorNumber].gameObject);
        scoreLayoutElements.Remove(removedPlayer.ActorNumber);
    }

    public void UpdateScoreUI(Player player, int score)
    {
        if (false == scoreLayoutElements.ContainsKey(player.ActorNumber))
        {
            Debug.LogWarning("이미 제거되었거나 존재하지 않는 플레이어");
            return;
        }

        scoreLayoutElements[player.ActorNumber].ScoreText = score.ToString();
    }
}
