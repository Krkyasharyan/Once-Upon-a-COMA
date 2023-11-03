using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextLevel : MonoBehaviour
{
    public RoomGenerator roomGenerator;
    public static bool goFlag = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goFlag)
        {
            goToNextLevel();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int id = other.GetComponent<PlayerController>().player_id;
            if (id == 0)
            {
                goToNextLevel();
            }
            else
            {
                WebController.sendToNextLevel();
            }
        }
    }

    private void goToNextLevel()
    {
        goFlag = false;
        
        //PlayerController player = other.GetComponent<PlayerController>();
        //player.SaveStats();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerObjects)
        {
            player.GetComponent<PlayerController>().SaveStats();
        }
        // Inventory inventory = other.GetComponent<Inventory>();
        // inventory.SaveItems();
        
        PlayerPrefs.SetInt("seed", Random.Range(0, 10000));
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // roomGenerator.resetFlag = true;
        // Debug.Log(roomGenerator.resetFlag);
    }
}