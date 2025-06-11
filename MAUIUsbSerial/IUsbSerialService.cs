namespace MAUIUsbSerial;

public interface IUsbSerialService
{
    Task<bool> ConnectAsync(string deviceId, int baudRate);
    Task<int> WriteAsync(byte[] data);
    Task<byte[]> ReadAsync(int count);
    Task DisconnectAsync();
    bool IsConnected { get; }
}
