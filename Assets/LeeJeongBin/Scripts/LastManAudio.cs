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
        photonView.RPC("CheckpointSound", RpcTarget.All);
    }
}
