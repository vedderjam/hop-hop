using System;

public class EventBroker
{
    public static event Action GameOver;
    public static event Action BirdScored;
    public static event Action StartPlaying;
    public static event Action StartIdling;
    public static event Action ChangeDifficultyLevel;
    public static event Action NotEnoughCoins;
    public static event Action<int> BirdPurchased;
    public static event Action BirdSelected;

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

    public static void CallNotEnoughCoins()
    {
        NotEnoughCoins?.Invoke();
    }

    public static void CallBirdPurchased(int index)
    {
        BirdPurchased?.Invoke(index);
    }
    
    public static void CallBirdSelected()
    {
        BirdSelected?.Invoke();
    }
}
