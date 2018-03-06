using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;
using System;

public class AppLog {

    StreamWriter logFile;
    //StreamWriter logFile;
	string msg;
	bool logClosed;
    string path;



    // Use this for initialization
    public AppLog()
    {

        if (!Directory.Exists(Application.dataPath + "Logs/"))
        {
            Directory.CreateDirectory(Application.dataPath + "Logs/");
        }
        //string path = Application.dataPath + "Logs/" + "_" + DateTime.Now.ToString("yyyMMddHHmm") + "-log.txt";
        string path = Application.dataPath + "Logs/" + "_" + DateTime.Now.ToString("yyyMMdd") + "_" + CreateFile.user1 + "-log.txt";
        //string path = Application.dataPath + "Logs/"; // + "_" +DateTime.Now.ToString ("yyyMMddHHmm") + "-log.txt";		
        UnityEngine.Debug.Log(path);
        logFile = new System.IO.StreamWriter(path, true);
        

        /*
        if (Directory.Exists(Application.dataPath + "Logs/"))
        {
            TextWriter tw = new StreamWriter(Application.dataPath + "Logs/" + "_" + DateTime.Now.ToString("yyyMMddHHmm") + "-log.txt", true);
            File.AppendAllText(Application.dataPath + "Logs/" + "_" + DateTime.Now.ToString("yyyMMddHHmm") + "-log.txt", "Hola");
        }
    }
    */
    }

        public void LogMessage(string message) {
            if (logFile != null) {
                //Debug.Log ("logging message");
                string logMsg = message;
                //logFile.WriteLine(",");
                logFile.WriteLine(logMsg);
            }
        }

  
	public void Close(){
		
		if(logFile != null){
			//logFile.WriteLine("#Ending log sesion");
			logFile.Close();
			logFile = null;
		}
	}
	
	public void OnDestroy(){
		Close();
		
	}
}
