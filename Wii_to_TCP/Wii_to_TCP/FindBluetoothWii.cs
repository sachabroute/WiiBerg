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
        static public void WiiDeviceSearch(WiiEchoServer echoServer)
        {
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

                //if (checkBox_RemoveExisting.Checked)
                //{
                    var btExistingList = btClient.DiscoverDevices(255, false, true, false);

                    foreach (var btItem in btExistingList)
                    {
                        if (!btItem.DeviceName.Contains("Nintendo")) continue;

                        BluetoothSecurity.RemoveDevice(btItem.DeviceAddress);
                        btItem.SetServiceState(BluetoothService.HumanInterfaceDevice, false);
                    }
                //}

                // Find unknown bluetooth devices.

                echoServer.Log("Searching for bluetooth devices...");

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

                    // Send special pin for permanent sync.

                    //if (checkBox_PermanentSync.Checked)
                    //{
                    //    // Sync button requires host address, holding 1+2 buttons requires device address.

                    //    var btPin = AddressToWiiPin(BluetoothRadio.PrimaryRadio.LocalAddress.ToString());

                    //    // Pin needs to be added before doing the pair request.

                    //    new BluetoothWin32Authentication(btItem.DeviceAddress, btPin);

                    //    // Null forces legacy pin request instead of SSP authentication.

                    //    BluetoothSecurity.PairRequest(btItem.DeviceAddress, null);
                    //}

                    // Install as a HID device and allow some time for it to finish.

                    btItem.SetServiceState(BluetoothService.HumanInterfaceDevice, true);
                }

                // Allow slow computers to finish installation before connecting.

                System.Threading.Thread.Sleep(4000);

                // Connect and send a command, otherwise they sleep and the device disappears.

                try
                {
                    if (btDiscoveredList.Length > btIgnored)
                    {
                        var deviceCollection = new WiimoteCollection();
                        deviceCollection.FindAllWiimotes();

                        foreach (var wiiDevice in deviceCollection)
                        {
                            wiiDevice.Connect();
                            wiiDevice.SetLEDs(true, false, false, false);
                            wiiDevice.Disconnect();
                        }
                    }
                }
                catch (Exception ex) { echoServer.Log("Error!" + ex); }

                // Status report.

                echoServer.Log("Finished - You can now close this window. Found: " + btDiscoveredList.Length + " Ignored: " + btIgnored);
            }
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
