using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLogFile : MonoBehaviour
{

    AppLog log;
    public string user = " ";
    public static string user1 = " ";

    private void Awake()
    {
        user1 = user;
        log = new AppLog();

        UnityEngine.Debug.Log("Creating file...");

    }
    // Use this for initialization
    void Start()
    {

        
        log.LogMessage("Task,Timestamp, weight, cogX , cogY , bl, br, tl, tr, accX, accY, accZ, falls");

    }

    // Update is called once per frame
    void Update()
    {

        

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

        closeLog();

    }







}
