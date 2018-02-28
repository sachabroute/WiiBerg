using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Wii_to_TCP
{

    class WiiEchoServer
    {
        int port;
        IPAddress serverIP;
        TcpListener listener;
        TcpClient client;
        StreamWriter writer;
        StreamReader reader;

        public WiiEchoServer(IPAddress ip, int p)
        {
            serverIP = ip;
            port = p;
        }

        public void StartServer()
        {
            Console.WriteLine("Starting echo server...");

            listener = new TcpListener(serverIP, port);
            listener.Start();

            Console.WriteLine("Started listening");
        }

        public void ConnectClient()
        {
            client = listener.AcceptTcpClient();
            Console.WriteLine("Accepted client");
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Got stream");
            writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            Console.WriteLine("Got writer");
            reader = new StreamReader(stream, Encoding.ASCII);
            Console.WriteLine("Got reader");
        }

        public void Log(String message)
        {
            try
            {
                writer.WriteLine("LOG_" + message);
            } catch(Exception)
            {
                throw;
            }
        }

        public void WriteWiiInfo(String message)
        {
            writer.WriteLine("WII-INFO_" + message);
            WaitForRead();
        }

        public String ReadLine()
        {
            return reader.ReadLine();
        }

        public void WaitForRead()
        {
            // check that message was received before sending another
            bool read = false;
            String message = "";
            while (!read)
            {
                try
                {
                    message = this.ReadLine();
                }
                catch (System.IO.IOException) { this.StopTCP(); }

                if (message == "read")
                {
                    read = true;
                }
            }
        }

        public void StopTCP()
        {
            client.Close();
            listener.Stop();
            Environment.Exit(0);
        }
    }
}
