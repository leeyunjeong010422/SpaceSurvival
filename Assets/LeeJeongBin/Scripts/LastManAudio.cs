using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastManAudio : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource attackSoundSource;
    [SerializeField] AudioClip checkpointSound;
    [SerializeField] AudioClip attackSound;

    private void Start()
    {
        if (photonView.IsMine)
        {
            SoundManager.Instance.PlayBGM(backgroundMusic.clip);
            backgroundMusic.volume = SoundManager.Instance.GetMixerScale(AudioGroup.BGM);

            backgroundMusic.mute = SoundManager.Instance.GetMute(AudioGroup.BGM);
        }
    }

    [PunRPC]
    private void CheckpointSound()
    {
        if (checkpointSound != null && attackSoundSource != null)
        {
            attackSoundSource.PlayOneShot(checkpointSound, SoundManager.Instance.GetMixerScale(AudioGroup.SFX));
        }
    }

    // Killing3에서 호출될 메서드
    public void OnAttack()
    {
        if (attackSound != null && attackSoundSource != null)
        {
            attackSoundSource.PlayOneShot(attackSound, SoundManager.Instance.GetMixerScale(AudioGroup.SFX));
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
