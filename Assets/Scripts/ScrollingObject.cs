using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScrollingObject : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField]private float scrollSpeed = -1.5f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        EventBroker.StartPlaying += Setup;
        EventBroker.StartIdling += Setup;
        EventBroker.GameOver += GameOver;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;
    }

    private void OnDestroy()
    {
        EventBroker.StartPlaying -= Setup;
        EventBroker.StartIdling -= Setup;
        EventBroker.GameOver -= GameOver;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
    }

    private void Setup()
    {
        rb2d.velocity = new Vector2(scrollSpeed, 0f);    
    }

    void Start()
    {
        Setup();
    }
    
    private void GameOver()
    {
        rb2d.velocity = Vector2.zero;
    }

    private void ChangeDifficultyLevel()
    {
        scrollSpeed = GameControl.Instance.currentDifficultyLevel.scrollSpeed;
        Setup();
    }
}
