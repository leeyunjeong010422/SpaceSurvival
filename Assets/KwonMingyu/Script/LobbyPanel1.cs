using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel1 : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry1 roomEntryPrefab;

    private Dictionary<string, RoomEntry1> roomDictionary = new Dictionary<string, RoomEntry1>();

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    // 서버의 Room 상태가 변경될 때 변경된 Room들을 매개변수로 호출
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // 변경된 모든 Room을 순회
        foreach (RoomInfo info in roomList)
        {
            // 해당 Room이 삭제, 비활성화, 닫힘 상태
            if (info.RemovedFromList || !info.IsVisible || !info.IsOpen)
            {
                // 딕셔너리에 해당 방이 없다면 넘김
                if (!roomDictionary.ContainsKey(info.Name)) continue;

                // 해당 룸을 Ui에서 삭제
                Destroy(roomDictionary[info.Name].gameObject);

                // 해당 룸을 딕셔너리에서 삭제
                roomDictionary.Remove(info.Name);
            }
            // 해당 룸이 딕셔너리에 없다면 생성 후 방 정보를 Ui에 출력
            else if (!roomDictionary.ContainsKey(info.Name))
            {
                RoomEntry1 roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomDictionary.Add(info.Name, roomEntry);
                roomEntry.SetRoomInfo(info);
            }
            // 해당 룸이 딕셔너리에 있다면 정보만 변경
            else
            {
                RoomEntry1 roomEntry = roomDictionary[info.Name];
                roomEntry.SetRoomInfo(info);
            }
        }
    }
    public void ClearRoom()
    {
        foreach (var item in roomDictionary.Keys)
        {
            Destroy(roomDictionary[item].gameObject);
        }
        roomDictionary.Clear();
    }
}
