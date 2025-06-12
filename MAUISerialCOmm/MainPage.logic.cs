using MAUISerialCOmm.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MAUISerialCOmm
{
    public partial class MainPage : ContentPage
    {
        private readonly IUsbSerialService _usbSerialService;
        public ObservableCollection<string> DeviceNames { get; set; } = new();
        private string _selectedDevice;

        public MainPage()
        {
            InitializeComponent();
            _usbSerialService = new UsbSerialService();
            _usbSerialService.DataReceived += UsbSerialService_DataReceived;
            DevicePicker.ItemsSource = DeviceNames;
            DevicePicker.SelectedIndexChanged += DevicePicker_SelectedIndexChanged;
            LoadDevices();
        }

        private async void LoadDevices()
        {
            var devices = await _usbSerialService.GetAvailableDeviceNamesAsync();
            DeviceNames.Clear();
            foreach (var d in devices)
                DeviceNames.Add(d);
            if (DeviceNames.Count > 0)
                DevicePicker.SelectedIndex = 0;
        }

        private void DevicePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DevicePicker.SelectedIndex >= 0)
                _selectedDevice = DeviceNames[DevicePicker.SelectedIndex];
        }

        private async void OnConnectClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedDevice))
            {
                StatusLabel.Text = "Status: Please select a device";
                return;
            }
            var success = await _usbSerialService.ConnectAsync(_selectedDevice);
            StatusLabel.Text = success ? $"Status: Connected to {_selectedDevice}" : "Status: Failed to connect";
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var text = SendEntry.Text;
            if (!string.IsNullOrEmpty(text))
            {
                var sent = await _usbSerialService.SendAsync(text);
                StatusLabel.Text = sent ? "Status: Sent" : "Status: Send failed";
            }
        }

        private async void OnDisconnectClicked(object sender, EventArgs e)
        {
            await _usbSerialService.DisconnectAsync();
            StatusLabel.Text = "Status: Disconnected";
        }

        private void UsbSerialService_DataReceived(object sender, string data)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReceivedLabel.Text = $"Received: {data}";
            });
        }
    }
}
