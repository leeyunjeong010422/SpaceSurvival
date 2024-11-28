using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownText : MonoBehaviour
{
    public UnityEvent OnCountdownComplete;

    [SerializeField] string completeString = "시작!";
    [SerializeField] AudioClip countClip;
    [SerializeField] AudioClip completeClip;

    private TMP_Text countdonwText;

    private int count;
    private Coroutine countdownRoutine;

    private void Awake()
    {
        countdonwText = GetComponent<TMP_Text>();
    }

    public void CountdownStart(int count)
    {
        this.gameObject.SetActive(true);
        this.count = count;
        countdownRoutine = StartCoroutine(CountdonwRoutine());
    }

    private IEnumerator CountdonwRoutine()
    {
        YieldInstruction waitOneSecond = new WaitForSeconds(1f);

        // 카운트다운 진행
        while (count > 0)
        {
            countdonwText.text = count.ToString();
            if (countClip != null)
            {
                GameManager.Sound.PlaySFX(countClip);
            }

            count--;
            yield return waitOneSecond;
        }

        // 카운트다운 완료 시점
        OnCountdownComplete?.Invoke();

        countdonwText.text = completeString;
        if (completeClip != null)
        {
            GameManager.Sound.PlaySFX(completeClip);
        }

        // 1초 후 숨기기
        yield return waitOneSecond;
        this.gameObject.SetActive(false);
    }
}
