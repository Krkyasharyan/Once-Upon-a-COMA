using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortingOrderManager : MonoBehaviour
{
    public int sortingOrderOffset = 50; // 偏移值，用于确保不同对象的sortingOrder之间有足够的间隔
     
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        // 获取所有的player和enemy对象
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        GameObject[] allElements = players.Concat(enemies).ToArray();
        allElements = allElements.Concat(obstacles).ToArray();

        // 根据对象的Y轴位置调整sortingOrder
        foreach (GameObject element in allElements)
        {
            Renderer playerRenderer = element.GetComponent<Renderer>();
            playerRenderer.sortingOrder = Mathf.RoundToInt(element.transform.position.y * -10) + sortingOrderOffset;
        }
    }
   
}
