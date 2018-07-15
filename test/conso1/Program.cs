using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace conso1
{
    class Program
    {
        static void Main(string[] args)
        {
            //IpcChannel._test_IpcChannel.RUN();
            //Rpc._test_Rpc.RUN();
            Google.ProtocolBuffers._test.RUN();

            /////////////////////////////////////////
            Console.WriteLine("Enter to exit...");
            Console.ReadLine(); 
        }
    }
}
