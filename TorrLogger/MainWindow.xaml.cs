using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TorrLogger.Managers;
using TorrLogger.ViewModels;
using System.Diagnostics;
using System.Dynamic;

namespace TorrLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // configure notify icon
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.AppIcon;
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick += new EventHandler(Icon_DoubleClick);

            _priorWindowState = WindowState;

            dgTorrents.ItemsSource = ViewManager.Instance.TorrentViewModels;
            dgClients.ItemsSource = ViewManager.Instance.ClientViewModels;
            dgExport.ItemsSource = ViewManager.Instance.ClientViewModelsForExport;
            ViewManager.Instance.ClientsGrid = dgClients;
        }

        // properties
        private NotifyIcon _notifyIcon;
        private WindowState _priorWindowState;

        private void OpenImportDialog()
        {
            ImportWindow importWindow = new ImportWindow();
            importWindow.ShowDialog();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenImportDialog();
        }

        void Icon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            if(WindowState == WindowState.Minimized)
            {
                WindowState = _priorWindowState;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            else
            {
                _priorWindowState = WindowState;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenImportDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = dgTorrents.SelectedIndex;
            if(selectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Please select torrent to delete.", "Empty selection");
                return;
            }

            // do something with selectedIndex;
            //throw new Exception("Not implemented yet");
            var clientViewModel = new ClientViewModel { No = ViewManager.Instance.ClientViewModels.Count, IpAddress = "1.2.3.4", Port = 4567, Client = "peer.ClientApp.Client.ToString()", Title = "Title", FileHash = "hashahsh", DateTime = DateTime.Now };

            ViewManager.Instance.ClientViewModels.Add(clientViewModel);

        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl != e.Source as System.Windows.Controls.TabControl) {
                return;
            }

            if(tabControl.SelectedIndex == 1)
            {
                cbISP.Items.Clear();
                cbISP.Items.Add("All");
                List<string> isps = TorrentsManager.Instance.GetAllIsps();
                foreach(var isp in isps)
                {
                    cbISP.Items.Add(isp);
                }
                cbISP.SelectedIndex = 0;

                cbTitle.Items.Clear();
                cbTitle.Items.Add("All");
                List<string> titles = TorrentsManager.Instance.GetAllTitles();
                foreach(var title in titles)
                {
                    cbTitle.Items.Add(title);
                }
                cbTitle.SelectedIndex = 0;
            }
        }

        private void cbISP_Or_cbTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string isp;
            try
            {
                isp = cbISP.SelectedItem.ToString();
            }
            catch
            {
                isp = "All";
            }
            string name;
            try
            {
                name = cbTitle.SelectedItem.ToString();
            }
            catch
            {
                name = "All";
            }
            TorrentsManager.Instance.GetClientViewModelsFromIspAndName(isp, name, ViewManager.Instance.ClientViewModelsForExport);
        }
    }
}
