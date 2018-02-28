using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

public class DetectFalls : MonoBehaviour {

	List<float> weightsList = new List<float>();
	List<float> cogXList = new List<float>();
	List<float> cogYList = new List<float>();
    float cogX;
    float cogY;
	float cogXSum;
	float cogYSum;


	private void Start()
	{

	}

	void FixedUpdate()
	{
		if (getWiiInfo.gettingWiiData)
		{
			float weight = float.Parse(getWiiInfo.wbb_info["weight"]);
			float cogX = float.Parse(getWiiInfo.wbb_info["cogX"]);
			float cogY = float.Parse(getWiiInfo.wbb_info["cogY"]);

			weightsList.Add (weight);
			cogXList.Add (cogX);
			cogYList.Add (cogY);

			if (weightsList.Count > 50) 
			{
				weightsList.RemoveAt(0);
			}

			if (cogXList.Count > 50) 
			{
				cogXList.RemoveAt(0);
				cogYList.RemoveAt(0);
			}

			if (FallDetector ()) 
			{
				UnityEngine.Debug.Log ("Fall detected");

			}

            UnityEngine.Debug.Log(cogX + "  " + cogY);
        }


    }

	private bool FallDetector()
	{
		bool fall = false;

		float lastValue = weightsList [weightsList.Count -1];
		float firstValue = weightsList [0];
		float cogLength = cogXList.Count - 1;


		/*
        for (int i = 0; i < cogLength; i++)
		{
			cogXSum = (cogXList [i + 1] - cogXList [i]) + cogXSum;
			cogYSum = (cogYList [i + 1] - cogYList [i]) + cogYSum;
		} */
        

		if (((lastValue - firstValue) < -20))
        {
			fall = true;
		}

		return fall;

	}
}
