using System;
using UnityEngine;

public enum Difficulty {Easy, Normal, Hard}

[Serializable][CreateAssetMenu]
public class DifficultyLevel : ScriptableObject
{
    public Difficulty level = Difficulty.Normal;
    public int coinsMultiplier = 1;
    public float scrollSpeed = -1.5f;
    public float spawnRate = 2f;
    public float columnMin = -1f;
    public float columnMax = 3.5f;
    public float bottomColumnOffset = -12f;

    public DifficultyLevel()
    {
        level = Difficulty.Normal;
        coinsMultiplier = 1;
        scrollSpeed = -1.5f;
        spawnRate = 2f;
        columnMin = -1f;
        columnMax = 3.5f;
        bottomColumnOffset = -12f;
    }

    public DifficultyLevel(Difficulty level, int coinsMultiplier, float scrollSpeed, float spawnRate, float columnMin, float columnMax, float bottomColumnOffset)
    {
        this.level = level;
        this.coinsMultiplier = coinsMultiplier;
        this.scrollSpeed = scrollSpeed;
        this.spawnRate = spawnRate;
        this.columnMin = columnMin;
        this.columnMax = columnMax;    
        this.bottomColumnOffset = bottomColumnOffset;
    }
}
