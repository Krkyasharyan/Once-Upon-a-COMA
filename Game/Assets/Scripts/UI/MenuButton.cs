using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
	[SerializeField] UIController uiController;
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] int thisIndex;

    // Update is called once per frame
    void Update()
    {
		if(menuButtonController.index == thisIndex)
		{
			animator.SetBool ("selected", true);
			if(Input.GetAxis ("Submit") == 1){
				animator.SetBool ("pressed", true);
				Choose();
			}else if (animator.GetBool ("pressed")){
				animator.SetBool ("pressed", false);
			}
		}else{
			animator.SetBool ("selected", false);
		}
    }

	void Choose()
	{
		if(thisIndex == 0)
		{
			PlayerPrefs.SetInt("curHP", 100);
			PlayerPrefs.SetInt("items", 0);
			PlayerPrefs.SetInt("seed", Random.Range(0, 10000));
			GameLoader.LoadSinglePlayerScene();
		}
		else if(thisIndex == 1)
		{
			GameLoader.LoadSinglePlayerScene();
		}
		else if(thisIndex == 2)
		{
			PlayerPrefs.SetInt("P1curHP", 100);
			PlayerPrefs.SetInt("P1items", 0);
			PlayerPrefs.SetInt("P2curHP", 100);
			PlayerPrefs.SetInt("P2items", 0);
			ButtonController.setipadr(WebController.getip());
        	WebController.getroomlist();
			uiController.UIMultiplayer();
		}
		else if(thisIndex == 3)
		{
			GameLoader.QuitGame();
		}
	}

	

}
