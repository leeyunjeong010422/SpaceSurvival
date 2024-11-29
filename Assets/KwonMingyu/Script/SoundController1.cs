using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundController1 : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] TMP_Text bgmText;

    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxText;
    private void Start()
    {
        bgmSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.BGM);
        bgmText.text = bgmSlider.value.ToString("0.0");

        sfxSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.SFX);
        sfxText.text = sfxSlider.value.ToString("0.0");
    }

    public void OnChangeBgmSlider()
    {
        bgmText.text = bgmSlider.value.ToString("0.0");
        GameManager.Sound.SetMixerScale(AudioGroup.BGM, bgmSlider.value);
    }
    public void OnChangeSfxSlider()
    {
        sfxText.text = sfxSlider.value.ToString("0.0");
        GameManager.Sound.SetMixerScale(AudioGroup.SFX, sfxSlider.value);
        Debug.Log(GameManager.Sound.GetMixerScale(AudioGroup.SFX));
    }
}
