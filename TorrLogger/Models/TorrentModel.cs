using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.Models
{
    class TorrentModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public TorrentManager TorrentManager { get; set; }
    }
}
