using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SmartHouse.Entities;
using SmartHouse.Printer.PrinterSettings;
using SmartHouse.Udp.Client;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog;


namespace SmartHouse.Printer.PrinterConnect
{
    public class Connect : IConnect
    {
        private const string On = "On";

        private const string Off = "Off";

        private DeviceUdp Device { get; set; }

        public HubConnection Connection { get; set; }

        private ISettings PrinterSettings { get; set; }


        private Logger _logger = LogManager.GetCurrentClassLogger();

        private byte[] Image { get; set; }
        private string Url { get; set; }
        public Connect(ISettings settings)
        {
            PrinterSettings = settings;
            var Port = JsonConvert.DeserializeObject<ConnectModel>(File.ReadAllText(Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", @"Configs\PortConfig.json"))));

            Device = new DeviceUdp(Port.PortToSend);
            Url = Device.GetContract().Url;
            Connection = new HubConnectionBuilder().WithUrl(Url).Build();
        }

        public async Task ClientConnect()
        {
            await Connection.StartAsync();
            if (Connection.State == HubConnectionState.Connected)   
            {
                Connection.Closed += Connection_Closed;
            }

            //resgistering the events  
            Connection.On<byte[]>("Print", PrintOn);
            Connection.On(On, async () => await DeviceOn());
            Connection.On(Off, async () => await DeviceOff());
            await Connection.InvokeAsync(Device.GetContract().Device, PrinterSettings.InitDevice());
        }
        private async Task Connection_Closed(Exception arg)
        {
            if (Connection.State == HubConnectionState.Disconnected)
            {
                // specify a retry duration
                while (true)
                {
                    try
                    {
                        await ClientConnect();
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }

                    Console.WriteLine("Connection closed");
                }
            }
        }

        public void PrintOn(byte[] args)
        {
            if (args != null)
            {
                Image = args;
                OnPropertyChanged("Image");
            }
        }
        public async Task DeviceOn()
        {
            await Connection.InvokeAsync("StatusChanger", PrinterSettings.InitDevice().MAC, SlaveStatus.On);
            _logger.Info("Device On");
        }

        public async Task DeviceOff()
        {
            await Connection.InvokeAsync("StatusChanger", PrinterSettings.InitDevice().MAC, SlaveStatus.Off);
            _logger.Info("Device Off");
        }

        public byte[] GetBytesImage()
        {
            return Image;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
