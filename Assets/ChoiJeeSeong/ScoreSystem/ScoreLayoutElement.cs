using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreLayoutElement : MonoBehaviour
{
    public string NicknameText { set => nicknameText.text = value; }
    public string ScoreText { set => scoreText.text = value; }

    [SerializeField] TMP_Text nicknameText;
    [SerializeField] TMP_Text scoreText;
}
