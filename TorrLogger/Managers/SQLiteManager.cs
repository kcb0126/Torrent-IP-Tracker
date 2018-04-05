using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.Managers
{
    class SQLiteManager
    {
        // SQLite configuration
        public const string dbfilename = "torrlogger.db";
        

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
            //conn = new SQLiteConnection(string.Format("Data Source={0};Version=3;Password={1}", ))
        }


        private string baseDir;
        private string dbPassword;

        private SQLiteConnection conn;

    }
}
