using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundController1 : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] TMP_Text bgmText;
    [SerializeField] Button bgmMuteButton;
    private Image bgmMuteButtonImg;

    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxText;
    [SerializeField] Button sfxMuteButton;
    private Image sfxMuteButtonImg;

    [SerializeField] Sprite soundOnImg;
    [SerializeField] Sprite soundMuteImg;

    private void Awake()
    {
        bgmMuteButtonImg = bgmMuteButton.GetComponent<Image>();
        sfxMuteButtonImg = sfxMuteButton.GetComponent<Image>();
    }
    private void OnEnable()
    {
        bgmMuteButton.onClick.AddListener(BgmMuteTogle);
        sfxMuteButton.onClick.AddListener(SfxMuteTogle);

        if (GameManager.Sound.GetMute(AudioGroup.BGM))
            MuteActive(AudioGroup.BGM);
        else
            MuteDisable(AudioGroup.BGM);

        if (GameManager.Sound.GetMute(AudioGroup.SFX))
            MuteActive(AudioGroup.SFX);
        else
            MuteDisable(AudioGroup.SFX);
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
        if (GameManager.Sound.GetMute(AudioGroup.BGM))
        {
            MuteDisable(AudioGroup.BGM);
        }
        else
        {
            MuteActive(AudioGroup.BGM);
        }
    }
    public void SfxMuteTogle()
    {
        if (GameManager.Sound.GetMute(AudioGroup.SFX))
        {
            MuteDisable(AudioGroup.SFX);
        }
        else
        {
            MuteActive(AudioGroup.SFX);
        }
    }
    private void MuteActive(AudioGroup audioGroup)
    {
        if (audioGroup == AudioGroup.BGM)
        {
            GameManager.Sound.SetMute(audioGroup, true);
            bgmMuteButtonImg.sprite = soundMuteImg;
            bgmSlider.interactable = false;
            bgmText.text = "X";
        }
        else
        {
            GameManager.Sound.SetMute(audioGroup, true);
            sfxMuteButtonImg.sprite = soundMuteImg;
            sfxSlider.interactable = false;
            sfxText.text = "X";
        }
    }
    private void MuteDisable(AudioGroup audioGroup)
    {
        if (audioGroup == AudioGroup.BGM)
        {
            GameManager.Sound.SetMute(audioGroup, false);
            bgmMuteButtonImg.sprite = soundOnImg;
            bgmSlider.interactable = true;
            float value = GameManager.Sound.GetMixerScale(audioGroup);
            bgmText.text = value.ToString("0.0");
            bgmSlider.value = value;
        }
        else
        {
            GameManager.Sound.SetMute(audioGroup, false);
            sfxMuteButtonImg.sprite = soundOnImg;
            sfxSlider.interactable = true;
            float value = GameManager.Sound.GetMixerScale(audioGroup);
            sfxText.text = value.ToString("0.0");
            sfxSlider.value = value;
        }
    }
}
