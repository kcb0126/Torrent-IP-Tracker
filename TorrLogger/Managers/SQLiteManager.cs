using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrLogger.ViewModels;

namespace TorrLogger.Managers
{
    class SQLiteManager
    {
        // SQLite configuration
        public const string dbfilename = "torrlogger.db";

        private const string sql_table_history = @"CREATE TABLE IF NOT EXISTS `tb_history` (`id` INTEGER, `ipaddress` TEXT, `port` INTEGER, `client` TEXT, `date` TEXT, `time` TEXT, `enddate` TEXT, `endtime` TEXT, `title` TEXT, `filehash` TEXT, `country` TEXT, `isp` TEXT, PRIMARY KEY (`id`))";

        private const string sql_insert_history = @"INSERT INTO `tb_history` (`ipaddress`, `port`, `client`, `date`, `time`, `enddate`, `endtime`, `title`, `filehash`, `country`, `isp`) VALUES ('{0}', {1:D}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')";
        private const string sql_getall_history = @"SELECT * FROM `tb_history`";
        private const string sql_clear_history = @"DELETE FROM `tb_history`";

        private const string sql_test_connection = @"PRAGMA database_list";

        public static SQLiteManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SQLiteManager();
                }
                return _instance;
            }
        }
        private static SQLiteManager _instance = null;

        public SQLiteManager(string password = "", string baseDir = "")
        {
            // current directory is represented as "."
            if(baseDir.Equals("")) { baseDir = "."; }

            // check directory validation
            if (!Directory.Exists(baseDir)) { baseDir = "."; }

            // add slash('\') at the end of path
            baseDir = baseDir.TrimEnd(Path.DirectorySeparatorChar);
            baseDir = baseDir + Path.DirectorySeparatorChar;

            // store information for db
            this.baseDir = baseDir;
            this.dbPassword = password;

            // open or create(if not exist) db file using above information
            conn = new SQLiteConnection(string.Format("Data Source={0};Version=3;Password={1}", DBFile, password));
            conn.Open();

            // checking sqlite database...
            if(RunQuery(sql_test_connection))
            {
                // check necessary tables and create if not exists
                InitializeTables();
            }
        }

        public bool InsertHistory(ClientViewModel model)
        {
            string sql = string.Format(sql_insert_history
                , model.IpAddress
                , model.Port
                , model.Client
                , model.Date
                , model.Time
                , model.EndDate
                , model.EndTime
                , model.Title
                , model.FileHash
                , model.Country
                , model.ISP);
            return RunQuery(sql);
        }

        public ObservableCollection<ClientViewModel> GetAllHistory()
        {
            string sql = sql_getall_history;
            ObservableCollection<ClientViewModel> history = new ObservableCollection<ClientViewModel>();
            RunQueryWithResult(sql, (rdr) =>
            {
                string ipaddress = (string)rdr["ipaddress"];
                int port = (int)(long)rdr["port"];
                string client = (string)rdr["client"];
                string date = (string)rdr["date"];
                string time = (string)rdr["time"];
                string enddate = (string)rdr["enddate"];
                string endtime = (string)rdr["endtime"];
                string title = (string)rdr["title"];
                string filehash = (string)rdr["filehash"];
                string country = (string)rdr["country"];
                string isp = (string)rdr["isp"];
                history.Add(new ClientViewModel {No = history.Count + 1, IpAddress = ipaddress, Port = port, Client = client, Date = date, Time = time, EndDate = enddate, EndTime = endtime, Title = title, FileHash = filehash, Country = country, ISP = isp });
                return 0;
            });
            return history;
        }

        public bool ClearHistory()
        {
            string sql = sql_clear_history;
            return RunQuery(sql);
        }

        private bool RunQuery(string sql)
        {
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            try
            {
                int result = comm.ExecuteNonQuery();
                Debug.WriteLine(sql);
                Debug.WriteLine("{0:D} rows affected.", result);
            }
            catch(SQLiteException)
            {
                return false;
            }
            return true;
        }

        private bool RunQueries(params string[] sql_list)
        {
            string sql = "";
            for(int i = 0; i < sql_list.Length; i++)
            {
                sql = sql + sql_list[i] + ";\n";
            }
            return RunQuery(sql);
        }

        private SQLiteDataReader RunQueryWithResult(string sql)
        {
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            SQLiteDataReader rdr = null;
            try
            {
                rdr = comm.ExecuteReader();
                Debug.WriteLine(sql);
            }
            catch
            {

            }
            return rdr;
        }

        private void RunQueryWithResult(string sql, Func<SQLiteDataReader, int> fetchRow)
        {
            SQLiteDataReader rdr = RunQueryWithResult(sql);
            if(rdr != null)
            {
                while(rdr.Read())
                {
                    fetchRow(rdr);
                }
            }
        }

        private void InitializeTables()
        {
            RunQueries(sql_table_history);
        }

        private string baseDir;
        private string dbPassword;

        private string DBFile
        {
            get
            {
                return baseDir + dbfilename;
            }
        }

        private SQLiteConnection conn;

    }
}
