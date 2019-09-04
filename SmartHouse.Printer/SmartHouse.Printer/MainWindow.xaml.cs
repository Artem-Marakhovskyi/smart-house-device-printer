using Microsoft.Extensions.DependencyInjection;
using SmartHouse.Entities;
using SmartHouse.Printer.PrinterConnect;
using SmartHouse.Printer.PrinterSettings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SmartHouse.Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private byte[] _image;

        private IConnect Connect { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public byte[] Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }
        public MainWindow()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<Device>()
                .AddSingleton<ISettings, Settings>()
                .AddSingleton<IConnect, Connect>()
                .BuildServiceProvider();

            Connect = serviceProvider.GetService<IConnect>();
            Connect.PropertyChanged += Connect_PropertyChanged;
            Connect.ClientConnect();

            InitializeComponent();
        }

        private void Connect_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("Image"))
            {
                Image = Connect.GetBytesImage();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
