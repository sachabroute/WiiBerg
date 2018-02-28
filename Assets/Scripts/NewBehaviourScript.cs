using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {

	public Canvas StartCanvas;
	public Canvas First;

	void Awake(){
		First.enabled = false;
	}

	public void FirstOn(){
		Application.LoadLevel ("Second");
	}

	public void ReturnOn(){
		First.enabled = false;
		StartCanvas.enabled = true;
	}
}
