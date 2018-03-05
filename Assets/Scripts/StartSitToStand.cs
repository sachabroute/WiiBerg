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
using UnityEngine.SceneManagement;


public class StartSitToStand : MonoBehaviour
{

    AppLog log;

    public static bool gettingWiiData = false;
    public static IDictionary<String, String> wbb_info = new Dictionary<String, String>();
    public static IDictionary<String, String> wm_info = new Dictionary<String, String>();

    //Wii Balance Board variables
    public static float cogX;//center of gravity in X
    public static float cogY;//center of gravity in Y
    public static float weight;
    public static float bl; //bottom-left sensor
    public static float br;//bottom-right sensor
    public static float tr;//top-right sensor
    public static float tl;//top-left sensor

    //Wii Mote variables
    public static float accX; //acceleration in X
    public static float accY; // " " Y
    public static float accZ;//" " Z
    public static bool buttonA;
    public static bool buttonB;


    // Use this for initialization
    void Awake()
    {
        //initialize the log and create the file
        //log = new AppLog();
        //log.LogMessage("cogX , cogY , weight, bl, br, tl, tr, falls");

    }

    void Start()
    {
        //UnityEngine.Debug.Log("File initialize...");
        //log.LogMessage("This is the first file I've created");

    }

    void FixedUpdate()
    {

        string received_data = TCPListener.readSocket();

        if (received_data != "")
        {
            String[] values = received_data.Split('_');
            if (values.Length == 2)
            {
                switch (values[0])
                {
                    case "LOG":
                        UnityEngine.Debug.Log(values[1]);
                        break;
                    case "WII-INFO":
                        UpdateWiiData(values[1]);
                        if (!gettingWiiData)
                            gettingWiiData = true;
                        break;
                }
            }
        }
    }

    public void closeLog()
    {
        log.Close();
    }

    void OnApplicationQuit()
    {
        closeLog();
    }



    public void UpdateWiiData(String dataString)
    {
        String[] data = dataString.Split(';');
        wbb_info = data[0].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);
        wm_info = data[1].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);

        //Getting variables from the dictionaries
        //Wii Balance Board variables
        /*
        cogX = float.Parse(getWiiInfo.wbb_info["cogX"]);
        cogY = float.Parse(getWiiInfo.wbb_info["cogY"]);
        weight = float.Parse(getWiiInfo.wbb_info["weight"]);
        bl = float.Parse(getWiiInfo.wbb_info["bl"]);
        br = float.Parse(getWiiInfo.wbb_info["br"]);
        tl = float.Parse(getWiiInfo.wbb_info["tl"]);
        tr = float.Parse(getWiiInfo.wbb_info["tr"]);
        */

        //Wii Mote variables
        accX = float.Parse(getWiiInfo.wm_info["accX"]);
        accY = float.Parse(getWiiInfo.wm_info["accY"]);
        accZ = float.Parse(getWiiInfo.wm_info["accZ"]);
        buttonA = bool.Parse(getWiiInfo.wm_info["buttonA"]);
        buttonB = bool.Parse(getWiiInfo.wm_info["buttonB"]);

        //File is created as (timesteps, weight, cogX, cogY, bl, br, tl, tr, accX, accY, accZ, falls )




        //log.LogMessage(weight + "," + cogY + "," + cogX +"," + bl + "," + br + "," + tl + "," + tr + "," + accX + "," + accY + "," + accZ + "," +DetectFalls.fallDetected);
        if (buttonA == true)
        {
            SceneManager.LoadScene("SitToStand2");
            buttonA = false;
        }





    }
}

