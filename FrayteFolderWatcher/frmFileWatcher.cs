
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Xml;
using Frayte.Services.Business;
using Frayte.Services;
using System.Configuration;
using Frayte.Services.Utility;

namespace FrayteFolderWatcher
{
    public partial class frmFileWatcher : Form
    {
        public ListBox listBox;
        public bool isApplicationWorking = true;


        string DevelopmentTeamEmail = System.Configuration.ConfigurationSettings.AppSettings["DevelopmentMail"].ToString();
        string OwnerMail = System.Configuration.ConfigurationSettings.AppSettings["OwnerMail"].ToString();
        DateTime lastdatetime = DateTime.Now;

        public frmFileWatcher()
        {
            InitializeComponent();
        }

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            // step: 1 Check that current folder have xml or not 
            string targetDirectory = System.Configuration.ConfigurationSettings.AppSettings["XmlFileFolderPath"];
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            if (fileEntries.Count() > 0)
            {
                foreach (string fileName in fileEntries)
                {
                    ProcessXml(fileName);
                }
            }
            button2_Click(sender, e);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            // Create a new FileSystemWatcher object.
            FileSystemWatcher fsWatcher = new FileSystemWatcher();

            //fsWatcher.Path = "D:\\App";
            fsWatcher.Path = System.Configuration.ConfigurationSettings.AppSettings["XmlFileFolderPath"];

            rtbmsg.Text = "XML File Watcher Application is looking for CargoWise XML File at : " + fsWatcher.Path;

            // Set Filter.
            fsWatcher.Filter = "*.*";
            // Monitor files and subdirectories.
            fsWatcher.IncludeSubdirectories = true;
            // Monitor all changes specified in the NotifyFilters.
            fsWatcher.NotifyFilter = NotifyFilters.Attributes |
                                     NotifyFilters.CreationTime |
                                     NotifyFilters.DirectoryName |
                                     NotifyFilters.FileName |
                                     NotifyFilters.LastAccess |
                                     NotifyFilters.LastWrite |
                                     NotifyFilters.Security |
                                     NotifyFilters.Size;

            fsWatcher.EnableRaisingEvents = true;
            // Raise Event handlers.
            //fsWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fsWatcher.Created += new FileSystemEventHandler(OnCreated);
            //fsWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            //fsWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            //fsWatcher.Error += new ErrorEventHandler(OnError);
        }

        // FileSystemWatcher – OnCreated Event Handler
        public void OnCreated(object sender, FileSystemEventArgs e)
        {
            bool closed = false;
            bool status = true;
            while (status)
            {
                closed = IsFileClosed(e.FullPath);
                if (closed)
                {
                    status = false;
                }
            }

            if (closed)
            {
                if (Path.GetExtension(e.FullPath) == ".xml")
                {
                    bool result = ProcessXml(e.FullPath);
                    if (!result)
                    {
                        this.Close();
                    }

                }
            }
            //if (Path.GetExtension(e.FullPath) == ".xml")
            //{
            //    bool result = ProcessXml(e.FullPath);
            //    if (!result)
            //    {
            //        this.Close();
            //    }

            //}
        }

        public static bool IsFileClosed(string filename)
        {
            try
            {
                using (var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
        private bool ProcessXml(string xmlPath)
        {
            try
            {
                if (Path.GetExtension(xmlPath) == ".xml")
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(xmlPath);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables["DataSource"];
                        DataTable dtCustom = ds.Tables["CustomizedField"];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string CargoWiseShipmentid = dt.Rows[0]["Key"].ToString();
                            int FrayteShipmentId = 0;

                            // Insert CargoWiseSo into Shipment table

                            foreach (DataRow row in dtCustom.Rows)
                            {
                                string cfKey = CommonConversion.ConvertToString(row, "Key");
                                if (cfKey == "FRAYTE SHIPMENT ID")
                                {
                                    FrayteShipmentId = CommonConversion.ConvertToInt(row, "Value");
                                    break;
                                }
                            }
                            string CargoWiseSo = new ShipmentRepository().GetCargoWiseSo(FrayteShipmentId);
                            if (String.IsNullOrEmpty(CargoWiseSo))
                            {
                                new ShipmentRepository().UpdateCargoWiseShipmentId(CargoWiseShipmentid, FrayteShipmentId);
                            }

                            // fullpath  to move file

                            string Movefolderpath = System.Configuration.ConfigurationSettings.AppSettings["MoveXmlFileFolderPath"] + "\\" + CargoWiseShipmentid + ".xml";

                            // Move file from Specific folder to Archive folder

                            RenameFile(xmlPath, Movefolderpath);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //Step 1: Shoot Mail
                #region send mail to development team and owner

                string body = "<!DOCTYPE html><html><head><title></title></head><body style=margin: 0; padding: 0; background: #eaf0f5; border: 1px solid #ddd; font-family: calibri, sans-serif;><header><div style=height: 70px; background: #fff !important; box-shadow: 0px 1px 4px -2px; padding: 0 30px; text-align: left;><img src=cid:FrayteLogo style=width: 150px; padding: 20px 0px;/>";
                body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><p>Dear Sir</p><p>CargoWise XML watcher application stop at :</p>" + xmlPath + " and have below error";
                body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><h3>Error Message</h3><p>" + ex.Message + "</p>";
                body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><h3>Error Stack Trace</h3><p>" + ex.StackTrace + "</p>";
                body += "<br /></div></div><footer><div style=height: 50px; border: 1px solid #ddd; background: #dce5ec !important; font-size: 12px; text-align: left; padding: 25px 30px;><p>FRAYTE LOGISTICS LTD. All rights reserved.</p></div></footer></body></html>";

                FrayteEmail.SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "CargoWise XML watcher application Stop Working", body, "", "");
                #endregion
                return false;
            }
        }
        // FileSystemWatcher – OnChanged Event Handler
        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Add event details in listbox.
            this.Invoke((MethodInvoker)delegate { listBox.Items.Add(String.Format("Path : {0}   || Action : {1}", e.FullPath, e.ChangeType)); });
        }
        // FileSystemWatcher – OnRenamed Event Handler
        public void OnRenamed(object sender, RenamedEventArgs e)
        {
            // Add event details in listbox.
            this.Invoke((MethodInvoker)delegate { listBox.Items.Add(String.Format("Path : {0}   || Action : {1} to {2}", e.FullPath, e.ChangeType, e.Name)); });
        }
        // FileSystemWatcher – OnDeleted Event Handler
        public void OnDeleted(object sender, FileSystemEventArgs e)
        {
            // Add event details in listbox.
            this.Invoke((MethodInvoker)delegate { listBox.Items.Add(String.Format("Path : {0}  || Action : {1}", e.FullPath, e.ChangeType)); });
        }
        // FileSystemWatcher – OnError Event Handler
        public void OnError(object sender, ErrorEventArgs e)
        {
            // Add event details in listbox.
            this.Invoke((MethodInvoker)delegate { listBox.Items.Add(String.Format("Error : {0}", e.GetException().Message)); });
        }

        #endregion

        #region Methods
        public void RenameFile(String oldFilename, String newFilename)
        {
            FileInfo file = new FileInfo(oldFilename);

            if (file.Exists)
            {
                if (oldFilename != newFilename)
                {
                    if (System.IO.File.Exists(newFilename))
                    {
                        System.IO.File.Delete(newFilename);
                    }
                    File.Move(oldFilename, newFilename);
                }
            }
        }

        #endregion

    }
}
