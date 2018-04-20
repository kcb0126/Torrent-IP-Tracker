using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.Models
{
    class ClientModel
    {
        public TorrentModel TorrentModel { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime DateTime { get; set; }
        public string ISP { get; set; }
    }
}
