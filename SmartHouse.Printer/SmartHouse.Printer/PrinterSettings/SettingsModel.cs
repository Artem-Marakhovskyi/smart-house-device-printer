using SmartHouse.Entities;

namespace SmartHouse.Printer.PrinterSettings
{
    public class SettingsModel
    {
        public string MAC { get; set; }

        public string Name { get; set; }

        public HouseSlaveInvoker On { get; set; }

        public HouseSlaveInvoker Off { get; set; }
    }
}
