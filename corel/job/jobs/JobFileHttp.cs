using System;
using System.Threading;
using System.Linq;
using NHttp;
using System.IO;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace corel
{
    public class JobFileHttp : JobBase
    {
        int Port = 3456;
        readonly HttpServer server;
        readonly ConcurrentDictionary<string, string> fileData;

        public JobFileHttp(IJobContext jobContext) : base(jobContext, JOB_TYPE.FILE_HTTP_CACHE)
        {
            fileData = new ConcurrentDictionary<string, string>();
            server = new HttpServer();
            server.EndPoint.Port = this.Port;
            server.RequestReceived += f_server_onRequestReceived;
        }
        void f_server_onRequestReceived(object sender, HttpRequestEventArgs e)
        {
            if (e.Request.Url.LocalPath == "/favicon.ico") { e.Response.OutputStream.Close(); return; }

            //string file_name = e.Request.QueryString["file_name"];
            //if (!string.IsNullOrEmpty(file_name))
            //{
            //    using (var writer = new StreamWriter(e.Response.OutputStream))
            //    {
            //        writer.Write("Hello world!");
            //    }
            //}

            string data = @"{""key"":""Tiếng Việt""}";

            //data = JsonConvert.SerializeObject(this.JobContext.f_getAllJobs());

            Thread.Sleep(9000);

            e.Response.ContentType = "application/json; charset=utf-8";
            using (var writer = new StreamWriter(e.Response.OutputStream))
                writer.Write(data);
        }

        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
            // Tracer.WriteLine("J{0} executes on thread {1}: INIT ...");
            //Process.Start(String.Format("http://{0}/", server.EndPoint));
            server.Start();
            Port = server.EndPoint.Port;
            Tracer.WriteLine(String.Format("http://{0}/", server.EndPoint));
        }
        public override void f_STOP()
        {
            fileData.Clear();
            server.Stop();
            server.Dispose();
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
