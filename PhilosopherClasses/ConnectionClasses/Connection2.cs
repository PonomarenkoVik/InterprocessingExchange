using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using PhilosopherClasses.MainClasses;

namespace PhilosopherClasses.ConnectionClasses 
{
    public class Connection2 : IForkExchange
    {
        const int MmfMaxSize = 50000;  // allocated memory for this memory mapped file (bytes)
        const int MmfViewSize = 20000; // how many bytes of the allocated memory can this process access
        public Connection2(string memoryName)
        {
            // creates the memory mapped file which allows 'Reading' and 'Writing'  
            _mmf = MemoryMappedFile.CreateOrOpen(memoryName, MmfMaxSize, MemoryMappedFileAccess.ReadWrite);
            _mmvStream = _mmf.CreateViewStream(0, MmfViewSize);
           
        }
        public Table Get()
        {
            Table table = null;
            if (_mmvStream.CanRead)
            {
                var buffer = new byte[MmfViewSize];
                _mmvStream.Seek(0, SeekOrigin.Begin);
                _mmvStream.Read(buffer, 0, MmfViewSize);
                var formatter = new BinaryFormatter();
                table = (Table)formatter.Deserialize(new MemoryStream(buffer));               
            }
            Thread.Sleep(10);
            return table;
        }

        public void Send(Table table)
        {
            _mmvStream.Seek(0, SeekOrigin.Begin);
            var formatter = new BinaryFormatter();
            formatter.Serialize(_mmvStream, table);
            Thread.Sleep(10);
        }

        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewStream _mmvStream;
        public void Dispose()
        {
        //    _mmf.Dispose();
        //    _mmvStream.Dispose();
        }
    }
}
