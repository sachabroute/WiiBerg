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
    public string task = " ";
    public string user = " ";
    public static string user1 = " ";
    
    
    
    private void Awake()
    {
        //Initialize file
        user1 = user;
        log = new AppLog();
        //log.LogMessage("Task,Timestamp, cogX , cogY , weight, bl, br, tl, tr, accX, accY, accZ, falls");
        
       
    }

    // Use this for initialization
    void Start () {
        UnityEngine.Debug.Log("File created");
        //log.LogMessage("this is the first message");
	}

    // Update is called once per frame
    void Update()
    {

        //We add to the file the data we receive from the WiiBoard and some items from the WiiMote
        //File is created as (timesteps, weight, cogX, cogY, bl, br, tl, tr, accX, accY, accZ, falls )

        log.LogMessage(task + "," + Time.time + "," + getWiiInfo.weight + "," + getWiiInfo.cogX + "," + getWiiInfo.cogY + "," + getWiiInfo.bl + ","
            + getWiiInfo.br + "," + getWiiInfo.tl + "," + getWiiInfo.tr + "," + getWiiInfo.accX + "," + getWiiInfo.accY + ","
            + getWiiInfo.accZ + "," + getWiiInfo.falls +"," + getWiiInfo.buttonA + "," + getWiiInfo.buttonB);
    }

    void OnApplicationQuit()
    {
        closeLog();
    }


    public void closeLog()
    {
        log.Close();
    }

    public void OnDestroy()
    {
        
        log.Close();
        
    }
}
