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
    public List<GameObject> birdsPrefabs;
    [SerializeField]private GameObject[] birds;
    public GameObject currentBird;
    private int currentBirdIndex;

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
        birds = new GameObject[birdsPrefabs.Capacity];
        userData.Load();
        currentDifficultyLevel = userData.GetDifficultyLevel();
        currentBirdIndex = userData.CurrentBirdIndex;
        SetBird(currentBirdIndex);
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
    private void BirdScored()
    {
        if(gameState != GameState.Playing) return;

        Score++;
        audioSource.PlayOneShot(scoreClip);
    }

    private void GameOver()
    {
        UpdateHighScore();
        UpdateCoins();
        ResetScore();
        gameState = GameState.GameOver;
        Invoke(nameof(Idle), 2f);
        audioSource.PlayOneShot(gameOverClip);
        userData.Save();
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

    private void UpdateCoins()
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

    public void SetBird(int index)
    {
        if(birds[index] == null)
        {
            if(currentBird != null)
                currentBird.SetActive(false); // Prior bird
            
            birds[index] = Instantiate(birdsPrefabs[index], initialPosition, Quaternion.identity, birdsParentTransform);
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
    }

    #endregion
}
