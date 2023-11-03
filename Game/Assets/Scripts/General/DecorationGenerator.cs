using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationGenerator : MonoBehaviour
{
    public GameObject[] decorationPrefabs;

    public float mapWidth;
    public float mapHeight;

    public int decorationCount;

    public List<GameObject> decorationList;

    // Start is called before the first frame update
    void Start()
    {
        //GenerateDecorations();
        //decorationList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GenerateDecorations(Vector3 pointPos)
    {
        for (int i = 0; i < decorationCount; i++)
        {
            
            GameObject decorationPrefab = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
            
            float randomX = Random.Range(-mapWidth / 2, mapWidth / 2);
            float randomY = Random.Range(-mapHeight / 2, mapHeight / 2);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0);

            Vector3 pos = pointPos + randomPosition;

            GameObject decorationInstance = Instantiate(decorationPrefab, pos, Quaternion.identity);
            
            decorationList.Add(decorationInstance);

        }
    }

    public void DestroyDecorations()
    {
        foreach (var decorationInstance in decorationList)
        {
            Destroy(decorationInstance);
        }
        
        decorationList.Clear();
    }
}
