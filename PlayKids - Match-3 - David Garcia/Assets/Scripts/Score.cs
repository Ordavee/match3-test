using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class Score : MonoBehaviour
{
    public static int scoreCount;
    private int roundCount;
    public Text scoreText, roundText;

    public static bool roundActive;
    public GameObject roundBackground, roundIcon;

    void Start()
    {
        roundBackground.SetActive(false);
        roundIcon.SetActive(false);
        scoreCount = 0;
    }

    void Update()
    {
        scoreText.text = scoreCount.ToString();
        roundText.text = roundCount.ToString();

        if(scoreCount >= 100)
        {
            roundActive = true;
            roundBackground.SetActive(true);
            roundIcon.SetActive(true);
            StartCoroutine(NextRound());
            scoreCount = 0;
            roundCount++;
        }
    }

    #region COROUTINA ENCONTRA MATCHES
    IEnumerator NextRound()
    {
        yield return new WaitForSeconds(3f);
        roundActive = false;
        roundBackground.SetActive(false);
        roundIcon.SetActive(false);
    }
    #endregion
}
