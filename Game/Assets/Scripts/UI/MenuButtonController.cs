using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour {

	// Use this for initialization
	public int index;
	[SerializeField] bool keyDown;
	[SerializeField] int maxIndex;

	void Start () {
		maxIndex = 3;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis ("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 )
		{
			if(!keyDown)
			{
				if (Input.GetAxis ("Vertical") < 0 || Input.GetAxis ("Vertical") > 0) 
				{
					index = (index + 2) % 4;					
				} 
				else if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
				{
					if(index < 2)
					{
                    	index = 1 - index;
                    }
					else
					{
                    	index = 5 - index;
                    }
				} 
				keyDown = true;
			}
		}
		else{
			keyDown = false;
		}
	}

}
