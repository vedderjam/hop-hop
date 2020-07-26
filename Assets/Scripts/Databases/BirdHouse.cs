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
        }
    }

    public void Save()
    {
        foreach(var birdInfo in birdInfos)
        {
            PlayerPrefs.SetInt(birdInfo.name, birdInfo.purchased == true ? 1 : 0);
        }
    }
}
