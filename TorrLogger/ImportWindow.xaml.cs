using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TorrLogger.Managers;
using TorrLogger.ViewModels;

namespace TorrLogger
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        public ImportWindow()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ViewManager.Instance.TorrentViewModels.Add(new TorrentViewModel { No = 4, Name = "Torrent4", Size = "890", Seeds = 4, Peers = 6 });

            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".torrent";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Torrents(*.torrent)|*.torrent|All files(*.*)|*.*";
            dlg.Title = "Select a .torrent to open";

            bool? result = dlg.ShowDialog();

            if(result == true)
            {
                txtFileName.Text = dlg.FileName;
            }
        }
    }
}
