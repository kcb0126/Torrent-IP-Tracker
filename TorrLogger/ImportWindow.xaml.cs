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
            int result = TorrentsManager.Instance.AddTorrent(txtFileName.Text, txtName.Text, torrent);
            if(result == 1)
            {
                MessageBox.Show("The torrent you are trying to add is already in the list of torrents.");
                return;
            }
            else if(result == 2)
            {
                MessageBox.Show("The name is duplicated. Please type another name.");
                return;
            }

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
                txtName.Text = torrent.Name;
                lblName.Content = Path.GetFileName(txtFileName.Text);
                double size = torrent.Size;
                lblSize.Content = Utils.Utils.FileSizeExpression(size);
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
