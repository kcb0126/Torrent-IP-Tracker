using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
