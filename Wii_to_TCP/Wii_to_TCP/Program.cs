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
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

namespace Wii_to_TCP
{
    class Program
    {
        static Wiimote wm;
        static Wiimote wbb;

        // we put initial values here so that it doesn't send an empty dictionary to unity
        static IDictionary<String, String> wbb_info = new Dictionary<String, String>
            {
                {"button", "false" }
            };
        static IDictionary<String, String> wm_info = new Dictionary<String, String>
            {
                {"buttonA", "false" }
            };

        static WiiEchoServer echoServer;
        static int port = 50000;
        static IPAddress serverIP = IPAddress.Loopback;

        static void Main(string[] args)
        {
            echoServer = new WiiEchoServer(serverIP, port);
            echoServer.StartServer();
            try
            {
                echoServer.ConnectClient();
            } catch(Exception ex)
            {
                Console.WriteLine("Can't make server, could the port or IP address be already used? " + ex);
                Quit();
            }

            ConnectWiiDevices();

            EchoWiiInfo();

            Quit();

        }

        static void EchoWiiInfo()
        {
            echoServer.Log("Sending Wii Info....");
            Console.WriteLine("Sending Wii Info....");

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
                    Console.WriteLine(inputLine);
                }
                catch (Exception) { break; }
            }
            echoServer.Log("Server was disconnect from client.");
            Console.WriteLine("Server was disconnect from client.");
        }

        static void ConnectWiiDevices()
        {
            
            try
            {
                FindBluetoothWii.WiiDeviceSearch(echoServer);
            } catch (Exception ex)
            {
                echoServer.Log("Error: " + ex.Message);
                Console.WriteLine("Error: " + ex.Message);
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
                Console.WriteLine("Wiimote not found error " + ex.Message);
                Quit();
            }
            catch (WiimoteException ex)
            {
                echoServer.Log("Wiimote error" + ex.Message);
                Console.WriteLine("Wiimote error" + ex.Message);
                Quit();
            }
            catch (Exception ex)
            {
                echoServer.Log("Unknown error" + ex.Message);
                Console.WriteLine("Unknown error" + ex.Message);
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
                    Console.WriteLine("Connected WiiMote");
                    wiim.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                    wm = wiim;
                    wm.SetLEDs(true,false,false,false);
                }
                else if (wiim.WiimoteState.ExtensionType == ExtensionType.BalanceBoard)
                {
                    echoServer.Log("Connected WBB");
                    Console.WriteLine("Connected WBB");
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

        public static void WiiDeviceSearch(WiiEchoServer echoServer)
        {
            try
            {
                quickConnect();

                echoServer.Log("Devices already found and connected, skipping the pairing process");
                Console.WriteLine("Devices already found and connected, skipping the pairing process");
                return;
            }
            catch (Exception)
            {
                echoServer.Log("Devices not found, starting pairing process");
                Console.WriteLine("Devices not found, starting pairing process");
            }

            using (var btClient = new BluetoothClient())
            {
                // PROBLEM:
                // false false true: finds only unknown devices, which excludes existing but broken device entries.
                // false true  true: finds broken entries, but even if powered off, so pairing attempts then crash.
                // WORK-AROUND:
                // Remove existing entries first, then find powered on entries.

                var btIgnored = 0;

                // Find remembered bluetooth devices.

                echoServer.Log("Removing existing bluetooth devices...");
                Console.WriteLine("Removing existing bluetooth devices...");

                var btExistingList = btClient.DiscoverDevices(255, false, true, false);

                foreach (var btItem in btExistingList)
                {
                    if (!btItem.DeviceName.Contains("Nintendo")) continue;

                    BluetoothSecurity.RemoveDevice(btItem.DeviceAddress);
                    btItem.SetServiceState(BluetoothService.HumanInterfaceDevice, false);
                }

                // Find unknown bluetooth devices.

                echoServer.Log("Searching for bluetooth devices...");
                Console.WriteLine("Searching for bluetooth devices...");

                var btDiscoveredList = btClient.DiscoverDevices(255, false, false, true);

                foreach (var btItem in btDiscoveredList)
                {
                    // Just in-case any non Wii devices are waiting to be paired.

                    if (!btItem.DeviceName.Contains("Nintendo"))
                    {
                        btIgnored += 1;
                        continue;
                    }

                    echoServer.Log("Adding: " + btItem.DeviceName + " ( " + btItem.DeviceAddress + " )");
                    Console.WriteLine("Adding: " + btItem.DeviceName + " ( " + btItem.DeviceAddress + " )");

                    // If interested in permanent sync look at WiiBalanceWalker code

                    // Install as a HID device and allow some time for it to finish.

                    btItem.SetServiceState(BluetoothService.HumanInterfaceDevice, true);
                }

                // Allow slow computers to finish installation before connecting.

                System.Threading.Thread.Sleep(4000);

                // Connect and send a command, otherwise they sleep and the device disappears.

                try
                {
                    quickConnect();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Status report.

                echoServer.Log("Finished - Found: " + btDiscoveredList.Length + " Ignored: " + btIgnored);
                Console.WriteLine("Finished - Found: " + btDiscoveredList.Length + " Ignored: " + btIgnored);
            }
        }

        static private void quickConnect()
        {
            //if (btDiscoveredList.Length > btIgnored)
            //{
            var deviceCollection = new WiimoteCollection();
            deviceCollection.FindAllWiimotes();

            if (deviceCollection.Count() == 0)
            {
                throw new Exception("No Wii devices found!");
            }

            foreach (var wiiDevice in deviceCollection)
            {
                wiiDevice.Connect();
                wiiDevice.SetLEDs(true, false, false, false);
                wiiDevice.Disconnect();
                string wiiType = "";
                switch (wiiDevice.WiimoteState.ExtensionType)
                {
                    case ExtensionType.None:
                        wiiType = "WiiMote";
                        break;
                    case ExtensionType.BalanceBoard:
                        wiiType = "Balance Board";
                        break;
                    default:
                        wiiType = "Something else: " + wiiDevice.WiimoteState.ExtensionType;
                        break;
                }
                echoServer.Log("Found " + wiiType);
                Console.WriteLine("Found " + wiiType);
            }
            //}
        }

        static private string AddressToWiiPin(string bluetoothAddress)
        {
            if (bluetoothAddress.Length != 12) throw new Exception("Invalid Bluetooth Address: " + bluetoothAddress);

            var bluetoothPin = "";
            for (int i = bluetoothAddress.Length - 2; i >= 0; i -= 2)
            {
                string hex = bluetoothAddress.Substring(i, 2);
                bluetoothPin += (char)Convert.ToInt32(hex, 16);
            }
            return bluetoothPin;
        }

        static void Quit()
        {
            echoServer.Log("Quitting echo server");
            Console.WriteLine("Quitting echo server");
            try
            {
                echoServer.StopTCP();
            }
            finally
            {
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
