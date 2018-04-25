using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using MonoTorrent.Dht;
using MonoTorrent.Dht.Listeners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TorrLogger.Models;
using TorrLogger.ViewModels;

namespace TorrLogger.Managers
{
    class TorrentsManager
    {
        public static TorrentsManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TorrentsManager();
                }
                return _instance;
            }
        }
        private static TorrentsManager _instance = null;

        public TorrentsManager()
        {
            // directories for torrent node
            string basePath = Environment.CurrentDirectory;
//            downloadsPath = Path.Combine(basePath, "Downloads");
            downloadsPath = "Q:\\fakedownloaddir";
            //fastResumeFile = Path.Combine(basePath, "fastresume.data"); // remove fast resume
            dhtNodeFile = Path.Combine(basePath, "DhtNodes");

            // Create the settings which the engine will use
            // downloadsPath - this is the path where we will save all the files to
            // port - this is the port we listen for connections on
            engineSettings = new EngineSettings(downloadsPath, port);
            engineSettings.PreferEncryption = false;
            engineSettings.AllowedEncryption = EncryptionTypes.All;

            // Create an instance of the engine.
            engine = new ClientEngine(engineSettings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, port));
            byte[] nodes = null;
            try
            {
                nodes = File.ReadAllBytes(dhtNodeFile);
            }
            catch
            {
                Debug.WriteLine("No existing dht nodes could be loaded");
            }

            DhtListener dhtListener = new DhtListener(new IPEndPoint(IPAddress.Any, port));
            DhtEngine dht = new DhtEngine(dhtListener);
            engine.RegisterDht(dht);
            dhtListener.Start();
            engine.DhtEngine.Start(nodes);

            // If the SavePath does not exist, we want to create it.
            //if (!Directory.Exists(engine.Settings.SavePath))
            //    Directory.CreateDirectory(engine.Settings.SavePath);

            try
            {
                //fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(fastResumeFile)); // remove fast resume
            }
            catch
            {
                //fastResume = new BEncodedDictionary(); // remove fast resume
            }

            // We need to cleanup correctly when the user closes the window
            // or an unhandled exception happens

            //AppDomain.CurrentDomain.ProcessExit += delegate { shutdown(); };
            //AppDomain.CurrentDomain.UnhandledException += delegate { shutdown(); };
            //Thread.GetDomain().UnhandledException += delegate { shutdown(); };
        }

        string downloadsPath;
        //string fastResumeFile; // remove fast resume
        string dhtNodeFile;

        int port = 4567;

        EngineSettings engineSettings;
        ClientEngine engine;

        //BEncodedDictionary fastResume; // remove fast resume

        public void AddTestClients()
        {
            if (torrentModels.Count == 0)
            {
                return;
            }
            string[] isps = new string[] { "First ISP", "second ISP", "Third ISP", "forth ISP", "Fifth ISP", "sixth ISP", "Seventh ISP", "eighth ISP", "Ninth ISP", "tenth ISP" };
            for (int i = 0; i < 10; i ++)
            {
                clientModels.Add(new ClientModel { TorrentModel = torrentModels[0], IpAddress = "12.34.56.78", Port = 12345, Client = "Client", DateTime = DateTime.Now, ISP = isps[i], Country = "Test Country" });
            }
        }

        public List<string> GetAllIsps()
        {
            List<string> isps = new List<string>();
            foreach(ClientModel client in clientModels)
            {
                if(!isps.Contains(client.ISP))
                {
                    isps.Add(client.ISP);
                }
            }
            return isps;
        }

        public List<string> GetAllTitles()
        {
            List<string> titles = new List<string>();
            foreach(TorrentModel torrent in torrentModels)
            {
                titles.Add(torrent.Name);
            }
            return titles;
        }

        public void GetClientViewModelsFromIspAndName(string isp, string name, ObservableCollection<ClientViewModel> viewModels)
        {
            viewModels.Clear();
            int nNo = 0;
            foreach(ClientModel model in clientModels)
            {
                if(isp == "All" || isp == model.ISP)
                {
                    if(name == "All" || name == model.TorrentModel.Name)
                    {
                        viewModels.Add(new ClientViewModel { No = ++nNo, IpAddress = model.IpAddress, Port = model.Port, Title = model.TorrentModel.Name, Client = model.Client, FileHash = model.TorrentModel.TorrentManager.Torrent.InfoHash.ToString(), DateTime = model.DateTime, Country = model.Country, ISP = model.ISP });
                    }
                }
            }
        }

        /// <summary>
        /// Add torrent into my torrent list and return success code.
        /// </summary>
        /// <param name="fileName">filename of torrent</param>
        /// <param name="name">name to export as excel document</param>
        /// <param name="torrent">torrent object</param>
        /// <returns>0: success, 1: duplicated torrent, 2: duplicated name, 3: unexpected error</returns>
        public int AddTorrent(string fileName, string name, Torrent torrent)
        {
            int maxId = -1;

            // check validation
            foreach(TorrentModel model in torrentModels)
            {
                if(model.FileName.Equals(fileName))
                {
                    return 1;
                }
                if (model.TorrentManager.Torrent.InfoHash.Equals(torrent.InfoHash))
                {
                    return 1;
                }
                if(model.Name.Equals(name))
                {
                    return 2;
                }

                if(model.Id > maxId)
                {
                    maxId = model.Id;
                }
            }

            // When any preprocessing has been completed, you create a TorrentManager
            // which you then register with the engine.
            TorrentManager manager = new TorrentManager(torrent, downloadsPath, torrentDefaults);
            //if(fastResume.ContainsKey(torrent.InfoHash.ToHex())) // remove fast resume
            //{ // remove fast resume
            //    manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.InfoHash.ToHex()])); // remove fast resume
            //} // remove fast resume
            engine.Register(manager);
            manager.PeersFound += new EventHandler<PeersAddedEventArgs>(manager_PeersFound);

            // Store the torrent manager in our list so we can access it later
            TorrentModel newModel = new TorrentModel { FileName = fileName, Name = name, TorrentManager = manager, Id = maxId + 1 };
            torrentModels.Add(newModel);

            // Start the torrentmanager. The file will then hash (if required) and begin downloading/seeding
            manager.Start();

            models_Changed();

            return 0;
        }

        public void RemoveTorrentAt(int index)
        {
            var model = torrentModels[index];
            torrentModels.RemoveAt(index);
            //var manager = model.TorrentManager;
            //manager.Stop();
            //while (manager.State != TorrentState.Stopped)
            //{
            //    Debug.WriteLine("{0} is {1}", model.Name, manager.State);
            //    Thread.Sleep(250);
            //}
            //fastResume.Add(manager.Torrent.InfoHash.ToHex(), manager.SaveFastResume().Encode()); // remove fast resume
        }

        private List<TorrentModel> torrentModels = new List<TorrentModel>();
        private List<ClientModel> clientModels = new List<ClientModel>();

        private TorrentSettings torrentDefaults = new TorrentSettings(4, 150, 0, 0);

        private void models_Changed()
        {
            ViewManager.Instance.TorrentViewModels.Clear();
            int nNo = 0;
            foreach(TorrentModel model in torrentModels)
            {
                double size = model.TorrentManager.Torrent.Size;
                string sizeUnit = "";
                if (size >= 1048576000)
                {
                    size = size / (1024 * 1024 * 1024);
                    sizeUnit = "GB";
                }
                else if (size >= 1024000)
                {
                    size = size / (1024 * 1024);
                    sizeUnit = "MB";
                }
                else if (size >= 1000)
                {
                    size = size / 1024;
                    sizeUnit = "KB";
                }
                else
                {
                    sizeUnit = "bytes";
                }

                ViewManager.Instance.TorrentViewModels.Add(new ViewModels.TorrentViewModel { No = ++nNo, Name = model.Name, Hash = model.TorrentManager.Torrent.InfoHash.ToString(), Size = string.Format("{0:0.##} {1}", size, sizeUnit) });
            }
        }

        static void manager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            Debug.WriteLine(string.Format("Found {0} new peers and {1} existing peers", e.NewPeers, e.ExistingPeers));//throw new Exception("The method or operation is not implemented.");
            foreach (var peer in e.TorrentManager.GetPeers())
            {
                var uri = peer.ConnectionUri;
                Debug.WriteLine(uri.ToString());
//                Debug.WriteLine(peer.ClientApp.Client.ToString());
                var isExist = false;
                foreach (var client in Instance.clientModels)
                {
                    if (uri.Host.Equals(client.IpAddress) && e.TorrentManager.Torrent.InfoHash.Equals(client.TorrentModel.TorrentManager.Torrent.InfoHash))
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist)
                {
                    break;
                }
                dynamic ispAndCountry = Utils.Utils.IspAndCountryFromIp(uri.Host);
                if((string)ispAndCountry.Country == "Unknown")
                {
                    continue;
                }
                //if (ispAndCountry.Country != "Germany")
                //{
                //    break;
                //}
                TorrentModel torrent = null;
                foreach(var tmp in Instance.torrentModels)
                {
                    if(tmp.TorrentManager == e.TorrentManager)
                    {
                        torrent = tmp;
                    }
                }
                if(torrent == null)
                {
                    continue;
                }
                ClientModel newClient = new ClientModel { IpAddress = uri.Host, Port = uri.Port, Client = "peer.ClientApp.Client.ToString()", TorrentModel = torrent, DateTime = DateTime.Now, ISP = ispAndCountry.Isp, Country = ispAndCountry.Country };
                Instance.clientModels.Add(newClient);

                ViewManager.Instance.AddClientViewModel(newClient.IpAddress, newClient.Port, torrent.Name, newClient.Client, torrent.TorrentManager.Torrent.InfoHash.ToString(), (string)ispAndCountry.Isp, (string)ispAndCountry.Country);
            }
        }

        private static void _shutdown()
        {
            //BEncodedDictionary fastResume = new BEncodedDictionary(); // remove fast resume
            foreach (var model in Instance.torrentModels)
            {
                var manager = model.TorrentManager;
                manager.Stop();
                while(manager.State != TorrentState.Stopped)
                {
                    Debug.WriteLine("{0} is {1}", model.Name, manager.State);
                    Thread.Sleep(250);
                }
                //fastResume.Add(manager.Torrent.InfoHash.ToHex(), manager.SaveFastResume().Encode()); // remove fast resume
            }
#if !DISABLE_DHT
            File.WriteAllBytes(Instance.dhtNodeFile, Instance.engine.DhtEngine.SaveNodes());
#endif
            //File.WriteAllBytes(Instance.fastResumeFile, fastResume.Encode()); // remove fast resume
            Instance.engine.Dispose();

            foreach(TraceListener lst in Debug.Listeners)
            {
                lst.Flush();
                lst.Close();
            }

            Thread.Sleep(2000);
        }
    }
}
