using MonoTorrent.Client;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrLogger.Models;

namespace TorrLogger.Managers
{
    class TorrentsManager
    {
        public static TorrentsManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TorrentsManager();
                }
                return _instance;
            }
        }
        private static TorrentsManager _instance = null;

        public TorrentsManager()
        {

        }

        public void AddTorrent(string fileName, string name, Torrent torrent)
        {
            TorrentManager manager = new TorrentManager(torrent, "", torrentDefaults);
        }

        private ObservableCollection<TorrentModel> torrentModels = new ObservableCollection<TorrentModel>();

        private TorrentSettings torrentDefaults = new TorrentSettings(4, 150, 0, 0);

        private void models_Changed()
        {
            ViewManager.Instance.TorrentViewModels.Clear();
            int nNo = 0;
            foreach(TorrentModel model in torrentModels)
            {
                double size = model.TorrentManager.Torrent.Size;
                string sizeUnit = "";
                if (size >= 1048576000)
                {
                    size = size / (1024 * 1024 * 1024);
                    sizeUnit = "GB";
                }
                else if (size >= 1024000)
                {
                    size = size / (1024 * 1024);
                    sizeUnit = "MB";
                }
                else if (size >= 1000)
                {
                    size = size / 1024;
                    sizeUnit = "KB";
                }
                else
                {
                    sizeUnit = "bytes";
                }

                ViewManager.Instance.TorrentViewModels.Add(new ViewModels.TorrentViewModel { No = ++nNo, Name = model.Name, Seeds = model.TorrentManager.Peers.Seeds, Leechs = model.TorrentManager.Peers.Leechs, Size = string.Format("{0:0.##} {1}", size, sizeUnit) });
            }
        }
    }
}
