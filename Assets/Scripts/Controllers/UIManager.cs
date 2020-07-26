using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GameObject")]
    public GameObject difficultyPanel;
    public GameObject gameOverPanel;
    public GameObject mainUIPanel;
    public GameObject birdsSelectionPanel;
    public GameObject startButton;
    public GameObject difficultyButton;
    public GameObject birdsSelectionButton;
    
    [Header("Birds selection")]
    public BirdHouse birdHouse;
    public Transform birdsButtonsParent;
    public Button[] birdsButtons;

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
        EventBroker.NotEnoughCoins += NotEnoughCoins;
        EventBroker.BirdPurchased += BirdPurchased;
        EventBroker.BirdSelected += BirdSelected;
    }

    private void Start()
    {
        SetUITexts();
        SetBirdsSelectionButtons();
    }

    private void SetUITexts()
    {
        scoreText.text = $"Score: {GameControl.Score}";
        hiScoreText.text = $"Hi-Score: {GameControl.Record}";
        coinsText.text = $"Coins: {GameControl.Coins}";
    }

    private void SetBirdsSelectionButtons()
    {
        birdsButtons = birdsButtonsParent.GetComponentsInChildren<Button>();

        int numberOfBirds = birdHouse.birdInfos.Capacity;
        int count = 0;
        foreach(var button in birdsButtons)
        {
            if(count < numberOfBirds)
            {
                button.interactable = true;
                Debug.Assert(button.interactable);
                var birdInfo = birdHouse.birdInfos[count];
                button.image.sprite = birdInfo.sprite;
                var texts = button.GetComponentsInChildren<Text>(true);
                texts[0].text = birdInfo.name;

                if (!birdInfo.purchased)
                {
                    texts[1].text = birdInfo.prize.ToString();
                    texts[1].gameObject.SetActive(true);
                    button.transform.GetChild(2).gameObject.SetActive(true); // activate the coin image
                }

                int i = count; // Dear closure
                button.onClick.AddListener(delegate{ 
                    GameControl.Instance.TryToBuyAndOrSelectBird(i); 
                });
                count++;
            }
        }
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
        EventBroker.NotEnoughCoins -= NotEnoughCoins;
        EventBroker.BirdPurchased -= BirdPurchased;
        EventBroker.BirdSelected -= BirdSelected;
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
        birdsSelectionButton.SetActive(true);
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

    private void NotEnoughCoins()
    {
        Debug.Log("Not enough coins");
    }
    
    private void BirdPurchased(int index)
    {
        UpdateCoins();
        HideBirdPrize(index);
    }
    
    private void HideBirdPrize(int index)
    {
        var children = birdsButtons[index].GetComponentsInChildren<RectTransform>(true);
        children[2].gameObject.SetActive(false); // Prize text
        children[3].gameObject.SetActive(false); // Coin Image
    }

    private void BirdSelected()
    {
        ToMainUIPanel();
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

    private void ToMainUIPanel()
    {
        difficultyPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        birdsSelectionPanel.SetActive(false);

        mainUIPanel.SetActive(true);
    }
}
