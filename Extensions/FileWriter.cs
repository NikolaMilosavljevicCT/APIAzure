using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQLModifications.Logger
{
    public class FileWriter
    {
        public string Filepath { get; set; } //= AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".log";
        private Object locker = new Object();
        readonly string endPath = ".log";
        //readonly string endPath = "_" + Environment.UserName + ".log";

        #region Constructors
        /// <summary>
        /// Base Constructor,
        /// Will write logs in : Logs/Log.log
        /// </summary>
        public FileWriter()
        {
            Filepath = CreatePath();
        }
        /// <summary>
        /// Will write data in specific file.
        /// Path: Logs/providedName.log
        /// </summary>
        /// <param name="file">Name of file you want date to be written to.</param>
        public FileWriter(string file)
        {
            Filepath = CreatePath(file);
        }
        /// <summary>
        /// Will write data in specific file, in specific folder.
        /// Path: Logs/providedFolder/providedName.log
        /// IMPORTANT: You must create folder first in Logs/
        /// </summary>
        /// <param name="folder">Name of folder where you want to put file</param>
        /// <param name="file">Name of file you want date to be written to.</param>
        public FileWriter(string folder, string file)
        {
            Filepath = CreatePath(folder+"\\"+file);
        }
        #endregion

        public async Task WriteLine(string text)
        {
            
            //MaxLength(Filepath);
            int timeOut = 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (true)
            {
                try
                {
                    DateTime time = DateTime.Now;
                    string format = "dd:MM:yyyy HH:mm:ss,fff";
                    //Wait for resource to be free
                    lock (locker)
                    {
                        using (FileStream file = new FileStream(Filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            writer.WriteLine(time.ToString(format) + " || " + text.ToString());
                        }
                    }
                    break;
                }
                catch
                {
                    //File not available, conflict with other class instances or application
                }
                if (stopwatch.ElapsedMilliseconds > timeOut)
                {
                    //Give up.
                    break;
                }
                //Wait and Retry
                await Task.Delay(5);
            }
            stopwatch.Stop();
        }

        public bool MaxLength(string fileName)
        {
            double len = new FileInfo(fileName).Length;
            if ((len / 1024) >= 5000)
            {
                DateTime time = DateTime.Now;
                string format = "ddMMHHmmss";
                string fileNameNew = fileName.Substring(0, fileName.Length - 4) + "_" + time.ToString(format) + ".txt";
                File.Move(fileName, fileNameNew);
                return true;
            }
            else
            {
                return false;
            }
        }
        public string CreatePath(string text = "Main")
        {
            return Filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + text + "_" +DateTime.Now.Date.ToString("d", new CultureInfo("pt-BR")).Replace('/', '_') + endPath;
        }
    }
}
