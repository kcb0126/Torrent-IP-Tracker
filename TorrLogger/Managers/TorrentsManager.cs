using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        //ObservableCollection<UTorrentWebClient>
    }
}
