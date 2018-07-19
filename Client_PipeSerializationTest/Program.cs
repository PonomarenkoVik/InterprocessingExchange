using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client_PipeSerializationTest
{
    class Program
    {
        static void Main(string[] args)
        {

            using (Client client= new Client("Server1"))
            {
                client.Start();
                Console.ReadKey();
            }
        }
    }
}
