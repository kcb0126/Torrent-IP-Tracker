using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
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

        private const string sql_table_torrent = @"CREATE TABLE IF NOT EXISTS `tb_torrent` (`id` INTEGER, `name` TEXT, `filename` TEXT, PRIMARY KEY (`id`))";

        private const string sql_insert_torrent = @"INSERT INTO `tb_torrent` (`name`, `filename`) VALUES('{0}', '{1}')";
        private const string sql_delete_torrent = @"DELETE FROM `tb_torrent` WHERE id = {0}";
        private const string sql_getall_torrent = @"SELECT * FROM `tb_torrent` WHERE 1";
        private const string sql_getcount_torrent = @"SELECT COUNT(*) as `count` FROM `tb_torrent` WHERE 1";

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

        private void InitializeTables()
        {
            RunQueries(sql_table_torrent);
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
