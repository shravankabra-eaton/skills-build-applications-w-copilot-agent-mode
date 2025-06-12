using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
#if ANDROID
using Android.Hardware.Usb;
using Android.Content;
using Android.App;
using Android.OS;
#endif

namespace MAUISerialCOmm.Services
{
    public class UsbSerialService : IUsbSerialService
    {
        public event EventHandler<string> DataReceived;
        public bool IsConnected { get; private set; }
#if WINDOWS
        private System.IO.Ports.SerialPort _serialPort;
#endif

        public async Task<bool> ConnectAsync()
        {
#if ANDROID
            // Android USB serial connection logic placeholder
            // You would use UsbManager, request permission, and open the device here
            IsConnected = true;
            return true;
#elif WINDOWS
            // Windows USB serial connection logic placeholder
            // You would use System.IO.Ports.SerialPort or Windows.Devices.SerialCommunication
            IsConnected = true;
            return true;
#else
            await Task.CompletedTask;
            return false;
#endif
        }

        public async Task DisconnectAsync()
        {
#if WINDOWS
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            IsConnected = false;
            await Task.CompletedTask;
#elif ANDROID
            // For a real implementation, close the USB connection and release resources
            // If using a serial library, call its close/disconnect method here
            IsConnected = false;
            await Task.CompletedTask;
#else
            IsConnected = false;
            await Task.CompletedTask;
#endif
        }

        public async Task<bool> SendAsync(string data)
        {
            if (!IsConnected) return false;
#if WINDOWS
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Write(data);
                }
            }
            catch
            {
                return false;
            }
            return true;
#else
            // Platform-specific send logic here
            await Task.CompletedTask;
            return true;
#endif
        }

        public async Task<List<string>> GetAvailableDeviceNamesAsync()
        {
#if ANDROID
            // Return list of USB serial device names using Android APIs
            var deviceNames = new List<string>();
            var usbManager = (UsbManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.UsbService);
            var deviceList = usbManager.DeviceList;
            foreach (var device in deviceList.Values)
            {
                // You can customize the display name as needed
                deviceNames.Add($"VID:{device.VendorId:X4} PID:{device.ProductId:X4} (ID:{device.DeviceId})");
            }
            return deviceNames;
#elif WINDOWS
            // Return list of serial port names using System.IO.Ports.SerialPort
            return await Task.Run(() => System.IO.Ports.SerialPort.GetPortNames().ToList());
#else
            await Task.CompletedTask;
            return new List<string>();
#endif
        }

        public async Task<bool> ConnectAsync(string deviceName)
        {
#if ANDROID
            // Connect to the selected Android USB device by name
            var usbManager = (UsbManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.UsbService);
            var deviceList = usbManager.DeviceList;
            UsbDevice selectedDevice = null;
            foreach (var device in deviceList.Values)
            {
                var name = $"VID:{device.VendorId:X4} PID:{device.ProductId:X4} (ID:{device.DeviceId})";
                if (name == deviceName)
                {
                    selectedDevice = device;
                    break;
                }
            }
            if (selectedDevice == null)
                return false;
            // Request permission and open the device for serial communication
            var permissionIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, new Intent("com.companyname.mauiserialcomm.USB_PERMISSION"), 0);
            usbManager.RequestPermission(selectedDevice, permissionIntent);
            // You would typically listen for the permission result via a BroadcastReceiver
            // and then open the device using usbManager.OpenDevice(selectedDevice)
            // For a full serial implementation, consider using a library like 'UsbSerialForAndroid'
            // Here, we assume permission is granted and device is opened (for demo purposes)
            IsConnected = true;
            return true;
#elif WINDOWS
            // Connect to the selected Windows serial port by name
            try
            {
                _serialPort = new System.IO.Ports.SerialPort(deviceName, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                _serialPort.DataReceived += (s, e) =>
                {
                    try
                    {
                        var sp = (System.IO.Ports.SerialPort)s;
                        string data = sp.ReadExisting();
                        DataReceived?.Invoke(this, data);
                    }
                    catch { }
                };
                _serialPort.Open();
                IsConnected = true;
                return true;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
#else
            await Task.CompletedTask;
            return false;
#endif
        }
    }
}
