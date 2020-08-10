﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BirdInfo
{
    public string name;
    public Sprite sprite;
    public int prize;
    public bool purchased;
    public GameObject prefab;
    public int aggregatedScore;
    public List<string> infoPills = new List<string>(6){ "Esta información estará disponible en breve.", "Esta información estará disponible en breve.","Esta información estará disponible en breve.","Esta información estará disponible en breve.","Esta información estará disponible en breve.", "Esta información estará disponible en breve."};
    public List<int> scoreNeededToShowInfoPill = new List<int>(6) { 50, 100, 200, 300, 350, 500};
}

[CreateAssetMenu]
public class BirdHouse : ScriptableObject
{
    public List<BirdInfo> birdInfos;

    public bool Purchase(int index, ref int coins)
    {
        if(index < 0 || index >= birdInfos.Capacity)
        {
            return false;
        }
        else if(coins < birdInfos[index].prize)
        {
            return false;
        }
        else
        {
            if (birdInfos[index].purchased)
            {
                return false;
            }
            else
            {
                coins -= birdInfos[index].prize;
                birdInfos[index].purchased = true;
                return true;
            }
        }
    }

    public void Load()
    {
        int capacity = birdInfos.Capacity;
        for(int i = 0; i < capacity; i++)
        {
            if(i == 0) // The first bird is always "purchased"
            {
                birdInfos[i].purchased = true;
            }
            else
            {
                birdInfos[i].purchased = PlayerPrefs.GetInt(birdInfos[i].name, 0) == 1 ? true : false;
            }
            birdInfos[i].aggregatedScore = PlayerPrefs.GetInt($"{birdInfos[i].name}_aggregatedScore", 0);
        }
    }

    public void Save()
    {
        foreach(var birdInfo in birdInfos)
        {
            PlayerPrefs.SetInt(birdInfo.name, birdInfo.purchased == true ? 1 : 0);
            PlayerPrefs.SetInt($"{birdInfo.name}_aggregatedScore", birdInfo.aggregatedScore);
        }
    }
}
