using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSampleButtons : MonoBehaviour
{
    [Header("테스트 대상")]
    [SerializeField] GameSceneTester2 gameSceneTester;
    [SerializeField] MiniGameScore scoreManager;

    [Header("Child UI")]
    [SerializeField] Button testStartButton;
    [SerializeField] Button addScoreButton;

    private void Start()
    {
        testStartButton.onClick.AddListener(gameSceneTester.TestStart);
        testStartButton.onClick.AddListener(() => testStartButton.interactable = false);
        addScoreButton.onClick.AddListener(() => scoreManager.AddScore(3));
        addScoreButton.interactable = false;
    }

    public void WhenTestStarted()
    {
        addScoreButton.interactable = true;
        testStartButton.interactable = false;
    }
}
