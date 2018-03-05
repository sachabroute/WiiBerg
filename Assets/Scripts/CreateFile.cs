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

public class CreateFile : MonoBehaviour {

    AppLog log;

    private void Awake()
    {
        //Initialize file
        log = new AppLog();
        log.LogMessage("cogX , cogY , weight, bl, br, tl, tr, falls");
    }

    // Use this for initialization
    void Start () {
        UnityEngine.Debug.Log("File created");

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnApplicationQuit()
    {
        closeLog();
    }


    public void closeLog()
    {
        log.Close();
    }



    //File is created as (timesteps, weight, cogX, cogY, bl, br, tl, tr, accX, accY, accZ, falls )

}
