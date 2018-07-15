using IpcChannel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IpcChannel
{
   public class _test_IpcChannel
    {
        const string KeyPath = @"CSharpTest.Net\IpcChannelTest";
        static readonly string _channel = new Guid("{1B617C4B-BF68-4B8C-AE2B-A77E6A3ECEC5}").ToString();
        static IIpcChannelRegistrar _registrar;

        public static void RUN()
        {
            _registrar = new IpcChannelRegistrar(Registry.CurrentUser, KeyPath);


            using (IpcEventChannel ch1 = new IpcEventChannel(_registrar, _channel))
            using (IpcEventChannel ch2 = new IpcEventChannel(_registrar, _channel))
            using (IpcEventChannel sender = new IpcEventChannel(_registrar, _channel))
            {
                ch1.StartListening("ch1");
                ch2.StartListening("ch2");
                sender.EnableAsyncSend();

                ////////////////////////////////////////////////////////////////////

                ch1["Message"].OnEvent += delegate (object o, IpcSignalEventArgs e)
                {
                    Console.WriteLine(string.Format("LISTENING_{0}: {1}", 1, String.Join(",", e.Arguments)));
                };
                ch2["Message"].OnEvent += delegate (object o, IpcSignalEventArgs e)
                {
                    Console.WriteLine(string.Format("LISTENING_{0}: {1}", 2, String.Join(",", e.Arguments)));
                };

                sender.ExecutionTimeout = 1000;

                Console.WriteLine("Enter to SEND broadcast to CHANNEL LISTENING ...");
                Console.ReadLine();

                sender.SendTo("CH1", "Message", "p1", "p2", "p3");
                sender.SendTo(new string[] { "CH2" }, "Message", "p1", "p2", "p3");
                sender.SendTo(1000, new string[] { "ch1", "ch2" }, "Message", "p1", "p2", "p3");

                ////////////////////////////////////////////////////////////////////

                //ch1.OnError += delegate (object o, ErrorEventArgs e)
                //{
                //    string error = e.GetException().Message;
                //    Console.WriteLine(string.Format("LISTENING_ERROR: {0}", error));
                //};

                //ch1["Test"].OnEvent += delegate (object o, IpcSignalEventArgs e)
                //{
                //    Console.WriteLine(string.Format("LISTENING_{0}: {1}", 1, String.Join(",", e.Arguments)));
                //};
                //ch2["Test"].OnEvent += delegate (object o, IpcSignalEventArgs e)
                //{
                //    Console.WriteLine(string.Format("LISTENING_{0}: {1}", 2, String.Join(",", e.Arguments)));
                //};

                //Console.WriteLine("Enter to SEND broadcast to CHANNEL LISTENING ...");
                //Console.ReadLine();

                //for (int i = 0; i < 1000; i++)
                //{
                //    //sender.SendTo(10000, i % 2 == 0 ? "ch1" : "ch2", "Test", "p1", "p2", "p3");
                //    sender.Broadcast(100, "Test", "1", "2", "3");
                //}

                ////////////////////////////////////////////////////////////////////

                sender.StopAsyncSending(true, -1);
                ch1.StopListening();
                ch2.StopListening();
            }

            /////////////////////////////////////////
            /// FREE RESOURCE

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();

            _registrar = null;
            Registry.CurrentUser.DeleteSubKeyTree(KeyPath);
        }
    }
}
