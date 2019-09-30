using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Ghostscript.NET.Processor;
using System.IO;
using System.Diagnostics;
using Ghostscript.NET.Processor;
using Frayte.Services.Business;
using Frayte.Services.Utility;

namespace DownloadLabels
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            var SessionIds = new DirectBookingUploadShipmentRepository().GetSessionIds();
            foreach (var res in SessionIds)
            {
                var result = new DirectBookingUploadShipmentRepository().getLabels(res);
                if (result.Count > 0)
                {
                    p.Start(result);
                    new DirectBookingUploadShipmentRepository().SaveIsSessionPrintFalse(res);
                }
            }
        }




        public Boolean PrintPDFs(string str)
        {
            //foreach (var file in str)
            //{
            try
            {

                //Process proc = new Process();
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //proc.StartInfo.Verb = "print";
                ////Define location of adobe reader/command line
                ////switches to launch adobe in "print" mode
                //proc.StartInfo.FileName =
                //  @"C:\Program Files (x86)\Adobe\Reader 8.0\Reader\AcroRd32.exe";
                ////proc.StartInfo.Arguments = String.Format(@"/p /h {0}", pdfFileName);
                //string flagNoSplashScreen = "/s /o";
                //string flagOpenMinimized = "/h";
                //var flagPrintFileToPrinter = string.Format("/t \"{0}\" \"{1}\"", str, "");
                //proc.StartInfo.Arguments = string.Format("{0} {1} {2}", flagNoSplashScreen, flagOpenMinimized, flagPrintFileToPrinter);
                //proc.StartInfo.UseShellExecute = false;
                ////proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.CreateNoWindow = false;
                //proc.Start();
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //proc.Refresh();
                //if (proc.HasExited == false)
                //{
                //    proc.WaitForExit(10000);
                //}

                //proc.EnableRaisingEvents = true;

                //proc.Close();
                //KillAdobe("AcroRd32");
                //return true;

                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                  @"C:\Program Files (x86)\Adobe\Reader 8.0\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = String.Format(@"/p /h {0}", str);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false)
                {
                    proc.WaitForExit(10000);
                }

                proc.EnableRaisingEvents = true;

                proc.Close();
                KillAdobe("AcroRd32");
                return true;
            }
            catch
            {
                return false;
            }
            //}
            //return true;
        }

        //For whatever reason, sometimes adobe likes to be a stage 5 clinger.
        //So here we kill it with fire.
        private bool KillAdobe(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses().Where(
                         clsProcess => clsProcess.ProcessName.StartsWith(name)))
            {
                clsProcess.Kill();
                return true;
            }
            return false;
        }
        public void Start(List<string> str)
        {
            // YOU NEED TO HAVE ADMINISTRATOR RIGHTS TO RUN THIS CODE
            foreach (var file in str)
            {
                string inputFile = file;
                try
                {
                    using (GhostscriptProcessor processor = new GhostscriptProcessor())
                    {
                        List<string> switches = new List<string>();
                        switches.Add("-empty");
                        switches.Add("-dPrinted");
                        switches.Add("-dBATCH");
                        switches.Add("-dNOPAUSE");
                        switches.Add("-dNOSAFER");
                        switches.Add("-dNumCopies=1");
                        switches.Add("-sDEVICE=mswinpr2");
                        switches.Add("-sOutputFile=%printer%" + AppSettings.PrinterName);
                        switches.Add("-f");
                        switches.Add(inputFile);
                        processor.StartProcessing(switches.ToArray(), null);
                    }
                }
                catch (Exception e)
                {

                }
            }
            
        }
    }
}
