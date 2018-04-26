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

            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to remove the selected torrent?", "TorrLogger", System.Windows.MessageBoxButton.YesNo);
            if(result == MessageBoxResult.No)
            {
                return;
            }

            // do something with selectedIndex;
            //throw new Exception("Not implemented yet");
            ViewManager.Instance.TorrentViewModels.RemoveAt(selectedIndex);
            TorrentsManager.Instance.RemoveTorrentAt(selectedIndex);
        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl != e.Source as System.Windows.Controls.TabControl) {
                return;
            }

            if(tabControl.SelectedIndex == 1)
            {
                // TorrentsManager.Instance.AddTestClients();
                cbISP.Items.Clear();
                cbISP.Items.Add("All");
                List<string> isps = TorrentsManager.Instance.GetAllIsps();
                isps.Sort();
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

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel Workbook(*.xlsx)|*.xlsx";
            dlg.Title = "Save";

            DialogResult result = dlg.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                ExcelManager.Instance.SaveClientViewModels(ViewManager.Instance.ClientViewModelsForExport, dlg.FileName);
            }
        }
    }
}
