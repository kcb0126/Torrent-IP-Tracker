using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TorrLogger.Managers;
using TorrLogger.ViewModels;

namespace TorrLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ObservableCollection<TorrentViewModel> torrents = new ObservableCollection<TorrentViewModel>();
            ViewManager.Instance.TorrentViewModels.Add(new TorrentViewModel { No = 1, Name = "Torrent1", Size = "123", Seeds = 1, Peers = 10});
            ViewManager.Instance.TorrentViewModels.Add(new TorrentViewModel { No = 2, Name = "Torrent2", Size = "456", Seeds = 2, Peers = 9});
            ViewManager.Instance.TorrentViewModels.Add(new TorrentViewModel { No = 3, Name = "Torrent3", Size = "789", Seeds = 3, Peers = 8});
            dgTorrents.ItemsSource = ViewManager.Instance.TorrentViewModels;

            ViewManager.Instance.ClientViewModels.Add(new ClientViewModel { No = 1, IpAddress = "0.0.0.0", Client = "BitTorrent1", StartTime = DateTime.Now, Title = "torrent1", FileHash = "1abc353" });
            ViewManager.Instance.ClientViewModels.Add(new ClientViewModel { No = 2, IpAddress = "0.0.1.0", Client = "BitTorrent3", StartTime = DateTime.Today, Title = "torrent2", FileHash = "328fa3" });
            ViewManager.Instance.ClientViewModels.Add(new ClientViewModel { No = 3, IpAddress = "0.43.0.0", Client = "BitTorrent2", StartTime = DateTime.UtcNow, Title = "torrent3", FileHash = "83fde" });
            dgClients.ItemsSource = ViewManager.Instance.ClientViewModels;
        }
    }
}
