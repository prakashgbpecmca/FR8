using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrayteFileSystemWatcher
{
    public class FrayteSystemwatcher
    {

        SqlConnection con;
        SqlCommand cmd;

        public void watch()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @"D:\\App";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = ".txt";
            watcher.Changed += new FileSystemEventHandler(OnCreated);
            watcher.EnableRaisingEvents = true;
        }

        //public void Watcher()
        //{
        //    FileSystemWatcher watcher = new FileSystemWatcher("D:\\App\\", ".txt");

        //    //watcher.NotifyFilter = NotifyFilters.LastAccess
        //    //            | NotifyFilters.LastWrite
        //    //            | NotifyFilters.FileName
        //    //            | NotifyFilters.DirectoryName;
        //    // Add event handlers.

        //    //watcher.Changed += new FileSystemEventHandler(OnChanged);
        //    watcher.Created += new FileSystemEventHandler(OnCreated);
        //    //watcher.Deleted += new FileSystemEventHandler(OnChanged);
        //    //watcher.Renamed += new RenamedEventHandler(OnRenamed);

        //    // Begin watching.
        //    watcher.EnableRaisingEvents = true;
        //}

        public void OnCreated(object sender, FileSystemEventArgs e)
        {
            string des = "a file " + e.Name + " is created in " + e.FullPath + " at " + DateTime.Now.ToString();

            con = new SqlConnection("Data Source=ADMIN-PC\\SQLEXPRESS2012;Initial Catalog=Frayte;User Id=sa;Pwd=1234;");

            cmd = new SqlCommand("Insert into Watchmen values('" + des + "')",con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
