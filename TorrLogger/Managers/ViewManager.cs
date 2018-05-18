using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TorrLogger.Models;
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

        public ObservableCollection<ClientViewModel> ClientViewModels = SQLiteManager.Instance.GetAllHistory();

        public ObservableCollection<ClientViewModel> ClientViewModelsForExport = new ObservableCollection<ClientViewModel>();

        private int lastIndexOfClientViewModel = 0;

        public void AddClientViewModel(ClientModel client)
        {
            if(lastIndexOfClientViewModel == 0)
            {
                lastIndexOfClientViewModel = ClientViewModels.Count;
            }
            var clientViewModel = new ClientViewModel { No = ++lastIndexOfClientViewModel, IpAddress = client.IpAddress, Port = client.Port, Client = client.Client, Title = client.TorrentModel.Name, FileHash = client.TorrentModel.TorrentManager.Torrent.InfoHash.ToString(), Date = DateTime.Now.ToString("dd:MM:yyyy"), Time = DateTime.Now.ToString("HH:mm:ss"), EndDate = DateTime.Now.ToString("dd:MM:yyyy"), EndTime = DateTime.Now.ToString("HH:mm:ss"), Country = client.Country, ISP = client.ISP, IsActive = true };
            Action<ClientViewModel> addMethod = ViewManager.Instance.ClientViewModels.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, clientViewModel);
            SQLiteManager.Instance.InsertHistory(clientViewModel);
        }

        public DataGrid ClientsGrid;
    }
}
