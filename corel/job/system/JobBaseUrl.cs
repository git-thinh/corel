using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Web;

namespace corel
{
    public class JobBaseUrl : IJob
    {
        volatile int Id = 0;
        volatile byte Status = 0; /* 0: none */

        public JOB_STATE State
        {
            get
            {
                switch (this.Status)
                {
                    case 1: return JOB_STATE.INIT;
                    case 2: return JOB_STATE.RUNNING;
                    case 3: return JOB_STATE.PROCESSING;
                    case 4: return JOB_STATE.STOPED;
                }
                return JOB_STATE.NONE;
            }
        }
        public IJobContext JobContext { get; }
        public IJobHandle Handle { get; private set; }
        public JOB_TYPE Type { get; }

        public int f_getId() { return Id; }

        public JobBaseUrl(IJobContext jobContext, JOB_TYPE type)
        {
            this.JobContext = jobContext;
            this.Id = jobContext.f_getTotalJob() + 1;
            this.Type = type;
            this.Status = 1; /* 1: init */
        }

        public void f_receiveMessage(Message m)
        {
        }
        public void f_receiveMessages(Message[] ms)
        {
        }
        public void f_stop()
        {
            this.Status = 4; /* 4: stop */
            System.Tracer.WriteLine("J{0}_{1} JobBaseUrl -> STOP", this.Id, this.Type);
            this.f_STOP();
            this.Handle.f_actionJobCallback();
        }

        public virtual void f_STOP() { }
        public virtual void f_INIT() { }
        public virtual void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m) { }
        public virtual Message f_PROCESS_MESSAGE(Message m) { return m; }

        delegate Message ProcessMessage(Message m);
        void f_callbackProcessMessage(IAsyncResult asyncRes)
        {
            AsyncResult ares = (AsyncResult)asyncRes;
            ProcessMessage delg = (ProcessMessage)ares.AsyncDelegate;
            Message result = delg.EndInvoke(asyncRes);
            this.f_PROCESS_MESSAGE_CALLBACK_RESULT(result);
            //Thread.Sleep(1000);
            f_runLoop(this.Handle);
        }

        void f_sleepAfterLoop(IJobHandle handle)
        {
            //Tracer.WriteLine("J{0} WAITING ...", this.Id);
            Thread.Sleep(JOB_CONST.JOB_TIMEOUT_RUN);
            f_runLoop(handle);
        }

        bool timeout = false;
        public void f_runLoop(IJobHandle handle)
        {
            // Create the token source.
            //CancellationTokenSource cts = new CancellationTokenSource();

            /* 4: stop */
            if (this.Status == 4) f_sleepAfterLoop(handle);

            /* 1: init */
            if (this.Status == 1)
            {
                System.Tracer.WriteLine("J{0}_{1} JobBaseUrl -> INIT", this.Id, this.Type);
                this.Handle = handle;
                this.f_INIT();
                this.Status = 2; /* 2: running */
                f_runLoop(handle);
            }

            /* 2: running */
            if (this.Status == 2)
            {
                Message m = null;

                if (this.Handle.Factory != null) 
                    m = this.Handle.Factory.f_getMessage(null);

                // WAITING TO RECEIVED MESSAGE ...
                if (m == null)
                    f_sleepAfterLoop(handle);
                else
                {
                    //Tracer.WriteLine("J{0} PROCESSING ...", this.Id);
                    // PROCESSING MESSAGE

                    string htm = string.Empty,
                        url = "http://localhost:3456/";

                    m.Input = url;

                    var request = CreateWebRequest(url);
                    IAsyncResult asyncRes = request.BeginGetResponse(null, request);

                    /// CHECK TIMEOUT = Poll IAsyncResult.IsCompleted
                    int counter = 1000, timeExpire = m.f_getTimeOutMillisecond();
                    timeout = false;
                    while (asyncRes.IsCompleted == false)
                    { 
                        Thread.Sleep(1000);  // emulate that method is busy
                        counter += 1000;
                        if (timeExpire > 0 && timeExpire <= counter)
                        {
                            timeout = true;
                            request.Abort();
                            break;
                        }
                    }
                    if (timeout)
                    {
                        // TIMEOUT: REQUEST FAILD
                        m.f_setErrorTimeOut();
                    }
                    else {
                        // REQUEST SUCCESS
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncRes);
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                            htm = reader.ReadToEnd();
                        response.Close();
                                                
                        m.Output.Ok = true;
                        m.Output.SetData(htm);
                    }

                    if (this.Handle.Factory != null)
                        this.Handle.Factory.f_sendResponseEvent(m);
                }
            }
            /// end function
            ///////////////////////
        }
        
        const string RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0";
        static WebRequest CreateWebRequest(string url, int timeout = 50 * 1000)
        {
            url = HttpUtility.UrlDecode(url); 
            var create = (HttpWebRequest)WebRequest.Create(url);
            create.UserAgent = RequestUserAgent;
            create.Timeout = timeout;
            return create;
        }

        ~JobBaseUrl()
        {
        }
    }
}
