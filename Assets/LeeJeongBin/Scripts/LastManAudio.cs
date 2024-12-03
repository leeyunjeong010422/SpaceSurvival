using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastManAudio : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioSource attackSoundSource;
    [SerializeField] AudioClip checkpointSound;
    [SerializeField] AudioClip attackSound;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(backgroundMusic);
    }

    [PunRPC]
    private void CheckpointSound()
    {
        if (checkpointSound != null)
        {
            SoundManager.Instance.PlaySFX(checkpointSound);
            //attackSoundSource.PlayOneShot(checkpointSound, SoundManager.Instance.GetMixerScale(AudioGroup.SFX));
        }
    }

    // Killing3에서 호출될 메서드
    public void OnAttack()
    {
        if (attackSound != null)
        {
            SoundManager.Instance.PlaySFX(attackSound);
            //attackSoundSource.PlayOneShot(attackSound, SoundManager.Instance.GetMixerScale(AudioGroup.SFX));
        }
    }

    // PlayerController3에서 호출될 메서드
    public void TriggerCheckPointRPC()
    {
        // 마스터 클라이언트만 소리를 재생
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("CheckpointSound", RpcTarget.All);
        }
    }
}
