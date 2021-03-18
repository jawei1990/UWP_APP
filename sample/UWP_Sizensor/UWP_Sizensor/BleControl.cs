using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP_Sizensor
{
    class BleControl
    {
        private MainPage rootPage;

        private ObservableCollection<BluetoothLEDeviceDisplay> KnownDevices = new ObservableCollection<BluetoothLEDeviceDisplay>();
        private List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();

        private DeviceWatcher deviceWatcher;
        private bool subscribedForNotifications = false;
        private GattDeviceService deviceService;
        private BluetoothLEDevice bluetoothLeDevice = null;

        // Only one registered characteristic at a time.
        //private GattCharacteristic registeredCharacteristic;
        private GattPresentationFormat presentationFormat;

        private readonly String DeviceNameSizensor = "iPin_Sizensor";
        private readonly String DeviceNamePro = "iPinRuler2.0_Pro";

        GattCharacteristic NotifyUUID, WriteUUID;

        private string BLE_UUID = "0003cdd0-0000-1000-8000-00805f9b0131";
        private string BLE_WRITE_UUID = "0003cdd2-0000-1000-8000-00805f9b0131";
        private string BLE_NOTIFY_UUID = "0003cdd1-0000-1000-8000-00805f9b0131";

        #region Error Codes
        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)
        #endregion

        public void BleInit(MainPage current)
        {
            rootPage = current;
        }

        public void BleSearching()
        {
            if (deviceWatcher == null || bluetoothLeDevice == null)
            {
                StartBleDeviceWatcher();
                Debug.WriteLine($"Device watcher started.");
            }
            else
            {
                StopBleDeviceWatcher();
                Debug.WriteLine($"Device watcher stopped.");
            }
        }

        public async void BleDisConnecting()
        {
            if (await DisconnectedBluetoothLEDeviceAsync())
                rootPage.ChangeBleIcon(BleConsts.STATE_DISCONNECTED);
        }

        private async Task ConnectDevice(DeviceInformation device)
        {
            Debug.WriteLine("ConnectDevice");
            if (!await ClearBluetoothLEDeviceAsync())
            {
                Debug.WriteLine("Error: Unable to reset state, try again.");
                return;
            }

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device.Id);

                if (bluetoothLeDevice == null)
                {
                    Debug.WriteLine("Failed to connect to device.");
                }
            }
            catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {
                Debug.WriteLine("Bluetooth radio is not on.");
            }

            if (bluetoothLeDevice != null)
            {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;

                    Debug.WriteLine(String.Format("Found {0} services", services.Count));
                    rootPage.ChangeBleIcon(BleConsts.STATE_CONNECTED);

                    foreach (var service in services)
                    {
                        if (DisplayHelpers.GetServiceUUID(service).Equals(BLE_UUID))
                        {
                            IReadOnlyList<GattCharacteristic> characteristics = null;
                            try
                            {
                                // Ensure we have access to the device.
                                var accessStatus = await service.RequestAccessAsync();
                                if (accessStatus == DeviceAccessStatus.Allowed)
                                {
                                    deviceService = service;
                                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                                    var result_Chara = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                                    if (result.Status == GattCommunicationStatus.Success)
                                    {
                                        characteristics = result_Chara.Characteristics;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Error accessing service.");

                                        // On error, act as if there are no characteristics.
                                        characteristics = new List<GattCharacteristic>();
                                    }
                                }
                                else
                                {
                                    // Not granted access
                                    Debug.WriteLine("Error accessing service.");

                                    // On error, act as if there are no characteristics.
                                    characteristics = new List<GattCharacteristic>();

                                }

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Restricted service. Can't read characteristics: " + ex.Message);
                                // On error, act as if there are no characteristics.
                                characteristics = new List<GattCharacteristic>();
                            }

                            foreach (GattCharacteristic c in characteristics)
                            {
                                if (DisplayHelpers.GetCharacteristicUUID(c).Equals(BLE_NOTIFY_UUID))
                                {
                                    NotifyUUID = c;
                                    Debug.WriteLine("Get Notify UUID");
                                    SubscribeService();
                                }
                                else if (DisplayHelpers.GetCharacteristicUUID(c).Equals(BLE_WRITE_UUID))
                                {
                                    WriteUUID = c;
                                    Debug.WriteLine("Get Write UUID");
                                }
                                else
                                {
                                    Debug.WriteLine("!!!!" + DisplayHelpers.GetCharacteristicUUID(c));
                                }

                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Device unreachable");
                }
            }
        }

        /// Starts a device watcher that looks for all nearby Bluetooth devices (paired or unpaired). 
        /// Attaches event handlers to populate the device collection.
        private void StartBleDeviceWatcher()
        {
            // Additional properties we would like about the device.
            // Property strings are documented here https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            // BT_Code: Example showing paired and non-paired in a single query.
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                        aqsAllBluetoothLEDevices,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            KnownDevices.Clear();

            // Start the watcher. Active enumeration is limited to approximately 30 seconds.
            // This limits power usage and reduces interference with other Bluetooth activities.
            // To monitor for the presence of Bluetooth LE devices for an extended period,
            // use the BluetoothLEAdvertisementWatcher runtime class. See the BluetoothAdvertisement
            // sample for an example.
            deviceWatcher.Start();
        }

        /// Stops watching for all nearby Bluetooth devices.
        private void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        private async Task<bool> DisconnectedBluetoothLEDeviceAsync()
        {
            Debug.WriteLine("ClearBluetoothLEDeviceAsync");
            if (subscribedForNotifications)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                var result = await NotifyUUID.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (result != GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("result xxx");
                    return false;
                }
                else
                {
                    NotifyUUID.ValueChanged -= Characteristic_ValueChanged;
                    //WriteUUID.ValueChanged -= Characteristic_ValueChanged;
                    subscribedForNotifications = false;
                    Debug.WriteLine("result oooo");

                }
            }

            deviceService?.Dispose();
            bluetoothLeDevice?.Dispose();
            Debug.WriteLine("Disconnect!!!");
            bluetoothLeDevice = null;
            NotifyUUID = null;
            WriteUUID = null;
            return true;
        }

        private async Task<bool> ClearBluetoothLEDeviceAsync()
        {
            Debug.WriteLine("ClearBluetoothLEDeviceAsync");
            if (subscribedForNotifications)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                var result = await NotifyUUID.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (result != GattCommunicationStatus.Success)
                {
                    return false;
                }
                else
                {
                    NotifyUUID.ValueChanged -= Characteristic_ValueChanged;
                    subscribedForNotifications = false;
                }
            }

            NotifyUUID = null;
            WriteUUID = null;
            deviceService?.Dispose();
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;
            return true;
        }

        private void AddValueChangedHandler()
        {
            if (!subscribedForNotifications)
            {
                NotifyUUID.ValueChanged += Characteristic_ValueChanged;
                subscribedForNotifications = true;
            }
        }

        private async void SubscribeService()
        {
            if (!subscribedForNotifications)
            {
                // initialize status
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                if (NotifyUUID.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }

                else if (NotifyUUID.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

                try
                {
                    // BT_Code: Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await NotifyUUID.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                    if (status == GattCommunicationStatus.Success)
                    {
                        AddValueChangedHandler();
                        Debug.WriteLine("Successfully subscribed for value changes");
                    }
                    else
                    {
                        Debug.WriteLine($"Error registering for value changes: {status}");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                try
                {
                    // BT_Code: Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // Note that this sample configures either Indicate or Notify, but not both.
                    var result = await NotifyUUID.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result == GattCommunicationStatus.Success)
                    {
                        subscribedForNotifications = false;
                        RemoveValueChangedHandler();
                        Debug.WriteLine("Successfully un-registered for notifications");
                    }
                    else
                    {
                        Debug.WriteLine($"Error un-registering for notifications: {result}");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                    Debug.WriteLine("UnauthorizedAccessException:" + ex.Message);
                }
            }
        }

        private void RemoveValueChangedHandler()
        {
            if (subscribedForNotifications)
            {
                NotifyUUID.ValueChanged -= Characteristic_ValueChanged;
                NotifyUUID = null;
                subscribedForNotifications = false;
            }
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.

            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
            Debug.WriteLine(newValue.ToString());
            rootPage.ShowDistance(newValue.ToString());
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Added {0}{1}", deviceInfo.Id, deviceInfo.Name));

                    if (deviceInfo.Name == DeviceNameSizensor || deviceInfo.Name == DeviceNamePro)
                    {
                        Debug.WriteLine("Get ====");
                        rootPage.ChangeBleIcon(BleConsts.STATE_CONNECTING);
                        StopBleDeviceWatcher();
                        _ = ConnectDevice(deviceInfo);
                    }
                    else 
                    {
                        rootPage.ChangeBleIcon(BleConsts.STATE_DISCONNECTED);
                    }
                }
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Updated {0}{1}", deviceInfoUpdate.Id, ""));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == deviceWatcher)
                    {
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            // Device is already being displayed - update UX.
                            bleDeviceDisplay.Update(deviceInfoUpdate);
                            return;
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            deviceInfo.Update(deviceInfoUpdate);
                            // If device has been updated with a friendly name it's no longer unknown.
                            if (deviceInfo.Name != String.Empty)
                            {
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                                UnknownDevices.Remove(deviceInfo);
                            }
                        }
                    }
                }
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Removed {0}{1}", deviceInfoUpdate.Id, ""));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == deviceWatcher)
                    {
                        // Find the corresponding DeviceInformation in the collection and remove it.
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            KnownDevices.Remove(bleDeviceDisplay);
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            UnknownDevices.Remove(deviceInfo);
                        }
                    }
                }
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    Debug.WriteLine($"{KnownDevices.Count} devices found. Enumeration completed.");
                    BleSearching();
                }
            });
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    Debug.WriteLine($"No longer watching for devices.",
                            sender.Status == DeviceWatcherStatus.Aborted ? "Error Msg" : "Status Msg");
                }
            });
        }

        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        public async Task BleSend(String str)
        {
            if (bluetoothLeDevice.ConnectionStatus != BleConsts.STATE_DISCONNECTED)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    var writeBuffer = CryptographicBuffer.ConvertStringToBinary(str,
                      BinaryStringEncoding.Utf8);

                    var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);
                }
            }
            else
            {
                rootPage.ChangeBleIcon(BleConsts.STATE_DISCONNECTED);
                await ClearBluetoothLEDeviceAsync();
            }
        }

        public bool getBleSatus()
        {
            if (bluetoothLeDevice.ConnectionStatus == BleConsts.STATE_DISCONNECTED)
                return false;
            else
                return true;
        }

        /* Read Data */
        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {
            if (bluetoothLeDevice.ConnectionStatus == BleConsts.STATE_DISCONNECTED)
            {
                rootPage.ChangeBleIcon(BleConsts.STATE_DISCONNECTED);
                return "Deivce Disconnected";
            }

            // BT_Code: For the purpose of this sample, this function converts only UInt32 and
            // UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            if (format != null)
            {
                if (format.FormatType == GattPresentationFormatTypes.UInt32 && data.Length >= 4)
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf8)
                {
                    try
                    {
                        return Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "(error: Invalid UTF-8 string)";
                    }
                }
                else
                {
                    // Add support for other format types as needed.
                    return "Unsupported format: " + CryptographicBuffer.EncodeToHexString(buffer);
                }
            }
            else if (data != null)
            {
                try
                {
                    //BitConverter.ToInt32(data, 0).ToString();
                    String str = Encoding.UTF8.GetString(data);
                    return str;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Notify Exception" + e.ToString());
                    return "Unknown format";
                }
            }
            else
            {
                return "Empty data received";
            }
        }

        private async Task<bool> WriteBufferToSelectedCharacteristicAsync(IBuffer buffer)
        {
            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await WriteUUID.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("Successfully wrote value to device");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"Write failed: {result.Status}");
                    return false;
                }
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                Debug.WriteLine("Err:" + ex.Message);
                return false;
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED || ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
