using System;
using System.IO;

namespace Practice
{
    public class LogTofile
    {
        private string m_LogShare = "C:\\Logs\\DailyBackup";
        private string m_LogFile = "Test.Log";
        private FileStream m_FileStream;
        private StreamWriter m_TextWriter;
        bool exists = System.IO.Directory.Exists(@"C:\\Logs\\DailyBackup");

        //properties needed for thread safe logging
        private static readonly object _syncObject = new object();
        static readonly TextWriter tw;

        static LogTofile()
        {
            tw = TextWriter.Synchronized(File.AppendText("C:\\Logs\\DailyBackup" + "\\DailyBackupLog.txt")); 
            //
            // TODO: Add constructor logic here
            //
        }
        //Call to this method for thread safe logging
        public void Write(string logMessage)
        {
            try
            {
                SafeLog(logMessage, tw);
            }
            catch (IOException e)
            {
                tw.Close();
            }
        }

        public void LogIt(string strMsg)
        {
            try
            {
                if (!exists)
                {
                    Directory.CreateDirectory(m_LogShare);
                }

                m_FileStream = new FileStream(m_LogShare + "\\" + m_LogFile, FileMode.Append);
                m_TextWriter = new StreamWriter(m_FileStream, System.Text.Encoding.ASCII);
                m_TextWriter.WriteLine(DateTime.Now + " " + strMsg);
                m_TextWriter.AutoFlush = true;
                m_FileStream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error appending to log file." + ex.Message);
            }
        }

        public void SafeLog(string logMessage, TextWriter w)
        {
            lock (_syncObject)
            {
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");

                // Update the file.
                w.Flush();
            }
        }
    }
}
