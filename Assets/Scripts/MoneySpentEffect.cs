using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpentEffect : MonoBehaviour
{
    public void OnEffectStarted()
    {
        Debug.Log("Effect started");
    }

    public void OnEffectFinished()
    {
        gameObject.SetActive(false);
        Debug.Log("Effect finished");
    }
}
