using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace corel
{

    public class JobFactory : IJobFactory
    {
        static readonly Func<IJobHandle, object, bool> JOB_ACTION = (IJobHandle handle, object para) =>
         {
             JOB_HANDLE cmd = (JOB_HANDLE)para;
             handle.f_actionJob(cmd);
             return true;
         };

        static readonly Func<IJobHandle[], Message[], bool> SEND_MESSAGE_LOAD_BALANCER_TO_JOB = (IJobHandle[] handles, Message[] ms) =>
        {
            int count = ms.Length, i = 0, id = 0;
            while (count > 0)
            {
                i = 0;
                for (i = 0; i < handles.Length; i++)
                {
                    ms[id].f_setJobExecuteId(handles[i].Job.f_getId());
                    handles[i].f_receiveMessage(ms[id]);
                    System.Tracer.WriteLine("J{0}: {1} = {2}", handles[i].Job.f_getId(), id, ms[id].GetMessageId());

                    id++;
                    if (id == ms.Length)
                    {
                        count = 0;
                        break;
                    }
                }
                count = count - i;
            }
            return true;
        };

        readonly ConcurrentDictionary<int, IJobHandle> JobHandles;
        readonly JOB_TYPE Type;
        readonly ConcurrentQueue<Message> Messages;

        public JobFactory(JOB_TYPE type)
        {
            this.Messages = new ConcurrentQueue<Message>();

            this.Type = type;
            JobHandles = new ConcurrentDictionary<int, IJobHandle>();
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

        public int f_count()
        {
            return JobHandles.Count;
        }

        public void f_actionJobs(JOB_HANDLE action)
        {
            //this.JobHandles.ExecuteFunc(JOB_ACTION, action);
        }

        public void f_sendRequestLoadBalancer(Message[] ms)
        {
            //this.JobHandles.ExecuteFuncLoadBalancer<Message>(SEND_MESSAGE_LOAD_BALANCER_TO_JOB, messages);
            for (int i = 0; i < ms.Length; i++)
            {
                this.Messages.Enqueue(ms[i]);
            }
        }

        public Message f_getMessage(Message msgDefault)
        {
            Message m = msgDefault;
            this.Messages.TryDequeue(out m);
            return m;
        }

        public void f_jobFactoryStateChanged(IJob job, JOB_HANDLE state)
        {
            Tracer.WriteLine("JOB_FACTORY STATE CHANGED: {0}.{1} = {2}", job.Type, job.f_getId(), state);
        }

        ~JobFactory()
        {
            //this.JobHandles.ExecuteFunc(JOB_ACTION, JOB_HANDLE.REMOVE);
        }
    }
}
