using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMixerSettingPanel : MonoBehaviour
{
    [SerializeField] AudioClip testClip;

    [Header("자식 UI")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle masterMuteToggle;
    [SerializeField] Toggle bgmMuteToggle;
    [SerializeField] Toggle sfxMuteToggle;

    private AudioGroup targetGroup;
    private bool valueChanged = false;

    private void Start()
    {
        masterSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.MASTER);
        bgmSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.BGM);
        sfxSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.SFX);

        masterSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.MASTER; });
        bgmSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.BGM; });
        sfxSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.SFX; });

        masterMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.MASTER, value));
        bgmMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.BGM, value));
        sfxMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.SFX, value));
    }

    private void Update()
    {
        if (valueChanged && Input.GetMouseButtonUp(0))
            PointerUp();
    }

    private void SetAndPlay(AudioGroup audioGroup, float value)
    {
        Debug.Log($"설정값: {value}");
        GameManager.Sound.SetMixerScale(audioGroup, value);
        GameManager.Sound.PlayTestSound(audioGroup, testClip);
    }

    public void PointerUp()
    {
        // 마우스를 뗄 때에만 설정 및 재생
        // onValueChanged에서 하면 드래그시 연달아 재생됨(너무 귀 아픔)
        valueChanged = false;

        Slider target;
        switch (targetGroup)
        {
            case AudioGroup.MASTER:
                target = masterSlider;
                break;
            case AudioGroup.BGM:
                target = bgmSlider;
                break;
            case AudioGroup.SFX:
                target = sfxSlider;
                break;
            default:
                Debug.LogWarning("정의되지 않은 AudioGroup");
                return;
        }
        SetAndPlay(targetGroup, target.value);
    }
}
