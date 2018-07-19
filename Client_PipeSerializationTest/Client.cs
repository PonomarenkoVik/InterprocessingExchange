using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Server_PipeSerializationTest;

namespace Client_PipeSerializationTest
{
    class Client : IDisposable
    {
        public Client(string pipeName)
        {
            _pipeName = pipeName;

        }


        public void Start()
        {
            bool use = true;
            for (int i = 0; i < 20; i++)
            {
                
                Table table = new Table(5);
                foreach (Fork fork in table)
                {
                    fork.IsUsing = use;
                    if (use)
                    {
                        use = false;
                    }
                    else
                    {
                        use = true;
                    }
                }
                
                
                IFormatter form = new BinaryFormatter();
                bool connected = false;
                do
                {
                    
                    try
                    {
                        using (_serverStream = new NamedPipeClientStream(_pipeName))
                        {
                            _serverStream.Connect();
                            form.Serialize(_serverStream, table);
                            _serverStream.Flush();                        
                        }
                        foreach (Fork fork in table)
                        {
                            Console.WriteLine("Server {0}", fork.IsUsing); 
                        }                   
                        
                        connected = true;
                    }
                    catch (Exception e)
                    {
                    }
                } while (!connected);
                
               
            }                  
        }

       
        public void Dispose()
        {
            _serverStream.Dispose();
        }

        private NamedPipeClientStream _serverStream;
        private string _pipeName;
    }
}
