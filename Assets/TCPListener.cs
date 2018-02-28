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

public class TCPListener : MonoBehaviour {

    public static String host = "localhost";
    public static Int32 port = 50000;
    private static Boolean socket_ready = false;
    private static String input_buffer = "";
    private static TcpClient tcp_socket;
    private static NetworkStream net_stream;
    public static StreamWriter socket_writer;
    public static StreamReader socket_reader;

    void Awake()
    {
        DontDestroyOnLoad(this);
        setupSocket();
    }

    void OnApplicationQuit()
    {
        closeSocket();
    }

    static void setupSocket()
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

    static public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }

    static public String readSocket()
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
            }
            catch (Exception ex) { UnityEngine.Debug.Log(ex); }
        }


        return message;
    }

    static public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

    
}
