using IpcChannel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rpc
{
   public class _test_Rpc
    {
        // The client and server must agree on the interface id to use:
        static Guid iid = new Guid("{1B617C4B-BF68-4B8C-AE2B-A77E6A3ECEC5}");
        const string codeAPI = "RpcExampleClientServer";
        static RpcServerApi server;
        static RpcClientApi client;

        public static void RUN()
        {
            /////////////////////////////////////////
            /// SERVER

            // Create the server instance, adjust the defaults to your needs.
            server = new RpcServerApi(iid, 100, ushort.MaxValue, allowAnonTcp: false);
            
            try
            {
                //// Add an endpoint so the client can connect, this is local-host only:
                server.AddProtocol(RpcProtseq.ncalrpc, codeAPI, 100);
                //// If you want to use TCP/IP uncomment the following, make sure your client authenticates or allowAnonTcp is true
                server.AddProtocol(RpcProtseq.ncacn_np, @"\pipe\" + codeAPI, 25);
                ////// If you want to use TCP/IP uncomment the following, make sure your client authenticates or allowAnonTcp is true
                server.AddProtocol(RpcProtseq.ncacn_ip_tcp, @"18081", 25);

                // Add the types of authentication we will accept
                server.AddAuthentication(RpcAuthentication.RPC_C_AUTHN_GSS_NEGOTIATE);
                server.AddAuthentication(RpcAuthentication.RPC_C_AUTHN_WINNT);
                server.AddAuthentication(RpcAuthentication.RPC_C_AUTHN_NONE);

                // Subscribe the code to handle requests on this event:
                server.OnExecute += f_server_OnExecute;
                //_server.OnExecute += delegate (IRpcClientInfo client, byte[] bytes) { return new byte[0]; };

                // Start Listening 
                server.StartListening();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            /////////////////////////////////////////
            /// CLIENT
            
            client = new RpcClientApi(iid, RpcProtseq.ncalrpc, null, codeAPI);
            //client = new RpcClientApi(iid, RpcProtseq.ncacn_np, null, @"\pipe\" + codeAPI);
            //client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, null, @"18081");

            // Provide authentication information (not nessessary for LRPC)
            client.AuthenticateAs(RpcClientApi.Self);

            Console.WriteLine("Enter to send data to SERVER ... ");
            Console.ReadLine();

            f_client_sendData();

            /////////////////////////////////////////
            /// EXIT

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();

            client.Dispose();

            if (server != null)
            {
                server.StopListening();

                GC.Collect(0, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

        static void f_client_sendData()
        {
            client.Execute(new byte[1] { 0xEC });

            // Send the request and get a response
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    var response = client.Execute(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                    Console.WriteLine("Server response: {0}", Encoding.UTF8.GetString(response));
                }

                //client.Execute(new byte[0]);

                //byte[] bytes = new byte[1 * 1024 * 1024]; //1mb in/out
                //new Random().NextBytes(bytes);

                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();

                //for (int i = 0; i < 2; i++)
                //    client.Execute(bytes);

                //stopWatch.Stop();
                //TimeSpan ts = stopWatch.Elapsed;
                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                //Console.WriteLine(elapsedTime + " ncalrpc-large-timming");
            }
            catch (RpcException rx)
            {
                if (rx.RpcError == RpcError.RPC_S_SERVER_UNAVAILABLE || rx.RpcError == RpcError.RPC_S_SERVER_TOO_BUSY)
                {
                    //Use a wait handle if your on the same box...
                    Console.Error.WriteLine("Waiting for server...");
                    System.Threading.Thread.Sleep(1000);
                }
                else
                    Console.Error.WriteLine(rx);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        static byte[] f_server_OnExecute(IRpcClientInfo client, byte[] input)
        {
            //Impersonate the caller:
            using (client.Impersonate())
            {
                var reqBody = Encoding.UTF8.GetString(input);
                Console.WriteLine("Received '{0}' from {1}", reqBody, client.ClientUser.Name);

                return Encoding.UTF8.GetBytes(String.Format("Hello {0}, I received your message '{1}'.", client.ClientUser.Name, reqBody));
            }

            ////==============================================================
            //if (input.Length > 0)
            //{
            //    msgDataEncode code = (msgDataEncode)input[0];
            //    switch (code)
            //    {
            //        case msgDataEncode.ping:  // 0
            //            break;
            //        case msgDataEncode.update_node:  // 255
            //            break;
            //        case msgDataEncode.number_byte:  // 9
            //            Console.WriteLine(input.Length);
            //            break;
            //        case msgDataEncode.string_ascii:  // 1
            //            break;
            //        case msgDataEncode.string_utf8:  // 2
            //            break;
            //        case msgDataEncode.string_base64:  // 3
            //            break;
            //        case msgDataEncode.number_decimal:  // 4
            //            break;
            //        case msgDataEncode.number_long:  // 5
            //            break;
            //        case msgDataEncode.number_double:  // 6
            //            break;
            //        case msgDataEncode.number_int:  // 8
            //            break;
            //        default:
            //            #region
            //            //Impersonate the caller:
            //            using (client.Impersonate())
            //            {
            //                var reqBody = Encoding.UTF8.GetString(input);
            //                Console.WriteLine("Received '{0}' from {1}", reqBody, client.ClientUser.Name);

            //                return Encoding.UTF8.GetBytes(
            //                    String.Format(
            //                        "Hello {0}, I received your message '{1}'.",
            //                        client.ClientUser.Name,
            //                        reqBody
            //                        )
            //                    );
            //            }
            //            #endregion
            //    }//end switch
            //}
            //return new byte[0];
        }

    }
}
