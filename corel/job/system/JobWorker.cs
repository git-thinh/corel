using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace corel
{
    public class JobWorker : IJob
    {
        readonly ConcurrentQueue<Message> Messages;
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

        public JobWorker(IJobContext jobContext, JOB_TYPE type)
        {
            this.Messages = new ConcurrentQueue<Message>();

            this.JobContext = jobContext;
            this.Id = jobContext.f_getTotalJob() + 1;
            this.Type = type;
            this.Status = 1; /* 1: init */
        }

        public void f_receiveMessage(Message m)
        {
            this.Messages.Enqueue(m);
        }
        public void f_receiveMessages(Message[] ms)
        {
            for (int i = 0; i < ms.Length; i++)
                this.Messages.Enqueue(ms[i]);
        }
        public void f_stop()
        {
            this.Status = 4; /* 4: stop */
            System.Tracer.WriteLine("J{0} BASE: STOP ...", this.Id);
            this.Handle.f_actionJobCallback();
        }

        public virtual void f_init() { }
        public virtual void f_processMessageCallbackResult(Message m) { }
        public virtual Message f_processMessage(Message m) { return m; }

        delegate Message ProcessMessage(Message m);
        void f_callbackProcessMessage(IAsyncResult asyncRes)
        {
            AsyncResult ares = (AsyncResult)asyncRes;
            ProcessMessage delg = (ProcessMessage)ares.AsyncDelegate;
            Message result = delg.EndInvoke(asyncRes);
            this.f_processMessageCallbackResult(result);
            //Thread.Sleep(1000);
            f_runLoop(this.Handle);
        }

        void f_sleepAfterLoop(IJobHandle handle)
        {
            Thread.Sleep(JOB_CONST.JOB_TIMEOUT_RUN);
            f_runLoop(handle);
        }

        public void f_runLoop(IJobHandle handle)
        {
            // Create the token source.
            CancellationTokenSource cts = new CancellationTokenSource();

            /* 4: stop */
            if (this.Status == 4) f_sleepAfterLoop(handle);

            /* 1: init */
            if (this.Status == 1)
            {
                System.Tracer.WriteLine("J{0} BASE: INITED ...", this.Id);
                this.Handle = handle;
                this.f_init();
                this.Status = 2; /* 2: running */
                f_runLoop(handle);
            }

            /* 2: running */
            if (this.Status == 2)
            {
                Message m = null;

                if (this.Handle.Factory == null)
                {
                    if (!this.Messages.IsEmpty) { this.Messages.TryDequeue(out m); }
                }
                else
                    m = this.Handle.Factory.f_getMessage(null);

                if (m == null)
                {
                    //Tracer.WriteLine("J{0} BASE: WAITING ...", this.Id);
                    // WAITING TO RECEIVED MESSAGE ...
                    f_sleepAfterLoop(handle);
                }
                else
                {
                    //Tracer.WriteLine("J{0} BASE: PROCESSING ...", this.Id);
                    // PROCESSING MESSAGE
                    ProcessMessage fun = this.f_processMessage;
                    IAsyncResult asyncRes = fun.BeginInvoke(m, new AsyncCallback(f_callbackProcessMessage), null);

                    ///// check timeout ...
                    //IAsyncResult asyncRes = fun.BeginInvoke(m, null, null);
                    //// Poll IAsyncResult.IsCompleted
                    //while (asyncRes.IsCompleted == false)
                    //{
                    //    Console.WriteLine("Square Number still processing");
                    //    Thread.Sleep(1000);  // emulate that method is busy
                    //}
                    //Console.WriteLine("Square Number processing completed");
                    //Guid res = fun.EndInvoke(asyncRes);
                }
            }
            /// end function
            ///////////////////////
        }

        ~JobWorker()
        {
        }
    }
}
