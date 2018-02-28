using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WiimoteLib;

namespace Wii_to_TCP
{
    class Program
    {
        static Wiimote wm;
        static Wiimote wbb;

        static IDictionary<String, String> wbb_info = new Dictionary<String, String>
                {
                    {"bl" ,  "0.0" }, { "br" , "0.0" }, {"tl" , "0.0" }, {"tr" , "0.0"}, {"cogX", "0.0" }, {"cogY", "0.0" }, {"weight", "0.0" }, {"button", "false" }
                };

        static IDictionary<String, String> wm_info = new Dictionary<String, String>
                {
                    {"accX" ,  "0.0" }, { "accY" , "0.0" }, {"accZ" , "0.0" }, {"buttonA" , "0.0"}, {"buttonB", "0.0" }
                };

        static WiiEchoServer echoServer;
        static int port = 50000;
        static IPAddress serverIP = IPAddress.Loopback;

        static void Main(string[] args)
        {
            echoServer = new WiiEchoServer(serverIP, port);
            echoServer.StartServer();
            echoServer.ConnectClient();

            ConnectWiiDevices();

            EchoWiiInfo();

            Quit();

        }

        static void EchoWiiInfo()
        {
            echoServer.Log("Sending Wii Info....");

            string inputLine = "";
            while (inputLine != null)
            {
                // this try catch is because on some "rare" occasions, the wbb_info is update at the same time as it is being called here..
                // so to avoid an exception we just try it again until it works.
                inputLine = "";
                while (inputLine == "")
                {
                    try
                    {
                        String wbb_info_string = string.Join("|", wbb_info.Select(m => m.Key + ":" + m.Value).ToArray());
                        String wm_info_string = string.Join("|", wm_info.Select(m => m.Key + ":" + m.Value).ToArray());
                        inputLine = wbb_info_string + ";" + wm_info_string;
                    }
                    catch (Exception) { }
                }
                
                try
                {
                    echoServer.WriteWiiInfo(inputLine);
                }
                catch (Exception) { break; }
            }
            echoServer.Log("Server was disconnect from client.");
        }

        static void ConnectWiiDevices()
        {
            
            try
            {
                FindBluetoothWii.WiiDeviceSearch(echoServer);
            } catch (Exception ex)
            {
                echoServer.Log("Error: " + ex.Message);
                Quit();
            }

            // find all wiimotes connected to the system
            WiimoteCollection mWC = new WiimoteCollection();

            try
            {
                mWC.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                echoServer.Log("Wiimote not found error " + ex.Message);
                Quit();
            }
            catch (WiimoteException ex)
            {
                echoServer.Log("Wiimote error" + ex.Message);
                Quit();
            }
            catch (Exception ex)
            {
                echoServer.Log("Unknown error" + ex.Message);
                Quit();
            }

            foreach (Wiimote wiim in mWC)
            {
                // connect it and set it up as always
                wiim.WiimoteChanged += wm_WiimoteChanged;
                wiim.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;

                wiim.Connect();
                if (wiim.WiimoteState.ExtensionType == ExtensionType.None)
                {
                    echoServer.Log("Connected WiiMote");
                    wiim.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                    wm = wiim;
                    wm.SetLEDs(true,false,false,false);
                }
                else if (wiim.WiimoteState.ExtensionType == ExtensionType.BalanceBoard)
                {
                    echoServer.Log("Connected WBB");
                    wbb = wiim;
                    wbb.SetLEDs(true, true, false, false);
                }
            }
        }

        static void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            if (args.Inserted)
                wm.SetReportType(InputReport.IRExtensionAccel, true);    // return extension data
            else
                wm.SetReportType(InputReport.IRAccel, true);            // back to original mode
        }

        static void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            // current state information
            WiimoteState ws = args.WiimoteState;

            switch (ws.ExtensionType)
            {
                case (ExtensionType.BalanceBoard):  
                    wbb_info["bl"] = ws.BalanceBoardState.SensorValuesKg.BottomLeft.ToString(CultureInfo.InvariantCulture);
                    wbb_info["br"] = ws.BalanceBoardState.SensorValuesKg.BottomRight.ToString(CultureInfo.InvariantCulture);
                    wbb_info["tl"] = ws.BalanceBoardState.SensorValuesKg.TopLeft.ToString(CultureInfo.InvariantCulture);
                    wbb_info["tr"] = ws.BalanceBoardState.SensorValuesKg.TopRight.ToString(CultureInfo.InvariantCulture);
                    wbb_info["cogX"] = ws.BalanceBoardState.CenterOfGravity.X.ToString(CultureInfo.InvariantCulture);
                    wbb_info["cogY"] = ws.BalanceBoardState.CenterOfGravity.Y.ToString(CultureInfo.InvariantCulture);
                    wbb_info["weight"] = ws.BalanceBoardState.WeightKg.ToString(CultureInfo.InvariantCulture);
                    wbb_info["button"] = ws.ButtonState.A.ToString();
                    break;
                case (ExtensionType.None):
                    wm_info["buttonA"] = ws.ButtonState.A.ToString();
                    wm_info["buttonB"] = ws.ButtonState.B.ToString();
                    wm_info["accX"] = ws.AccelState.Values.X.ToString(CultureInfo.InvariantCulture);
                    wm_info["accY"] = ws.AccelState.Values.Y.ToString(CultureInfo.InvariantCulture);
                    wm_info["accZ"] = ws.AccelState.Values.Z.ToString(CultureInfo.InvariantCulture);
                    break;
            }
        }

        static void Quit()
        {
            echoServer.Log("Quitting echo server");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
