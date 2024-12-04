using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameInfoUI1 : MonoBehaviour
{
    [SerializeField] Image fadeImg;
    [SerializeField] float fadeSecond;

    [ContextMenu("FadeIn")]
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    [ContextMenu("FadeOut")]
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    IEnumerator FadeInCoroutine()
    {
        Color fadeColor = new Color(1f, 1f, 1f, 0.5f);
        while (fadeColor.a > 0)
        {
            fadeColor.a -= 0.5f * (1 / fadeSecond) * Time.deltaTime;
            fadeImg.color = fadeColor;
            yield return null;
        }
    }
    IEnumerator FadeOutCoroutine()
    {
        Color fadeColor = new Color(1f, 1f, 1f, 0f);
        while (fadeColor.a < 0.5f)
        {
            fadeColor.a += 0.5f * (1 / fadeSecond) * Time.deltaTime;
            fadeImg.color = fadeColor;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
