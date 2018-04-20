using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.ViewModels
{
    class ClientViewModel
    {
        public int No { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Client { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string FileHash { get; set; }
    }
}
