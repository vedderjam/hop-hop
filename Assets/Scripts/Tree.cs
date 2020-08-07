using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public List<Transform> bushSpriteTransforms;
    public List<GameObject> bushAGameObjects;
    public List<GameObject> bushBGameObjects;
    public List<GameObject> bushCGameObjects;
    public List<Vector3> originalPositionOfBushes;
    public float minOffset;
    public float maxOffset;
    public float bushOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bird>() != null)
        {
            EventBroker.CallBirdScored();
        }
    }

    public void SetBushOffset(float bushOffset)
    {
        this.bushOffset = bushOffset;
        originalPositionOfBushes[0] = new Vector3(0f, bushOffset);
        originalPositionOfBushes[2] = new Vector3(0f, -bushOffset);
    }

    public void SetBushesPositions()
    {
        var numberOfSprites = bushSpriteTransforms.Capacity;

        for(int i = 0; i < numberOfSprites; i++)
        {
            bushSpriteTransforms[i].localPosition = 
                originalPositionOfBushes[i] + new Vector3(0f, Random.Range(minOffset, maxOffset));
        }

        ToggleBushesSprites(ref bushAGameObjects);
        ToggleBushesSprites(ref bushBGameObjects);
        ToggleBushesSprites(ref bushCGameObjects);
    }

    private void ToggleBushesSprites(ref List<GameObject> gameObjects)
    {
        int max = gameObjects.Capacity;
        int rnd = Random.Range(0, max);
        for(int i = 0; i < max; i++)
        {
            if(i == rnd)
                gameObjects[i].SetActive(true);
            else
                gameObjects[i].SetActive(false);
        }
    }
}
