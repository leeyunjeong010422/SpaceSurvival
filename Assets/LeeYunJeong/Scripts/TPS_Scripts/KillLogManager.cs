using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using UnityEngine;
using System.Collections;

public class KillLogManager : MonoBehaviourPun
{
    [SerializeField] private TMP_Text killLogText;
    [SerializeField] private int maxLogCount = 3; // 화면에 표시할 최대 로그 수
    [SerializeField] private float logDuration = 5f;

    private Queue<string> logMessages = new Queue<string>(); // 로그 메시지 큐

    // 킬 로그 추가 함수 (모든 클라이언트 동기화)
    [PunRPC]
    public void AddKillLog(string killer, string victim)
    {
        if (victim == PhotonNetwork.LocalPlayer.NickName)
        {
            victim = $"<color=red>{victim}</color>";
        }
        else if (killer == PhotonNetwork.LocalPlayer.NickName)
        {
            killer = $"<color=red>{killer}</color>";
        }
        string logMessage = $"Kill: {killer} -> {victim}";

        // 로그를 큐에 추가
        logMessages.Enqueue(logMessage);

        // 최대 로그 수 초과 시 이전 로그 삭제
        if (logMessages.Count > maxLogCount)
        {
            logMessages.Dequeue();
        }

        UpdateKillLogUI();

        StartCoroutine(RemoveLogRoutine(logMessage));
    }

    private void UpdateKillLogUI()
    {
        killLogText.text = string.Join("\n", logMessages);
    }

    // 킬 로그를 네트워크 전체에 전파하는 함수
    public void AddKillLogNetwork(string killer, string victim)
    {
        // 모든 클라이언트에 킬 로그 전달
        photonView.RPC("AddKillLog", RpcTarget.All, killer, victim);
    }

    // 로그 일정 시간 뒤에 제거
    private IEnumerator RemoveLogRoutine(string logMessage)
    {
        yield return new WaitForSeconds(logDuration);

        // 큐에 해당 로그가 아직 있다면 제거
        if (logMessages.Count > 0 && logMessages.Peek() == logMessage)
        {
            logMessages.Dequeue();
            UpdateKillLogUI();
        }
    }
}
