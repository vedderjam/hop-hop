﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GameObject")]
    public GameObject gameOverPanel;
    public GameObject startButton;
    public GameObject difficultyButton;

    [Header("Text")]
    public Text scoreText;
    public Text hiScoreText;
    public Text coinsText;
    public Text finalScoreText;
    public Text rewardText;
    public List<Image> difficultyLevelImages;

    private void Awake()
    {
        EventBroker.BirdScored += BirdScored;
        EventBroker.GameOver += GameOver;
        EventBroker.StartIdling += Idle;
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;
    }

    private void Start()
    {
        SetUITexts();
    }

    private void SetUITexts()
    {
        scoreText.text = $"Score: {GameControl.Score}";
        hiScoreText.text = $"Hi-Score: {GameControl.Record}";
        coinsText.text = $"Coins: {GameControl.Coins}";
    }

    private void StartPlaying()
    {
        startButton.SetActive(false);
        scoreText.text = "Score: 0";
    }

    private void OnDestroy()
    {
        EventBroker.BirdScored -= BirdScored;
        EventBroker.GameOver -= GameOver;
        EventBroker.StartIdling -= Idle;
        EventBroker.StartPlaying -= StartPlaying;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        UpdateHighScore();
        UpdateHighScoreColor();
        UpdateCoins();
        UpdateRewardText();
    }

    private void Idle()
    {
        gameOverPanel.SetActive(false);
        startButton.SetActive(true);
        difficultyButton.SetActive(true);
    }

    private void BirdScored()
    {
        scoreText.text = $"Score: {GameControl.Score}";
    }

    private void ChangeDifficultyLevel()
    {
        UpdateHighScore();
        UpdateHighScoreColor();
        UpdateDifficultyButtonsColor();
    }

    private void UpdateHighScore()
    {
        hiScoreText.text = $"Hi-Score: {GameControl.Record}";
    }

    private void UpdateHighScoreColor()
    {
        switch (GameControl.Instance.currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                hiScoreText.color = Color.green;
                break;
            case Difficulty.Normal:
                hiScoreText.color = Color.yellow;
                break;
            case Difficulty.Hard:
                hiScoreText.color = Color.red;
                break;
            default:
                hiScoreText.color = Color.white;
                break;
        }
    }

    private void UpdateDifficultyButtonsColor()
    {
        switch (GameControl.Instance.currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                difficultyLevelImages[0].color = Color.green;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.white;
                break;
            case Difficulty.Normal:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.yellow;
                difficultyLevelImages[2].color = Color.white;
                break;
            case Difficulty.Hard:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.red;
                break;
            default:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.white;
                break;
        }
    }

    private void UpdateCoins()
    {
        coinsText.text = $"Coins: {GameControl.Coins}";
    }

    private void UpdateRewardText()
    {
        finalScoreText.text = scoreText.text;
        rewardText.text =  $"Reward: {GameControl.Reward} coins";
    }
}
