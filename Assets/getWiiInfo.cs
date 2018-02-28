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

public class getWiiInfo : MonoBehaviour {

    public static bool gettingWiiData = false;
    public static IDictionary<String, String> wbb_info = new Dictionary<String, String>();
    public static IDictionary<String, String> wm_info = new Dictionary<String, String>();

    // Use this for initialization
    void Awake()
    {

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

    public void UpdateWiiData(String dataString)
    {
        String[] data = dataString.Split(';');

        wbb_info = data[0].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);
        wm_info = data[1].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);
    }
}
