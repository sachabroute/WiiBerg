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

    public String host = "localhost";
    public Int32 port = 50000;
    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;
    StreamWriter socket_writer;
    StreamReader socket_reader;

    public static bool gettingWiiData = false;
    public static IDictionary<String, String> wbb_info = new Dictionary<String, String>
                {
                    {"bl" ,  "0.0" }, { "br" , "0.0" }, {"tl" , "0.0" }, {"tr" , "0.0"}, {"cogX", "0.0" }, {"cogY", "0.0" }, {"weight", "0.0" }, {"button", "false" }
                };
    public static IDictionary<String, String> wm_info = new Dictionary<String, String>
                {
                    {"accX" ,  "0.0" }, { "accY" , "0.0" }, {"accZ" , "0.0" }, {"buttonA" , "false"}, {"buttonB", "false" }
                };

    void FixedUpdate()
    {

        string received_data = readSocket();

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

    void Awake()
    {
        setupSocket();
    }

    void OnApplicationQuit()
    {
        closeSocket();
    }

    public void setupSocket()
    {
        UnityEngine.Debug.Log(Directory.GetCurrentDirectory());
        try
        {
            Process wbb_tcp = new Process();
            wbb_tcp.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Wii_to_TCP/Wii_to_TCP/bin/Debug/Wii_to_TCP.exe";
            wbb_tcp.Start();

        }
        catch (Exception ex) { UnityEngine.Debug.Log(ex); return; }

        for (int i = 0; i < 10; i++)
        {
            UnityEngine.Debug.Log("Trying to connect #" + i + "/10");
            try
            {
                tcp_socket = new TcpClient(host, port);

                net_stream = tcp_socket.GetStream();
                socket_writer = new StreamWriter(net_stream, Encoding.ASCII) { AutoFlush = true };
                socket_reader = new StreamReader(net_stream);

                socket_ready = true;

                UnityEngine.Debug.Log("Connected");
                break;
            }
            catch (Exception e)
            {
                // Something went wrong
                UnityEngine.Debug.Log("Socket error: " + e);
                Thread.Sleep(500);
            }
        }
    }

    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }

    public String readSocket()
    {
        String message = "";
        if (!socket_ready)
            message = "socket not ready";

        if (net_stream.DataAvailable)
        {
            message = socket_reader.ReadLine();
            try
            {
                socket_writer.WriteLine("read");
            } catch(Exception ex) { UnityEngine.Debug.Log(ex); }
        }
            

        return message;
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

    public void UpdateWiiData(String dataString)
    {
        String[] data = dataString.Split(';');

        wbb_info = data[0].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);
        wm_info = data[1].Split('|').Select(p => p.Trim().Split(':')).ToDictionary(p => p[0], p => p[1]);
    }
    
}
