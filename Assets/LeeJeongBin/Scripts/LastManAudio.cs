using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastManAudio : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource backgroundMusic;  // 배경 음악을 재생할 AudioSource
    [SerializeField] AudioSource attackSoundSource; // 공격 소리를 재생할 AudioSource
    [SerializeField] AudioClip checkpointSound;    // 체크포인트 도달 시 재생될 소리
    [SerializeField] AudioClip attackSound;        // 공격 시 재생될 소리

    private void Start()
    {
        // 배경 음악 반복 재생
        if (photonView.IsMine)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    // 체크포인트 도달 시 모든 클라이언트에게 오디오 전달
    [PunRPC]
    private void CheckpointSound()
    {
        if (backgroundMusic != null && checkpointSound != null)
        {
            // 체크포인트 사운드 재생
            backgroundMusic.PlayOneShot(checkpointSound);
        }
    }

    // 공격 오디오 (Killing3에서 호출될 메서드)
    public void OnAttack()
    {
        // 공격 소리 재생 (자기 자신만 들음)
        if (photonView.IsMine && attackSound != null)
        {
            attackSoundSource.PlayOneShot(attackSound);  // 공격 소리 출력
        }
    }

    // TriggerCheckPointRPC 메서드 (PlayerController3에서 호출될 메서드)
    public void TriggerCheckPointRPC()
    {
        // 체크포인트 도달 시 모든 클라이언트에게 오디오 전달
        photonView.RPC("CheckpointSound", RpcTarget.All);
    }
}
