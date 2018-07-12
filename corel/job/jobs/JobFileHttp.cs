using System;
using System.Threading;
using System.Linq;
using NHttp;
using System.IO;

namespace corel
{
    public class JobFileHttp : JobBase
    {
        readonly DictionaryThreadSafe<string, string> fileData;
        readonly QueueThreadSafe<Message> msg;

        public JobFileHttp(IJobAction jobAction) : base(JOB_TYPE.FILE_HTTP_CACHE, jobAction)
        {
            msg = new QueueThreadSafe<Message>();
            fileData = new DictionaryThreadSafe<string, string>();
            server = new HttpServer();
            server.RequestReceived += f_server_onRequestReceived;
        }

        private void f_server_onRequestReceived(object sender, HttpRequestEventArgs e)
        {
            string file_name = e.Request.QueryString["file_name"];
            if (!string.IsNullOrEmpty(file_name))
            {
                using (var writer = new StreamWriter(e.Response.OutputStream))
                {
                    writer.Write("Hello world!");
                }
            }
        }

        public override int f_getPort() { return Port; }
        public override bool f_checkKey(object key) { return false; }
        public override bool f_setData(string key, object data) { return false; }

        public override void f_receiveMessage(Message m)
        {
            msg.Enqueue(m);
        }

        int Port = 0;
        readonly HttpServer server;
        private void f_Init()
        {
            // Tracer.WriteLine("J{0} executes on thread {1}: INIT ...");
            //Process.Start(String.Format("http://{0}/", server.EndPoint));
            server.Start();
            Port = server.EndPoint.Port;
            Tracer.WriteLine(String.Format("http://{0}/", server.EndPoint));
        }

        private volatile bool _inited = false;
        public void f_stopJob()
        {
            fileData.Clear();
            server.Stop();
            server.Dispose();

            //jobInfo.f_stopJob();
        }

        private JobHandle jobInfo;
        public void f_runLoop(object state, bool timedOut)
        {
            if (!_inited)
            {
                jobInfo = (JobHandle)state;
                _inited = true;
                f_Init();
                return;
            }            
            if (!timedOut)
            {
                // Tracer.WriteLine("J{0} executes on thread {1}: SIGNAL -> STOP ...", Id, Thread.CurrentThread.GetHashCode().ToString());
                f_stopJob();
                return;
            }

            Tracer.WriteLine("J{0} executes on thread {1}:DO SOMETHING ...", this.f_getId(), Thread.CurrentThread.GetHashCode().ToString());
            // Do something ...

        }

    }
}
