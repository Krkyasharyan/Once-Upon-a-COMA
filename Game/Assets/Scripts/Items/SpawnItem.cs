using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject item;

    public void SpawnItem(GameObject character)
    {
        Vector2 characterPos = character.transform.position;
        Vector2 spawnPos = new Vector2(characterPos.x, characterPos.y);
        GameObject spawnedItem = Instantiate(item, spawnPos, Quaternion.identity);
        SpriteRenderer itemRenderer = spawnedItem.GetComponent<SpriteRenderer>();

        // If the spawned item has a SpriteRenderer component, set the sorting order
        if (itemRenderer != null)
        {
            // Get the sorting order of the enemy's SpriteRenderer
            int enemySortingOrder = character.GetComponent<SpriteRenderer>().sortingOrder;

            // Set the sorting order of the spawned item based on the enemy's sorting order
            itemRenderer.sortingOrder = enemySortingOrder;
        }
    }

}
