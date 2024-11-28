using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioGroup { MASTER, BGM, SFX }

public class SoundManager : SingletonBehaviour<SoundManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    private static readonly string[] paramNames = { "Master", "BGM", "SFX" };

    private void Awake()
    {
        RegisterSingleton(this);
    }

    /// <summary>
    /// BGM을 재생
    /// </summary>
    /// <param name="clip">BGM으로 재생할 오디오 클립</param>
    /// <param name="volumeScale">음량 배율</param>
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

    /// <summary>
    /// 효과음을 재생, 거리의 영향을 받는 효과음은 매니저를 통하지 말고 따로 재생 필요
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volumeScale">음량 배율</param>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f) => sfxSource.PlayOneShot(clip, volumeScale);

    /// <summary>
    /// 해당 오디오 그룹에 대한 전역 음량 스케일을 설정<br/>
    /// Playe 메서드의 매개변수 volumeScale과는 별개
    /// </summary>
    /// <param name="group">그룹</param>
    /// <param name="scale">음량</param>
    public void SetMixerScale(AudioGroup group, float scale)
    {
        if (false == mixer.SetFloat(paramNames[(int)group], scale))
        {
            Debug.LogWarning("잘못된 AudioGroup");
        }
    }

    public float GetMixerScale(AudioGroup group)
    {
        if (false == mixer.GetFloat(paramNames[(int)group], out float value))
        {
            Debug.LogWarning("잘못된 AudioGroup");
        }
        return value;
    }
}
