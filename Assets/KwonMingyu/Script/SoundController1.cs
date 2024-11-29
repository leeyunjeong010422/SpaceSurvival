using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundController1 : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] TMP_Text bgmText;
    [SerializeField] Button bgmMuteButton;

    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxText;
    [SerializeField] Button sfxMuteButton;

    [SerializeField] Sprite soundOnImg;
    [SerializeField] Sprite soundMuteImg;
    private void OnEnable()
    {
        bgmSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.BGM);
        bgmText.text = bgmSlider.value.ToString("0.0");

        sfxSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.SFX);
        sfxText.text = sfxSlider.value.ToString("0.0");

        bgmMuteButton.onClick.AddListener(BgmMuteTogle);
        sfxMuteButton.onClick.AddListener(SfxMuteTogle);
    }
    private void OnDisable()
    {
        bgmMuteButton.onClick.RemoveAllListeners();
        sfxMuteButton.onClick.RemoveAllListeners();
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
    }

    public void BgmMuteTogle()
    {
        Image image = bgmMuteButton.GetComponent<Image>();
        if (image.sprite == soundOnImg)
        {
            GameManager.Sound.SetMute(AudioGroup.BGM, true);
            image.sprite = soundMuteImg;
            bgmSlider.interactable = false;
            bgmText.text = "X";
        }
        else
        {
            GameManager.Sound.SetMute(AudioGroup.BGM, false);
            image.sprite = soundOnImg;
            bgmSlider.interactable = true;
            bgmText.text = bgmSlider.value.ToString("0.0");
        }
    }
    public void SfxMuteTogle()
    {
        Image image = sfxMuteButton.GetComponent<Image>();
        if (image.sprite == soundOnImg)
        {
            GameManager.Sound.SetMute(AudioGroup.SFX, true);
            image.sprite = soundMuteImg;
            sfxSlider.interactable = false;
            sfxText.text = "X";
        }
        else
        {
            GameManager.Sound.SetMute(AudioGroup.SFX, false);
            image.sprite = soundOnImg;
            sfxSlider.interactable = true;
            sfxText.text = sfxSlider.value.ToString("0.0");
        }
    }
}
