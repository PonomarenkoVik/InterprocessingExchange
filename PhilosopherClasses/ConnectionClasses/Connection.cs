using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using PhilosopherClasses.Enums;
using PhilosopherClasses.MainClasses;

namespace PhilosopherClasses.ConnectionClasses
{
    public class Connection : IForkExchange
    {
        public Connection(string pipeName)
        {
            _pipeName = pipeName;
            //_outPipeName = outPipeName;      
        }


        public Table Get()
        {
            Table table;
            using (var serverStream = new NamedPipeServerStream(_pipeName))
            {
                serverStream.WaitForConnection();
                IFormatter f = new BinaryFormatter();
                table = (Table)f.Deserialize(serverStream);
                serverStream.Disconnect();
            }
            return table;
        }

        public void Send(Table table)
        {
            IFormatter form = new BinaryFormatter();
            bool connected = false;
            do
            {
                try
                {
                    using (var clientStream = new NamedPipeClientStream(_pipeName))
                    {
                        clientStream.Connect();
                        form.Serialize(clientStream, table);
                        clientStream.Flush();
                    }
                    connected = true;
                }
                catch (Exception e)
                {
                }
            } while (!connected);
        }

        public void Dispose()
        {
            
        }      
        private readonly string _pipeName;
        private readonly string _outPipeName;    
        public IEnumerable<object> _ { get; set; }
    }
}
