using System;
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
}
