using System;
using System.IO;

namespace API.DataManagement.Logger
{
    public class Logger
    {
        String fileName;
        readonly string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Main";
        readonly string endPath = "_" + Environment.UserName  + ".log";

        #region Constructors
        /// <summary>
        /// Base Constructor,
        /// Will write logs in : Logs/Log.log
        /// </summary>
        public Logger()
        {
            fileName = path + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + endPath;
        }
        /// <summary>
        /// Will write data in specific file.
        /// Path: Logs/providedName.log
        /// </summary>
        /// <param name="file">Name of file you want date to be written to.</param>
        public Logger(string file)
        {
            fileName = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + file + endPath;
        }
        /// <summary>
        /// Will write data in specific file, in specific folder.
        /// Path: Logs/providedFolder/providedName.log
        /// IMPORTANT: You must create folder first in Logs/
        /// </summary>
        /// <param name="folder">Name of folder where you want to put file</param>
        /// <param name="file">Name of file you want date to be written to.</param>
        public Logger(string folder, string file)
        {
            fileName = fileName = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + folder + "\\"+  file + endPath;
        }
        #endregion

        public String FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                if (value.Length > 0) fileName = value;
            }
        }
        public void WriteLine(String message)
        {
            DateTime time = DateTime.Now;
            string format = "dd:MM:yyyy HH:mm:ss,fff";
            try
            {
                if (message.Length != 0)
                {
                    using StreamWriter sw = new StreamWriter(fileName, true);
                    sw.WriteLine(time.ToString(format) + " || " + message.ToString());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }
        public void Write(String message)
        {
            try
            {
                if (message.Length != 0)
                {
                    using StreamWriter sw = new StreamWriter(fileName, true);
                    sw.Write(message.ToString());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }
        public bool MaxLength()
        {
            double len = new FileInfo(fileName).Length;
            if ((len / 1024) >= 5000)
            {
                DateTime time = DateTime.Now;
                string format = "ddMMHHmmss";
                string fileNameNew = fileName.Substring(0,fileName.Length - 4) + "_" + time.ToString(format) + ".txt";
                File.Move(fileName, fileNameNew);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}