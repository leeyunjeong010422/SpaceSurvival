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

            if (SoundManager.Instance.GetMute(AudioGroup.BGM))
            {
                backgroundMusic.mute = true;
            }
            else
            {
                backgroundMusic.mute = false;
            }
        }
    }

    [PunRPC]
    private void CheckpointSound()
    {
        if (backgroundMusic != null && checkpointSound != null)
        {
            backgroundMusic.PlayOneShot(checkpointSound);
        }
    }

    // Killing3에서 호출될 메서드
    public void OnAttack()
    {
        if (photonView.IsMine && attackSound != null)
        {
            attackSoundSource.PlayOneShot(attackSound);
        }
    }

    // PlayerController3에서 호출될 메서드
    public void TriggerCheckPointRPC()
    {
        photonView.RPC("CheckpointSound", RpcTarget.All);
    }
}
