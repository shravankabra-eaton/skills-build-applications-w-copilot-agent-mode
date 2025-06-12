using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAUISerialCOmm.Services;
using Xunit;

namespace MAUISerialCOmm.Tests
{
    public class UsbSerialServiceTests
    {
        [Fact]
        public async Task GetAvailableDeviceNamesAsync_ReturnsList()
        {
            var service = new UsbSerialService();
            var devices = await service.GetAvailableDeviceNamesAsync();
            Assert.NotNull(devices);
            Assert.IsType<List<string>>(devices);
        }

        [Fact]
        public async Task ConnectAsync_ReturnsFalse_WhenDeviceNameIsNull()
        {
            var service = new UsbSerialService();
            var result = await service.ConnectAsync(null);
            Assert.False(result);
        }

        [Fact]
        public async Task ConnectAsync_ReturnsFalse_WhenDeviceNameIsInvalid()
        {
            var service = new UsbSerialService();
            var result = await service.ConnectAsync("INVALID_DEVICE");
            Assert.False(result);
        }

        [Fact]
        public async Task DisconnectAsync_SetsIsConnectedFalse()
        {
            var service = new UsbSerialService();
            await service.DisconnectAsync();
            Assert.False(service.IsConnected);
        }

        [Fact]
        public async Task SendAsync_ReturnsFalse_WhenNotConnected()
        {
            var service = new UsbSerialService();
            var result = await service.SendAsync("test");
            Assert.False(result);
        }

        [Fact]
        public async Task ConnectAsync_SetsIsConnectedTrue_WhenDeviceNameIsValid_Windows()
        {
#if WINDOWS
            var service = new UsbSerialService();
            // Use a valid port name if available, otherwise skip
            var devices = await service.GetAvailableDeviceNamesAsync();
            if (devices.Count == 0)
                return; // No ports to test
            var result = await service.ConnectAsync(devices[0]);
            Assert.True(result);
            Assert.True(service.IsConnected);
            await service.DisconnectAsync();
#endif
        }

        [Fact]
        public async Task ConnectAsync_SetsIsConnectedTrue_WhenDeviceNameIsValid_Android()
        {
#if ANDROID
            var service = new UsbSerialService();
            // Use a valid device name if available, otherwise skip
            var devices = await service.GetAvailableDeviceNamesAsync();
            if (devices.Count == 0)
                return; // No devices to test
            var result = await service.ConnectAsync(devices[0]);
            Assert.True(result);
            Assert.True(service.IsConnected);
            await service.DisconnectAsync();
#endif
        }

        [Fact]
        public async Task DisconnectAsync_ResetsConnection_Windows()
        {
#if WINDOWS
            var service = new UsbSerialService();
            var devices = await service.GetAvailableDeviceNamesAsync();
            if (devices.Count == 0)
                return;
            await service.ConnectAsync(devices[0]);
            await service.DisconnectAsync();
            Assert.False(service.IsConnected);
#endif
        }

        [Fact]
        public async Task DisconnectAsync_ResetsConnection_Android()
        {
#if ANDROID
            var service = new UsbSerialService();
            var devices = await service.GetAvailableDeviceNamesAsync();
            if (devices.Count == 0)
                return;
            await service.ConnectAsync(devices[0]);
            await service.DisconnectAsync();
            Assert.False(service.IsConnected);
#endif
        }
    }
}
