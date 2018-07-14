using Salar.Bois;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace corel
{
    public static class test_job
    {
        public static void f_rpc_Handle()
        {
            IJobContext jc = new JobMonitor();

            IJob server = new JobRpcServerTest(jc);
            IJobHandle handleServer = new JobHandle(server);

            Console.WriteLine("Enter to create client ... ");
            Console.ReadLine();

            IJob client = new JobRpcClientTest(jc);
            IJobHandle handleClient = new JobHandle(client);

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();

            handleClient.f_actionJob(JOB_HANDLE.REMOVE);
            handleServer.f_actionJob(JOB_HANDLE.REMOVE);
            jc.f_removeAll();

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void f_websocket_Handle()
        {
            IJobContext jc = new JobMonitor();

            IJob ws = new JobWebSocketServerTest(jc);
            IJobHandle handleWS = new JobHandle(ws);

            Console.WriteLine("Enter to create WS Client ... ");
            Console.ReadLine();

            IJob client = new JobWebSocketClientTest(jc);
            IJobHandle handleClient = new JobHandle(client);

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();

            handleClient.f_actionJob(JOB_HANDLE.REMOVE);
            handleWS.f_actionJob(JOB_HANDLE.REMOVE);
            jc.f_removeAll();

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }


        public static void f_JobTestRequestUrl()
        {
            IJobContext jc = new JobMonitor();

            jc.f_createNew(new JobTestRequestUrl(jc));
            //jc.f_createNew(new JobTestRequestUrl(jc));
            //jc.f_createNew(new JobTestRequestUrl(jc));
            //jc.f_createNew(new JobTestRequestUrl(jc));
            //jc.f_createNew(new JobTestRequestUrl(jc));

            /////////////////////////////////////////////////////

            Console.WriteLine("Enter to send many request to load balancer jobs on factory ...");
            Console.ReadLine();
            const int len = 1;
            Message[] ms = new Message[len];
            for (int i = 0; i < len; i++)
            {
                ms[i] = new Message() { Input = i };
                if (i == 0) ms[i].f_setTimeOutMillisecond(3000);
                if (i == 5) ms[i].f_setTimeOutMillisecond(7000);
                if (i == 8) ms[i].f_setTimeOutMillisecond(5000);
            }

            Func<IJobHandle, Guid, bool> responseCallbackDoneAll = (msgHandle, groupId) =>
             {
                 System.Tracer.WriteLine("TEST_JOB.RUN_TEST(): FINISH ....");
                 return false;
             };
            jc.f_sendRequestMessages(JOB_TYPE.REQUEST_URL, ms, responseCallbackDoneAll);

            /////////////////////////////////////////////////////


            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    Console.WriteLine("Enter to stop all...");
            //    Console.ReadLine();
            //    jobs.f_job_stopAll();
            //    Console.WriteLine("Enter to restart all...");
            //    Console.ReadLine();
            //    jobs.f_restartAllJob();
            //}

            /////////////////////////////////////////////////////

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();
            jc.f_removeAll();

            /////////////////////////////////////////////////////
            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void f_handle_HTTP_FILE()
        {
            IJobContext jc = new JobMonitor();
            IJob jo = new JobFileHttp(jc);
            IJobHandle handle = new JobHandle(jo);

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();
            handle.f_actionJob(JOB_HANDLE.REMOVE);

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void f_handle_Test()
        {
            IJobContext jc = new JobMonitor();
            IJob jo = new JobTest(jc);
            IJobHandle handle = new JobHandle(jo);

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();
            handle.f_actionJob(JOB_HANDLE.REMOVE);

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void f_jobTest_Factory()
        {
            IJobContext jc = new JobMonitor();

            jc.f_createNew(new JobTest(jc));
            //jc.f_createNew(new JobTest(jc));
            //jc.f_createNew(new JobTest(jc));
            //jc.f_createNew(new JobTest(jc));
            //jc.f_createNew(new JobTest(jc));

            /////////////////////////////////////////////////////

            //Console.WriteLine("Enter to send many request to load balancer jobs on factory ...");
            //Console.ReadLine();
            //const int len = 9;
            //Message[] ms = new Message[len];
            //for (int i = 0; i < len; i++)
            //{
            //    ms[i] = new Message() { Input = i };
            //    if (i == 0) ms[i].f_setTimeOut(30000);
            //    if (i == 5) ms[i].f_setTimeOut(7000);
            //    if (i == 8) ms[i].f_setTimeOut(5000);
            //}

            //Func<IJobHandle, Guid, bool> responseCallbackDoneAll = (msgHandle, groupId) =>
            // {
            //     System.Tracer.WriteLine("TEST_JOB.RUN_TEST(): FINISH ....");
            //     return false;
            // };
            //jc.f_sendRequestMessages(JOB_TYPE.NONE, ms, responseCallbackDoneAll);

            /////////////////////////////////////////////////////


            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    Console.WriteLine("Enter to stop all...");
            //    Console.ReadLine();
            //    jobs.f_job_stopAll();
            //    Console.WriteLine("Enter to restart all...");
            //    Console.ReadLine();
            //    jobs.f_restartAllJob();
            //}

            /////////////////////////////////////////////////////

            Console.WriteLine("Enter to stop all JOB...");
            Console.ReadLine();
            jc.f_removeAll();

            /////////////////////////////////////////////////////
            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void f_bookmark_import()
        {
            //string s = File.ReadAllText("bookmarks.html");
            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(s);

            //List<oLink> list = new List<oLink>();
            //string tag = string.Empty;
            //var nodes = doc.DocumentNode.QuerySelectorAll("h3");
            //foreach (var node in nodes)
            //{
            //    if (node.ParentNode.QuerySelectorAll("h3").Count == 1)
            //    {
            //        tag = node.ParentNode.QuerySelector("h3").InnerText;
            //        var a = node.ParentNode.QuerySelectorAll("a").Select(x => new oLink() { Tags = tag, Title = x.InnerText, Link = x.getAttribute("href") }).ToArray();
            //        list.AddRange(a);
            //    }
            //}

            //var remains = doc.DocumentNode.QuerySelectorAll("a").Select(x => new oLink() { Tags = tag, Title = x.InnerText, Link = x.getAttribute("href") }).ToArray();
            //list.AddRange(remains);
            //list = list.GroupBy(x => x.Link).Select(x => x.First()).ToList();

            //oLinkTag[] tags = list.GroupBy(x => x.Tags).Select(x => new oLinkTag() { Count = x.Count(), Tag = x.Key }).ToArray();

            //using (var file = new FileStream("lin.dat", FileMode.OpenOrCreate))
            //{
            //    new BoisSerializer().Serialize<oLink[]>(list.ToArray(), file);
            //    file.Close();
            //}

        }

        public static void f_MediaMP3Stream_Demo()
        {
            //Application.EnableVisualStyles();
            //Application.Run(new fMediaMP3Stream_Demo());
        }

        public static void f_jobWebClient()
        {
            //var jobs = new JobStore();

            //jobs.f_job_addNew(new JobWebClient(jobs));
            //jobs.f_job_addNew(new JobWebClient(jobs));
            //jobs.f_job_addNew(new JobWebClient(jobs));
            //jobs.f_job_addNew(new JobWebClient(jobs));
            //jobs.f_job_addNew(new JobWebClient(jobs));

            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    Console.WriteLine("Input URL: ");
            //    string url = Console.ReadLine();
            //    jobs.f_url_AddRange(new string[] { "https://dictionary.cambridge.org/grammar/british-grammar/" });

            //    //Console.WriteLine("Enter to stop all...");
            //    //Console.ReadLine();
            //    //jobs.f_stopAll(); 
            //    //Console.WriteLine("Enter to restart all...");
            //    //Console.ReadLine();
            //    //jobs.f_restartAllJob(); 
            //}
        }

        public static void f_jobSpeechEN()
        {
            //var jobs = new JobStore();

            //jobs.f_job_addNew(new JobSpeechEN(jobs)); 

            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    //Console.WriteLine("Enter to stop all...");
            //    //Console.ReadLine();
            //    //jobs.f_stopAll();
            //    //Console.WriteLine("Enter to restart all...");
            //    //Console.ReadLine();
            //    //jobs.f_restartAllJob();
            //    Console.Write("Enter to speech: ");
            //    string input = Console.ReadLine();
            //    //jobs.f_job_sendMessage(id, input);

            //}
        }

        public static void f_JobGooTranslate()
        {
            //var jobs = new JobStore();

            //jobs.f_job_addNew(new JobGooTranslate(jobs)); 

            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    //Console.WriteLine("Enter to stop all...");
            //    //Console.ReadLine();
            //    //jobs.f_stopAll();
            //    //Console.WriteLine("Enter to restart all...");
            //    //Console.ReadLine();
            //    //jobs.f_restartAllJob();
            //    Console.Write("Enter to translate: ");
            //    string input = Console.ReadLine();
            //    //jobs.f_job_sendMessage(id, input);

            //}
        }

        public static void f_JobWord()
        {
            //var jobs = new JobStore();

            //jobs.f_job_addNew(new JobWord(jobs)); 

            //jobs.OnStopAll += (se, ev) => {
            //    Tracer.WriteLine(">>>>> STOP ALL JOBS: DONE ...");
            //};

            //while (true)
            //{
            //    //Console.WriteLine("Enter to stop all...");
            //    //Console.ReadLine();
            //    //jobs.f_stopAll();
            //    //Console.WriteLine("Enter to restart all...");
            //    //Console.ReadLine();
            //    //jobs.f_restartAllJob();
            //    Console.Write("Enter word to dictionary: ");
            //    string input = Console.ReadLine();
            //    //jobs.f_job_sendMessage(id, input);

            //}
        }


    }
}
