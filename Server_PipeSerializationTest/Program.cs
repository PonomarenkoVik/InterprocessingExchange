using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_PipeSerializationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Server client = new Server("Server1"))
            {
                client.Start();
                Console.ReadKey();
            }
        }
    }
}
