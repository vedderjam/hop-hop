using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState{ Idle, Playing, GameOver}

public class GameControl : Singleton<GameControl>
{
    #region Members

    public DifficultyLevel currentDifficultyLevel;
    public UserData userData;
    public GameState gameState;
    private AudioSource audioSource;
    private Vector3 initialPosition = new Vector3(-1.5f, 0f, 0f);

    [Header("Audio clips")]
    public AudioClip scoreClip;
    public AudioClip gameOverClip;
    public AudioClip selectClip;

    public static int Score
    {
        get;
        private set;
    }
    public static int Record
    {
        get;
        private set;
    }
    public static int Coins
    {
        get;
        private set;
    }
    public static int Reward
    {
        get;
        private set;
    }

    [Header("Birds")]
    public Transform birdsParentTransform;
    [SerializeField]private GameObject[] birds;
    public GameObject currentBird;
    private int currentBirdIndex;
    public BirdHouse birdHouse;

    [Space]
    public Animator moneyEffectAnimator;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        EventBroker.GameOver += GameOver;
        EventBroker.BirdScored += BirdScored;
    }

    private void Start()
    {
        birds = new GameObject[birdHouse.birdInfos.Capacity];
        Load();
        currentDifficultyLevel = userData.GetDifficultyLevel();
        currentBirdIndex = GetCurrentBirdIndex();
        SelectBird(currentBirdIndex);
        EventBroker.CallChangeDifficultyLevel();
        Score = 0;
        Record = userData.GetCurrentLevelRecord();
        Coins = userData.Coins;
        gameState = GameState.Idle;
        UpdateHighScore();
    }

    private void OnDestroy()
    {
        EventBroker.GameOver -= GameOver;
        EventBroker.BirdScored -= BirdScored;
    }
    #endregion

    #region Methods
    private void Load()
    {
        userData.Load();
        birdHouse.Load();
    }

    private int GetCurrentBirdIndex()
    {
        int current = userData.CurrentBirdIndex;

        if(current >= birdHouse.birdInfos.Capacity)
        {
            current = 0;
        }

        return current;
    }

    private void BirdScored()
    {
        if(gameState != GameState.Playing) return;

        Score++;
        audioSource.PlayOneShot(scoreClip);
    }

    private void GameOver()
    {
        UpdateHighScore();
        UpdateCoinsAfterLosing();
        ResetScore();
        gameState = GameState.GameOver;
        Invoke(nameof(Idle), 2f);
        audioSource.PlayOneShot(gameOverClip);
        userData.Save();
        birdHouse.Save(); // REMOVE
    }
    
    private void UpdateHighScore()
    {
        switch (currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                userData.EasyRecord = Score;
                Record = userData.EasyRecord;
                break;
            case Difficulty.Normal:
                userData.NormalRecord = Score;
                Record = userData.NormalRecord;
                break;
            case Difficulty.Hard:
                userData.HardRecord = Score;
                Record = userData.HardRecord;
                break;
        }
    }

    private void UpdateCoinsAfterLosing()
    {
        Reward = Score * currentDifficultyLevel.coinsMultiplier;
        userData.AddCoins(Reward);
        Coins = userData.Coins;
    }

    private void ResetReward()
    {
        Reward = 0;
    }

    private void Idle()
    {
        if(gameState == GameState.GameOver)
        {
            gameState = GameState.Idle;
            EventBroker.CallStartIdling();
        }
    }

    public void StartPlaying()
    {
        gameState = GameState.Playing;
        EventBroker.CallStartPlaying();
    }

    private void ResetScore()
    {
        Score = 0;
    }

    public void SetDifficultyLevel(int index)
    {
        if(userData.SetDifficultyLevelByIndex(index))
        {
            currentDifficultyLevel = userData.GetDifficultyLevel();
            UpdateHighScore();
            EventBroker.CallChangeDifficultyLevel();
        }
    }

    public void TryToBuyAndOrSelectBird(int index)
    {
        if (!birdHouse.birdInfos[index].purchased)
        {
            Debug.Log(birdHouse.birdInfos[index].name);
            var coins = Coins;
            var purchased = birdHouse.Purchase(index, ref coins);
            if(purchased)
            {
                Coins = coins;
                userData.SetCoins(Coins);
                ShowMoneySpentEffect(index);
                EventBroker.CallBirdPurchased(index);
                SelectBird(index);
                userData.Save();
                birdHouse.Save();
            }
            else
            {
                audioSource.PlayOneShot(gameOverClip);
                EventBroker.CallNotEnoughCoins();
            }
        }
        else SelectBird(index);

    }

    private void ShowMoneySpentEffect(int birdIndex)
    {
        moneyEffectAnimator.gameObject.GetComponentInChildren<Text>().text = $"-{birdHouse.birdInfos[birdIndex].prize}";
        moneyEffectAnimator.gameObject.SetActive(true);
        moneyEffectAnimator.SetTrigger("Fade");
    }

    private void SelectBird(int index)
    {
        if(birds[index] == null)
        {
            if(currentBird != null)
                currentBird.SetActive(false); // Prior bird
            
            birds[index] = Instantiate(birdHouse.birdInfos[index].prefab, initialPosition, Quaternion.identity, birdsParentTransform);
            currentBird = birds[index];
        }
        else
        {
            currentBird.SetActive(false);
            currentBird = birds[index];
            currentBird.SetActive(true);
        }

        currentBirdIndex = index;
        userData.CurrentBirdIndex = currentBirdIndex;
        EventBroker.CallBirdSelected();
    }

    #endregion
}
