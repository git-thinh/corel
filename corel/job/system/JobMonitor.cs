using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace corel
{
    public class JobMonitor : IJobContext
    {
        readonly IJobHandle MessageHandle;
        readonly ConcurrentDictionary<JOB_TYPE, IJobFactory> JobFactories = new ConcurrentDictionary<JOB_TYPE, IJobFactory>();
        readonly ConcurrentDictionary<JOB_TYPE, IJobHandle> JobSingletons = new ConcurrentDictionary<JOB_TYPE, IJobHandle>();

        public IMessageContext MessageContext { get; }

        public JobMonitor()
        {
            var m = new JobMessage(this);
            this.MessageContext = m;
            this.MessageHandle = f_createNew(m);
            //f_createNew(new JobLink(this));
        }

        public IJobHandle f_createNew(IJob job)
        {
            IJobHandle handle = null;
            JOB_TYPE type = job.Type;
            switch (type)
            {
                case JOB_TYPE.NONE:
                case JOB_TYPE.REQUEST_URL:
                    // factory
                    IJobFactory fac;
                    if (this.JobFactories.ContainsKey(type))
                    {
                        fac = this.JobFactories[type];
                    }
                    else
                    {
                        fac = new JobFactory(type);
                        this.JobFactories.TryAdd(type, fac);
                    }
                    handle = fac.f_createNew(job);
                    break;
                default:
                    // singleton
                    if (!this.JobSingletons.ContainsKey(type))
                    {
                        handle = new JobHandle(job);
                        this.JobSingletons.TryAdd(type, handle);
                    }
                    break;
            }
            return handle;
        }
        
        public int f_getTotalJob()
        {
            IJobFactory[] facs = this.JobFactories.Values.ToArray();
            return facs.Sum(x => x.f_count()) + this.JobSingletons.Count;
        }

        public void f_removeAll()
        {
            foreach (var kv in this.JobFactories)
                kv.Value.f_actionJobs(JOB_HANDLE.REMOVE);
            foreach (var kv in this.JobSingletons)
                kv.Value.f_actionJob(JOB_HANDLE.REMOVE);
        }

        public void f_runAll()
        { 
            foreach (var kv in this.JobFactories) 
                    kv.Value.f_actionJobs(JOB_HANDLE.RUN);
            foreach (var kv in this.JobSingletons)
                kv.Value.f_actionJob(JOB_HANDLE.RUN);
        }
        
        public bool f_sendRequestMessages(JOB_TYPE type, Message[] ms,
            Func<IJobHandle, Guid, bool> responseCallbackDoneAll)
        {
            if (this.JobFactories.ContainsKey(type))
            {
                if (responseCallbackDoneAll != null)
                    if (this.MessageContext != null)
                        this.MessageContext.f_sendRequestMessages(type, ms, responseCallbackDoneAll);
                this.JobFactories[type].f_sendRequestLoadBalancer(ms);
            }
            else if (this.JobSingletons.ContainsKey(type))
            {
                //this.JobSingletons[type].f_sendMessages(ms);
            }
            return false;
        }

        public void f_jobSingletonStateChanged(IJob job, JOB_HANDLE state)
        {
            Tracer.WriteLine("JOB_SINGLETON STATE CHANGED: {0}.{1} = {2}", job.Type, job.f_getId(), state);
        }

        ~JobMonitor()
        {
            f_removeAll();
            
            this.JobFactories.Clear();
            this.JobSingletons.Clear();
        }
    }
}
