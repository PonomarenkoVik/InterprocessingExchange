using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhilosopherClasses.MainClasses
{
    public class Writer : IDisposable
    {
        public Writer(string logFileName)
        {
            _stream = new FileStream(logFileName, FileMode.Append);            
            _writer = new StreamWriter(_stream);           
        }

        public void SaveLog(string str)
        {            
            _writer.Write(str);        
        }


        public void Dispose()
        {
            _writer.Flush();
            _stream.Flush();            
            _writer.Dispose();
            _stream.Dispose();
        }

        private readonly FileStream _stream;
        private readonly StreamWriter _writer;
    }
}
