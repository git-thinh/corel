using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace corel
{
    public class JobFactoryBase : IJobFactory
    {
        readonly ConcurrentDictionary<int, IJobHandle> JobHandles;
        readonly JOB_TYPE Type;
        protected readonly ConcurrentQueue<Message> Messages;

        public JobFactoryBase(JOB_TYPE type)
        {
            this.JobHandles = new ConcurrentDictionary<int, IJobHandle>();
            this.Messages = new ConcurrentQueue<Message>();
            this.Type = type;
        }

        public IJobHandle f_createNew(IJob job)
        {
            // The main thread uses AutoResetEvent to signal the
            // registered wait handle, which executes the callback
            // method: new AutoResetEvent(???)
            //          + true = signaled -> thread continous run
            //          + false = non-signaled -> thread must wait
            //      EventWaitHandle có ba phương thức chính bạn cần quan tâm:
            //      – Close: giải phóng các tài nguyên được sử dụng bởi WaitHandle.
            //      – Reset: chuyển trạng thái của event thành non-signaled.
            //      – Set: chuyển trạng thái của event thành signaled.
            //      – WaitOne([parameters]): Chặn thread hiện tại cho đến khi trạng thái của event được chuyển sang signaled.

            IJobHandle jo = new JobHandle(job, this);
            int id = job.f_getId();
            JobHandles.TryAdd(id, jo);

            return jo;
        }

        public JobInfo[] f_getAllJobs()
        {
            return this.JobHandles.Select(x => new JobInfo() { Id = x.Key, Type = x.Value.Job.Type }).ToArray();
        }

        public int f_count()
        {
            return JobHandles.Count;
        }

        public void f_actionJobs(JOB_HANDLE action)
        {
            foreach (var kv in this.JobHandles)
                kv.Value.f_actionJob(action);
        }

        public virtual void f_sendRequests(Message[] ms)
        {
            for (int i = 0; i < ms.Length; i++)
                this.Messages.Enqueue(ms[i]);
        }

        public virtual Message f_getMessage(Message msgDefault)
        {
            Message m = msgDefault;
            this.Messages.TryDequeue(out m);
            return m;
        }

        public virtual void f_sendResponseEvent(Message m) { }

        public void f_jobFactoryStateChanged(IJob job, JOB_HANDLE state)
        {
            Tracer.WriteLine("JOB_FACTORY STATE CHANGED: {0}.{1} = {2}", job.Type, job.f_getId(), state);
        }

        ~JobFactoryBase()
        {
            //this.JobHandles.ExecuteFunc(JOB_ACTION, JOB_HANDLE.REMOVE);
        }
    }
}
