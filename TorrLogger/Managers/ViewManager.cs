using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TorrLogger.ViewModels;

namespace TorrLogger.Managers
{
    class ViewManager
    {
        public static ViewManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ViewManager();
                }
                return _instance;
            }
        }
        private static ViewManager _instance = null;

        public ObservableCollection<TorrentViewModel> TorrentViewModels = new ObservableCollection<TorrentViewModel>();

        public ObservableCollection<ClientViewModel> ClientViewModels = new ObservableCollection<ClientViewModel>();

        public ObservableCollection<ClientViewModel> ClientViewModelsForExport = new ObservableCollection<ClientViewModel>();

        private int lastIndexOfClientViewModel = 0;

        public void AddClientViewModel(string ip, int port, string title, string client, string hash, string isp, string country)
        {
            var clientViewModel = new ClientViewModel { No = ++lastIndexOfClientViewModel, IpAddress = ip, Port = port, Client = client, Title = title, FileHash = hash, Date = DateTime.Now.ToString("dd:MM:yyyy"), Time = DateTime.Now.ToString("HH:mm:ss"), Country = country, ISP = isp };
            Action<ClientViewModel> addMethod = ViewManager.Instance.ClientViewModels.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, clientViewModel);
        }

        public DataGrid ClientsGrid;
    }
}
