using System;
using System.Threading;
using System.Linq;
using NHttp;
using System.IO;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Fleck;
using System.Collections.Generic;
using RPC;
using System.Text;

namespace corel
{
    public class JobRpcServerTest : JobBase
    {
        // The client and server must agree on the interface id to use:
        readonly Guid iid = new Guid("{1B617C4B-BF68-4B8C-AE2B-A77E6A3ECEC5}");
        const string codeAPI = "RpcExampleClientServer";
        readonly RpcServerApi server;


        public JobRpcServerTest(IJobContext jobContext) : base(jobContext, JOB_TYPE.RPC_SERVER)
        {
            // Create the server instance, adjust the defaults to your needs.
            this.server = new RpcServerApi(iid, 100, ushort.MaxValue, allowAnonTcp: false);
        }
         
        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
            
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
        }

        byte[] f_server_OnExecute(IRpcClientInfo client, byte[] input)
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

        public override void f_STOP()
        {
            if (server != null)
            {
                server.StopListening();

                GC.Collect(0, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }

            Tracer.WriteLine("J{0}_{1} {2} -> STOP", this.f_getId(), this.Type, this.GetType().Name);
        }
        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m)
        {
        }
        public override Message f_PROCESS_MESSAGE(Message m)
        {
            return m;
        }
    }
}
