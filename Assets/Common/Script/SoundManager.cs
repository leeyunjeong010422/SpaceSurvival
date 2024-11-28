using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonBehaviour<SoundManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    private void Awake()
    {
        RegisterSingleton(this);
    }

    public void PlayBGM(AudioClip clip, float volumeScale = 1f)
    {
        bgmSource.clip = clip;
        bgmSource.volume = volumeScale;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 3D가 아닌 효과음 출력 대행
    public void PlaySFX(AudioClip clip, float volumeScale = 1f) => sfxSource.PlayOneShot(clip, volumeScale);
}
