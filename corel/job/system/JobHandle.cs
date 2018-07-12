using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace corel
{
    public class JobHandle : IJobHandle
    {
        public IJob Job { get; }
        public IJobFactory Factory { get; }
        public JOB_HANDLE State { get; private set; }
         
        RegisteredWaitHandle HandleJob;

        public JobHandle(IJob job)
        {
            this.Job = job; 
            this.State = JOB_HANDLE.RUN;

            this.HandleJob = ThreadPool.RegisterWaitForSingleObject(
                new AutoResetEvent(false),
                new WaitOrTimerCallback((state, timeOut) => { this.Job.f_runLoop(this); }),
                this,
                JOB_CONST.JOB_TIMEOUT_RUN,
                true); /* true: run once; false: repeate */
        }

        public JobHandle(IJob job, IJobFactory factory)
        {
            this.Factory = factory;

            this.Job = job;
            this.State = JOB_HANDLE.RUN;

            this.HandleJob = ThreadPool.RegisterWaitForSingleObject(
                new AutoResetEvent(false),
                new WaitOrTimerCallback((state, timeOut) => { this.Job.f_runLoop(this); }),
                this,
                JOB_CONST.JOB_TIMEOUT_RUN,
                true); /* true: run once; false: repeate */
        }

        void f_postEventStopLoop()
        {
            this.Job.f_stop();
            // new AutoResetEvent(false) ----> call: if (this.EvenStopLoop != null) this.EvenStopLoop.Set();
        }

        public void f_actionJobCallback()
        {
            switch (this.State)
            {
                case JOB_HANDLE.PAUSE:
                    break;
                case JOB_HANDLE.STOP:
                    break;
                case JOB_HANDLE.RESET:
                    //if (this.HandleJob != null)
                    //{
                    //    this.HandleJob.Unregister(null);
                    //    this.HandleJob = null;
                    //}

                    //this.EvenStopLoop.Reset();

                    //this.HandleJob = ThreadPool.RegisterWaitForSingleObject(
                    //    this.EvenStopLoop,
                    //    new WaitOrTimerCallback(this.Job.f_runLoop),
                    //    this,
                    //    JOB_CONST.JOB_TIMEOUT_RUN,
                    //    false);
                    break;
                case JOB_HANDLE.REMOVE:
                    if (this.HandleJob != null)
                    {
                        this.HandleJob.Unregister(null);
                        this.HandleJob = null;
                    }
                    if (this.Factory != null)
                        this.Factory.f_jobFactoryStateChanged(this.Job, this.State);
                    else
                        this.Job.JobContext.f_jobSingletonStateChanged(this.Job, this.State);
                    break;
            }
        }

        public void f_actionJob(JOB_HANDLE action)
        {
            if (this.State == action) return;
            switch (action)
            {
                case JOB_HANDLE.RUN:                    
                    this.State = action;
                    //this.HandleJob = ThreadPool.RegisterWaitForSingleObject(
                    //    this.EvenStopLoop,
                    //    new WaitOrTimerCallback(this.Job.f_runLoop),
                    //    this,
                    //    JOB_CONST.JOB_TIMEOUT_RUN,
                    //    false);
                    break;
                case JOB_HANDLE.PAUSE:
                    this.State = action;
                    f_postEventStopLoop();
                    break;
                case JOB_HANDLE.STOP:
                    this.State = action;
                    f_postEventStopLoop();
                    break;
                case JOB_HANDLE.RESET:
                    this.State = action;
                    f_postEventStopLoop();
                    break;
                case JOB_HANDLE.REMOVE:
                    this.State = action;
                    f_postEventStopLoop();
                    break;
                case JOB_HANDLE.CLEAR:
                    break;
            }
        }
        
        public void f_receiveMessage(Message m) => this.Job.f_receiveMessage(m);
        public void f_receiveMessages(Message[] ms) => this.Job.f_receiveMessages(ms);

        public override string ToString() { return this.Job.f_getId().ToString(); }
    }
}
