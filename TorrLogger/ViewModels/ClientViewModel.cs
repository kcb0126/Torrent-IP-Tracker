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
        public string Date { get; set; }
        public string Time { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string FileHash { get; set; }
        public string Country { get; set; }
        public string ISP { get; set; }
        public bool IsActive { get; set; }
    }
}
