using Newtonsoft.Json;
using SmartHouse.Entities;
using System;
using System.IO;

namespace SmartHouse.Printer.PrinterSettings
{
    public class Settings:ISettings
    {
        public Device Printer { get; set; }

        public SlaveType Slave { get; set; }

        public Settings()
        {
            Slave = SlaveType.Device;
            Printer = new Device();
        }
        public Device InitDevice()
        {
            var _setting = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", @"Configs\PrinterConfig.json"))));

            Printer.MAC = _setting.MAC;
            Printer.Name = _setting.Name;
            Printer.On = _setting.On;
            Printer.Off = _setting.Off;

            return Printer;
        }
    }
}
