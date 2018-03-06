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


public class ActivateSitTioStand : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) == true)
        {
            SceneManager.LoadScene("SitToStand2");
        }

    }


}

