using System;

public class EventBroker
{
    public static event Action GameOver;
    public static event Action BirdScored;
    public static event Action StartPlaying;
    public static event Action StartIdling;
    public static event Action ChangeDifficultyLevel;

    public static void CallGameOver()
    {
        GameOver?.Invoke();
    }

    public static void CallBirdScored()
    {
        BirdScored?.Invoke();
    }

    public static void CallStartPlaying()
    {
        StartPlaying?.Invoke();
    }

    public static void CallStartIdling()
    {
        StartIdling?.Invoke();
    }

    public static void CallChangeDifficultyLevel()
    {
        ChangeDifficultyLevel?.Invoke();
    }
}
