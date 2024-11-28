using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundController1 : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] TMP_Text bgmText;

    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxText;
    private void OnEnable()
    {
        //bgmSlider.value = GameManager.Sound.volume
    }

    public void OnChangeBgmSlider()
    {
        bgmText.text = bgmSlider.value.ToString("0.0");
        //GameManager.Sound.BgmVolume
    }
    public void OnChangeSfxSlider()
    {
        sfxText.text = sfxSlider.value.ToString("0.0");
        //GameManager.Sound.BgmVolume
    }
}
