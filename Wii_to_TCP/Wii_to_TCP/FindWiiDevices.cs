using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.IO;

namespace Wii_to_TCP
{
    static class FindBluetoothWii
    {

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
    }
}
