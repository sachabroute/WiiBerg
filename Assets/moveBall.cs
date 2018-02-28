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
using System.Globalization;

public class moveBall : MonoBehaviour {

    private void Start()
    {
        
    }

    void Update()
    {
        if (getWiiInfo.gettingWiiData)
        {

            float COGx = float.Parse(getWiiInfo.wbb_info["cogX"]);
            float COGy = -float.Parse(getWiiInfo.wbb_info["cogY"]);


            transform.position = new Vector3(COGx, 0, COGy);
        }
    }
}
