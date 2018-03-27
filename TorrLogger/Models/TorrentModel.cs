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
        public string Name { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public int Seeds { get; set; }
        public int Peers { get; set; }
    }
}
