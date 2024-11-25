using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Killing3 : MonoBehaviourPun
{
    [SerializeField] private GameObject attackRange;
    private HashSet<GameObject> targetsInRange = new HashSet<GameObject>();

    private void Start()
    {
        if (attackRange.GetComponent<Collider>() == null)
        {
            Debug.LogError("콜라이더 없음");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (GameObject target in targetsInRange)
            {
                if ((target.CompareTag("Player") || target.CompareTag("AI")) && target != this.gameObject)
                {
                    Debug.Log($"대상 {target.name} 제거");

                    PhotonView targetPhotonView = target.GetComponent<PhotonView>();
                    if (targetPhotonView != null)
                    {
                        if (targetPhotonView.IsMine || PhotonNetwork.IsMasterClient)
                        {
                            Debug.Log($"네트워크에서 {target.name} 제거");
                            PhotonNetwork.Destroy(target);
                        }
                        else
                        {
                            Debug.Log($"로컬에서 {target.name} 제거");
                            Destroy(target);
                        }
                    }
                    else
                    {
                        Debug.Log($"로컬에서 {target.name} 제거");
                        Destroy(target);
                    }

                    GameOver3.Instance.OnPlayerDeath(target.name);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            targetsInRange.Add(other.gameObject);
            Debug.Log($"{other.name}가 공격 범위에 들어왔습니다.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            targetsInRange.Remove(other.gameObject);
            Debug.Log($"{other.name}가 공격 범위에서 나갔습니다.");
        }
    }
}
