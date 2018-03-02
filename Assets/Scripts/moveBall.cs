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
            transform.position = new Vector3(getWiiInfo.cogX, 0, getWiiInfo.cogY);
        }
    }
}
