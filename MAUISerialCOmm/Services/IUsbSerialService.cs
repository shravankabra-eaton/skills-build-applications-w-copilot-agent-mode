namespace MAUISerialCOmm.Services
{
    public interface IUsbSerialService
    {
        Task<List<string>> GetAvailableDeviceNamesAsync();
        Task<bool> ConnectAsync(string deviceName);
        Task DisconnectAsync();
        Task<bool> SendAsync(string data);
        event EventHandler<string> DataReceived;
        bool IsConnected { get; }
    }
}
