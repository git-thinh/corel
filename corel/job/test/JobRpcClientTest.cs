using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RPC;

namespace corel
{
    public class JobRpcClientTest : JobBase
    {
        // The client and server must agree on the interface id to use:
        readonly Guid iid = new Guid("{1B617C4B-BF68-4B8C-AE2B-A77E6A3ECEC5}");
        const string codeAPI = "RpcExampleClientServer";

        readonly RpcClientApi client;

        public JobRpcClientTest(IJobContext jobContext) : base(jobContext, JOB_TYPE.RPC_CLIENT)
        {
            client = new RpcClientApi(iid, RpcProtseq.ncalrpc, null, codeAPI);
            //client = new RpcClientApi(iid, RpcProtseq.ncacn_np, null, @"\pipe\" + codeAPI);
            //client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, null, @"18081");

            // Provide authentication information (not nessessary for LRPC)
            client.AuthenticateAs(RpcClientApi.Self);
        }

        void f_sendData()
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
                    Thread.Sleep(1000); 
                }
                else
                    Console.Error.WriteLine(rx);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        } 

        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
            f_sendData();
        }

        public override void f_STOP()
        {
            this.client.Dispose();
            Tracer.WriteLine("J{0}_{1} {2} -> STOP", this.f_getId(), this.Type, this.GetType().Name);
        }

        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m) { }
        public override Message f_PROCESS_MESSAGE(Message m) { return m; }
    }
     
}
