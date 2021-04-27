using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int blueScore = 0;
    [SerializeField] private int orangeScore = 0;
    [SerializeField] TextMeshProUGUI gameTime = null;
    [SerializeField] TextMeshProUGUI blueScoreText = null;
    [SerializeField] TextMeshProUGUI blueScoreUnderlay = null;
    [SerializeField] TextMeshProUGUI orangeScoreText = null;
    [SerializeField] TextMeshProUGUI orangeScoreUnderlay = null;
    [SerializeField] TextMeshProUGUI startCountdown = null;
    [SerializeField] public bool gameActive = false;
    [SerializeField] private bool countdownActive = true;

    private float timer = 3f;
    private int timeDisplay = 0;

    private float gameTimer = 301f;

    private void Update()
    {
        if (countdownActive)
        {
            timer -= Time.deltaTime;
            timeDisplay = Mathf.CeilToInt(timer % 60);
            startCountdown.text = timeDisplay + "";
            if(timer <= 0)
            {
                StartCoroutine(Go());
                countdownActive = false;
                gameActive = true;
                StartPlayers();
            }
        }
        if (gameActive)
        {
            gameTimer -= Time.deltaTime;
            float minutes = Mathf.FloorToInt(gameTimer / 60);
            float seconds = Mathf.FloorToInt(gameTimer % 60);
            gameTime.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    IEnumerator Go()
    {
        startCountdown.text = "GO!";
        yield return new WaitForSeconds(1f);
        startCountdown.gameObject.SetActive(false);
    }

    public void BlueScored()
    {
        blueScore++;
        blueScoreText.text = blueScore + "";
        blueScoreUnderlay.text = blueScore + "";
        gameActive = false;
    }

    public void OrangeScored()
    {
        orangeScore++;
        orangeScoreText.text = orangeScore + "";
        orangeScoreUnderlay.text = orangeScore + "";
        gameActive = false;
    }

    public void StartCountdown()
    {
        timer = 3f;
        countdownActive = true;
        startCountdown.gameObject.SetActive(true);
        StopPlayers();
    }

    public void StartPlayers()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.GameStarted();
        }
    }

    public void StopPlayers()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.GameStopped();
        }
    }
}
