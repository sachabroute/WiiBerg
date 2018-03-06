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


	
	// Use this for initialization
	public AppLog () {

		if(!Directory.Exists(Application.dataPath + "Logs/")){
			Directory.CreateDirectory(Application.dataPath   + "Logs/");
		}


		string path = Application.dataPath + "Logs/" + "_" +DateTime.Now.ToString ("yyyMMddHHmm") + "-log.txt";		
		//string path = Application.dataPath + "Logs/"; // + "_" +DateTime.Now.ToString ("yyyMMddHHmm") + "-log.txt";		
		Debug.Log (path);
		logFile = new System.IO.StreamWriter(path, true);
         



				
	}
	
	public void LogMessage(string message){
		if(logFile!=null){
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
