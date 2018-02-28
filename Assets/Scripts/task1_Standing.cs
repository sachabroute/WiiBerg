using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class task1_Standing : MonoBehaviour {

	public bool end_waiting;
	public int surfaceCounter;

	private List<float> cogXList = new List<float>();
	private List<float> cogYList = new List<float>();
	private List<float> weightList = new List<float> ();
	private List<float> surfaceList = new List<float> ();

	float tr;
	float br;
	float tl;
	float bl;



	void Start () {
		StartCoroutine (WaitTwoMinutes ());
	}


	void Update () {

		//Have two minutes already finished? If not keep storing data
		float cogX = float.Parse(getWiiInfo.wbb_info["cogX"]);
		float cogY = float.Parse(getWiiInfo.wbb_info["cogY"]);
		float weight = float.Parse (getWiiInfo.wbb_info ["weight"]);
		float br = float.Parse (getWiiInfo.wbb_info ["br"]);
		float bl = float.Parse (getWiiInfo.wbb_info ["bl"]);
		float tr = float.Parse (getWiiInfo.wbb_info ["tr"]);
		float tl = float.Parse (getWiiInfo.wbb_info ["tl"]);

		if (!end_waiting) {
			cogXList.Add (cogX);
			cogYList.Add (cogY);
			AreaOfSupport ();
		}

		//If the two minutes have already finished, stop the loop and analyse data
		else if (end_waiting) 
		{
			
		}
	}

	IEnumerator WaitTwoMinutes()
	{
		yield return new WaitForSeconds (60);
		end_waiting = true;
	}

	private void AreaOfSupport()
	{
		float right = br + tr;
		float left = tl + bl;
		if (Mathf.Abs (left - right) > 5) 
		{
			surfaceList.Add (left - right);
			if (surfaceList.Count > 1 && (Mathf.Sign (surfaceList [surfaceList.Count - 1]) != Mathf.Sign (surfaceList [surfaceList.Count - 2]))) 
			{
				surfaceCounter += 1;
			}
		}
 	}


}
