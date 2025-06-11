using MAUIUsbSerial;
using System.IO.Ports;

namespace MAUIUsbSerial.Platforms.Windows;

public class UsbSerialService : IUsbSerialService
{
    private SerialPort? _serialPort;
    public bool IsConnected => _serialPort?.IsOpen ?? false;

    public async Task<bool> ConnectAsync(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate);
        _serialPort.Open();
        return _serialPort.IsOpen;
    }

    public async Task<int> WriteAsync(byte[] data)
    {
        if (_serialPort == null || !_serialPort.IsOpen) return 0;
        _serialPort.Write(data, 0, data.Length);
        return data.Length;
    }

    public async Task<byte[]> ReadAsync(int count)
    {
        if (_serialPort == null || !_serialPort.IsOpen) return new byte[0];
        byte[] buffer = new byte[count];
        int bytesRead = _serialPort.Read(buffer, 0, count);
        return buffer.Take(bytesRead).ToArray();
    }

    public async Task DisconnectAsync()
    {
        if (_serialPort != null && _serialPort.IsOpen)
            _serialPort.Close();
    }
}
