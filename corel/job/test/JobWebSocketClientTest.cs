using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace corel
{
    public class JobWebSocketClientTest : JobBase
    {
        int Port = 8181;
        readonly string URL;

        // The response from the remote device.
        private static String response = String.Empty;
        readonly Socket client;

        public JobWebSocketClientTest(IJobContext jobContext) : base(jobContext, JOB_TYPE.WEB_SOCKET_CLIENT)
        {
            this.URL = "ws://0.0.0.0:" + this.Port.ToString();

            // Create a TCP/IP socket.
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        static byte[] cache = new byte[StateObject.BufferSize];
        void startClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com". 
                IPAddress ipAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, this.Port);

                // Connect to the remote endpoint.
                var asyncResult = client.BeginConnect(remoteEP, null, client);
                // Poll state complete
                while (!asyncResult.IsCompleted)
                    Thread.Sleep(1000);

                // Retrieve the socket from the state object.
                //Socket client = (Socket)asyncResult.AsyncState;
                // Complete the connection.
                client.EndConnect(asyncResult);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());


                // Convert the string data to byte data using ASCII encoding.
                //byte[] byteData = Encoding.ASCII.GetBytes(data);
                
                //////////////////////////////////////////////////
                /// SEND PRIVATE_KEY REQUEST CONNECT

                // Send HS
                var handshake = string.Format("GET / HTTP/1.1\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Version: 13\r\nSec-WebSocket-Key: p2z/MFplfpRzjsVywqRQTg==\r\nHost: {0}\r\nOrigin: http://{0}/\r\n\r\n", "127.0.0.1:8181");
                var byteData = Encoding.UTF8.GetBytes(handshake);

                // Begin sending the data to the remote device.
                var asyncRsSend = client.BeginSend(byteData, 0, byteData.Length, 0, null, client);
                // Poll state complete
                while (!asyncRsSend.IsCompleted)
                    Thread.Sleep(1000);

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(asyncRsSend);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                
                //////////////////////////////////////////////////
                /// RECEIVER PUBLIC_KEY CONNECT AND ALWAYS WAITING RECEIVING DATA FROM SERVER
                
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.
                var asyncRsRead = client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, null, state);
                // Poll state complete
                while (!asyncRsRead.IsCompleted)
                    Thread.Sleep(1000);

                // Read data from the remote device.
                int bytesRead = client.EndReceive(asyncRsRead);

                while (bytesRead > 0)
                {
                    if (!UnsafeCompare(cache, state.buffer))
                    {
                        Array.Copy(state.buffer, cache, state.buffer.Length);

                        // There might be more data, so store the data received so far.
                        string s = Encoding.ASCII.GetString(cache, 0, bytesRead);
                        Console.WriteLine("=> Receive: " + s + "\n");
                        //HTTP/1.1 101 Switching Protocols
                        //Upgrade: websocket
                        //Connection: Upgrade
                        //Sec-WebSocket-Accept: NXrzFctS9UiabL6AvN1eb7X5cn8=

                        //receiveDone.Set();

                        // Get the rest of the data.
                        //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        Thread.Sleep(100);
                        //Console.WriteLine("=> waiting ... ");
                        //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }

        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
            // Tracer.WriteLine("J{0} executes on thread {1}: INIT ...");
            //Process.Start(String.Format("http://{0}/", server.EndPoint));

            Tracer.WriteLine(String.Format("CLIENT: http://127.0.0.1:{0}/", Port));

            startClient();

        }

        public override void f_STOP()
        {
            this.client.Close();
            Tracer.WriteLine("J{0}_{1} {2} -> STOP", this.f_getId(), this.Type, this.GetType().Name);
        }

        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m) { }
        public override Message f_PROCESS_MESSAGE(Message m) { return m; }
    }


    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

}
