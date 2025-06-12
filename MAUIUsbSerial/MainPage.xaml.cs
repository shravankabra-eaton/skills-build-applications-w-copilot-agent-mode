using System;
using System.Linq;
using MAUIUsbSerial;

namespace MAUIUsbSerial;

public partial class MainPage : ContentPage
{
    private readonly IUsbSerialService _usbSerialService;

    public MainPage()
    {
        InitializeComponent();
        _usbSerialService = GetUsbSerialService();
        UpdateStatus();
    }

    private IUsbSerialService GetUsbSerialService()
    {
#if ANDROID
        return new Platforms.Android.UsbSerialService();
#elif WINDOWS
        return new Platforms.Windows.UsbSerialService();
#else
        throw new NotImplementedException("USB Serial not implemented for this platform.");
#endif
    }

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        var deviceId = DeviceIdEntry.Text;
        int.TryParse(BaudRateEntry.Text, out int baudRate);
        var result = await _usbSerialService.ConnectAsync(deviceId, baudRate);
        await DisplayAlert("Connect", result ? "Connected" : "Failed to connect", "OK");
        UpdateStatus();
    }

    private async void OnWriteClicked(object sender, EventArgs e)
    {
        var dataText = WriteDataEntry.Text;
        if (string.IsNullOrWhiteSpace(dataText)) return;
        var bytes = dataText.Split(',').Select(s => byte.TryParse(s.Trim(), out var b) ? b : (byte)0).ToArray();
        var written = await _usbSerialService.WriteAsync(bytes);
        await DisplayAlert("Write", $"Bytes written: {written}", "OK");
    }

    private async void OnReadClicked(object sender, EventArgs e)
    {
        int.TryParse(ReadCountEntry.Text, out int count);
        var bytes = await _usbSerialService.ReadAsync(count);
        ReadResultLabel.Text = $"Read Result: {string.Join(", ", bytes)}";
    }

    private async void OnDisconnectClicked(object sender, EventArgs e)
    {
        await _usbSerialService.DisconnectAsync();
        await DisplayAlert("Disconnect", "Disconnected", "OK");
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        ConnectionStatusLabel.Text = $"Status: {(_usbSerialService.IsConnected ? "Connected" : "Disconnected" )}";
    }

    private async void OnOpenSetPointClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MAUIUsbSerial.SetPointConnect.MainScreen_MAUI());
    }
}
