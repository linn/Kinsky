using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Linn
{
    public class UserLogFile : IUserLogListener, IDisposable
    {
        private TextWriter iTextWriter;
        private const string kFileName = "UserLog.txt";

        public UserLogFile(string basePath) 
        {
            string fileName = Path.Combine(basePath, kFileName);
            iTextWriter = new StreamWriter(fileName);
        }

        public void Write(string aMessage)
        {
            if (iTextWriter != null)
            {
                iTextWriter.Write(aMessage);
                iTextWriter.Flush();
            }
        }

        public void WriteLine(string aMessage)
        {
            if (iTextWriter != null)
            {
                iTextWriter.WriteLine(aMessage);
                iTextWriter.Flush();
            }
        }

        public void Dispose()
        {
            if (iTextWriter != null)
            {
                iTextWriter.Close();
            }
        }
    }
}
