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
    public GameObject gamePausedPanel;
    public GameObject pauseButton;
    public GameObject newRecord;

    [Header("Birds selection")]
    public BirdHouse birdHouse;
    public Transform birdsButtonsParent;
    public Button[] birdsButtons;

    [Header("Text")]
    public Text scoreText;
    public Text hiScoreText;
    public Text coinsText;
    public Text selectionScreenCoinsText;
    public Text finalScoreText;
    public Text rewardText;
    public Text moreRewardText;

    [Header("Animators")]
    public Animator coinsTextAnimator;
    public Animator coinsImageAnimator;
    public Animator rewardTextAnimator;
    public Animator hiScoreTextAnimator;

    [Space]
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
        EventBroker.GamePaused += PauseGame;
        EventBroker.GameResumed += ResumeGame;
        EventBroker.EarnedRewardedAd += EventBroker_EarnedRewardedAd;
    }

    private void Start()
    {
        SetUITexts();
        SetBirdsSelectionButtons();

        #if DEVELOPMENT_BUILD || UNITY_EDITOR
            pauseButton.SetActive(true);
        #endif
    }

    private void SetUITexts()
    {
        scoreText.text = $"{GameControl.Score}";
        hiScoreText.text = $"RECORD:{GameControl.Record}";
        coinsText.text = GameControl.Coins.ToString();
        selectionScreenCoinsText.text = GameControl.Coins.ToString();
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
        scoreText.text = "0";
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
        EventBroker.GamePaused -= PauseGame;
        EventBroker.GameResumed -= ResumeGame;
        EventBroker.EarnedRewardedAd -= EventBroker_EarnedRewardedAd;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);

        UpdateRewardText();
    }

    private void Idle()
    {
        UpdateHighScore();
        UpdateHighScoreColor();
        UpdateCoins();
        gameOverPanel.SetActive(false);
        startButton.SetActive(true);
        difficultyButton.SetActive(true);
        birdsSelectionButton.SetActive(true);
    }

    private void BirdScored()
    {
        scoreText.text = $"{GameControl.Score}";
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

    public void RaiseGamePausedEvent()
    {
        EventBroker.CallGamePaused();
    }

    private void PauseGame()
    {
        print("UIManager PauseGame");
        mainUIPanel.SetActive(false);
        gamePausedPanel.SetActive(true);
    }

    public void RaiseResumeGameEvent()
    {
        EventBroker.CallGameResumed();
    }

    private void ResumeGame()
    {
        print("UIManager ResumeGame");
        mainUIPanel.SetActive(true);
        gamePausedPanel.SetActive(false);
    }
    
    private void EventBroker_EarnedRewardedAd()
    {
        rewardText.text = GameControl.Reward.ToString();
        rewardTextAnimator.SetTrigger("Flash");
    }

    private void UpdateHighScore()
    {
        hiScoreText.text = $"RECORD:{GameControl.Record}";
        if(GameControl.NewRecord)
            hiScoreTextAnimator.SetTrigger("Flash");
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
                Debug.Log("NORMAL");
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.grey;
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
        selectionScreenCoinsText.text = GameControl.Coins.ToString();
        coinsText.text = GameControl.Coins.ToString();
        coinsTextAnimator.SetTrigger("Flash");
        coinsImageAnimator.SetTrigger("Flash");
    }

    private void UpdateRewardText()
    {
        if(GameControl.NewRecord)
            newRecord.SetActive(true);
        else newRecord.SetActive(false);

        finalScoreText.text = scoreText.text;
        rewardText.text =  GameControl.Reward.ToString();
        
        if(GameControl.MoreReward == GameControl.Reward)
            moreRewardText.text = $"x2";
        else 
            moreRewardText.text = $"+{GameControl.MoreReward}";
    }

    private void ToMainUIPanel()
    {
        difficultyPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        birdsSelectionPanel.SetActive(false);

        mainUIPanel.SetActive(true);
    }
}
