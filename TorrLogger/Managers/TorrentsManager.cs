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
            downloadsPath = Path.Combine(basePath, "Downloads");
            fastResumeFile = Path.Combine(basePath, "fastresume.data");
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
            if (!Directory.Exists(engine.Settings.SavePath))
                Directory.CreateDirectory(engine.Settings.SavePath);

            try
            {
                fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(fastResumeFile));
            }
            catch
            {
                fastResume = new BEncodedDictionary();
            }

            workerTorrentsManage.DoWork += WorkerTorrentsManage_DoWork;
            workerTorrentsManage.ProgressChanged += WorkerTorrentsManage_ProgressChanged;
            workerTorrentsManage.WorkerSupportsCancellation = true;
            workerTorrentsManage.WorkerReportsProgress = true;
            workerTorrentsManage.RunWorkerAsync();

            // We need to cleanup correctly when the user closes the window
            // or an unhandled exception happens
            AppDomain.CurrentDomain.ProcessExit += delegate { shutdown(); };
            AppDomain.CurrentDomain.UnhandledException += delegate { shutdown(); };
            Thread.GetDomain().UnhandledException += delegate { shutdown(); };
        }

        private void WorkerTorrentsManage_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            foreach(var torrent in torrentModels)
            {
                foreach(var peer in torrent.TorrentManager.GetPeers())
                {
                    var uri = peer.Uri;
                    var isExist = false;
                    foreach(var client in clientModels)
                    {
                        if(uri.Host.Equals(client.IpAddress) && torrent.TorrentManager.Torrent.InfoHash.Equals(client.TorrentModel.TorrentManager.Torrent.InfoHash))
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if(isExist)
                    {
                        break;
                    }
                    dynamic ispAndCountry = Utils.Utils.IspAndCountryFromIp(uri.Host);
                    if(ispAndCountry.Country != "Germany")
                    {
                        break;
                    }
                    ClientModel newClient = new ClientModel { IpAddress = uri.Host, Port = uri.Port, TorrentModel = torrent, DateTime = DateTime.Now, ISP = ispAndCountry.Isp};
                    clientModels.Add(newClient);

                    ViewManager.Instance.ClientViewModels.Add(new ClientViewModel { No = ViewManager.Instance.ClientViewModels.Count, IpAddress = newClient.IpAddress, Port = newClient.Port, Client = peer.ClientApp.Client.ToString(), Title = torrent.Name, FileHash = torrent.TorrentManager.Torrent.InfoHash.ToString(), DateTime = newClient.DateTime });
                }
            }
        }

        private void WorkerTorrentsManage_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                workerTorrentsManage.ReportProgress(0);
                Thread.Sleep(1000);
            }
        }

        private BackgroundWorker workerTorrentsManage = new BackgroundWorker();

        string downloadsPath;
        string fastResumeFile;
        string dhtNodeFile;

        int port = 12345;

        EngineSettings engineSettings;
        ClientEngine engine;

        BEncodedDictionary fastResume;

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
            if(fastResume.ContainsKey(torrent.InfoHash.ToHex()))
            {
                manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.InfoHash.ToHex()]));
            }
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
            foreach (var torrent in Instance.torrentModels)
            {
                foreach (var peer in e.TorrentManager.GetPeers())
                {
                    var uri = peer.Uri;
                    var isExist = false;
                    foreach (var client in Instance.clientModels)
                    {
                        if (uri.Host.Equals(client.IpAddress) && torrent.TorrentManager.Torrent.InfoHash.Equals(client.TorrentModel.TorrentManager.Torrent.InfoHash))
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
                    if (ispAndCountry.Country != "Germany")
                    {
                        break;
                    }
                    ClientModel newClient = new ClientModel { IpAddress = uri.Host, Port = uri.Port, TorrentModel = torrent, DateTime = DateTime.Now, ISP = ispAndCountry.Isp };
                    Instance.clientModels.Add(newClient);

                    ViewManager.Instance.ClientViewModels.Add(new ClientViewModel { No = ViewManager.Instance.ClientViewModels.Count, IpAddress = newClient.IpAddress, Port = newClient.Port, Client = peer.ClientApp.Client.ToString(), Title = torrent.Name, FileHash = torrent.TorrentManager.Torrent.InfoHash.ToString(), DateTime = newClient.DateTime });
                }
            }
        }

        private static void shutdown()
        {
            BEncodedDictionary fastResume = new BEncodedDictionary();
            foreach(var model in Instance.torrentModels)
            {
                var manager = model.TorrentManager;
                manager.Stop();
                while(manager.State != TorrentState.Stopped)
                {
                    Debug.WriteLine("{0} is {1}", model.Name, manager.State);
                    Thread.Sleep(250);
                }
                fastResume.Add(manager.Torrent.InfoHash.ToHex(), manager.SaveFastResume().Encode());
            }
#if !DISABLE_DHT
            File.WriteAllBytes(Instance.dhtNodeFile, Instance.engine.DhtEngine.SaveNodes());
#endif
            File.WriteAllBytes(Instance.fastResumeFile, fastResume.Encode());
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
