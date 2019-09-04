using SmartHouse.Entities;

namespace SmartHouse.Printer.PrinterSettings
{
    public interface ISettings
    {
        Device Printer { get; set; }

        Device InitDevice();
    }
}
