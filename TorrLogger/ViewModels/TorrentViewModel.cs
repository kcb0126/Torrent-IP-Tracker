﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.ViewModels
{
    class TorrentViewModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int Seeds { get; set; }
        public int Leechs { get; set; }
    }
}
