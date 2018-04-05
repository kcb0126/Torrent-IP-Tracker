using Microsoft.Win32;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TorrLogger.Managers;
using TorrLogger.ViewModels;

namespace TorrLogger
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private Torrent torrent;

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
            dlg.Multiselect = false;
            dlg.Filter = "Torrents(*.torrent)|*.torrent|All files(*.*)|*.*";
            dlg.Title = "Select a .torrent to open";

            bool? result = dlg.ShowDialog();

            if(result == true)
            {
                txtFileName.Text = dlg.FileName;
            }
        }

        private void txtFileName_Changed(object sender, TextChangedEventArgs e)
        {
            lblName.Content = "";
            lblSize.Content = "";
            lblHash.Content = "";
            lblPath.Content = "";

            try
            {
                torrent = Torrent.Load(txtFileName.Text);
                lblName.Content = Path.GetFileName(txtFileName.Text);
                double size = torrent.Size;
                string sizeUnit = "";
                if(size >= 1024000)
                {
                    size = size / (1024 * 1024);
                    sizeUnit = "MB";
                }
                else if(size >= 1000)
                {
                    size = size / 1024;
                    sizeUnit = "KB";
                }
                else
                {
                    sizeUnit = "bytes";
                }
                lblSize.Content = string.Format("{0:0.##} {1}", size, sizeUnit);
                lblHash.Content = torrent.InfoHash.ToString();
                lblPath.Content = Path.GetDirectoryName(txtFileName.Text);
                btnImport.IsEnabled = true;
            }
            catch (Exception ex)
            {
                btnImport.IsEnabled = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
