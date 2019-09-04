using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SmartHouse.Printer.PrinterConnect
{
    public interface IConnect : INotifyPropertyChanged
    {
        Task ClientConnect();
        byte[] GetBytesImage();
        HubConnection Connection { get; set; }
    }
}
