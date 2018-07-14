using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace corel
{
    public class JobFactoryUrl : JobFactoryBase
    {
        public JobFactoryUrl(JOB_TYPE type) : base(type)
        {
        }

        public override void f_sendRequests(Message[] ms)
        {
            for (int i = 0; i < ms.Length; i++)
                this.Messages.Enqueue(ms[i]);
        }

        public override Message f_getMessage(Message msgDefault)
        {
            Message m = msgDefault;
            this.Messages.TryDequeue(out m);
            return m;
        }

        public override void f_sendResponseEvent(Message m)
        {
            if (m.IsTimeOut())
            {
                Tracer.WriteLine("FACTORY_URL: {0} = {1}", m.Input, "TIME_OUT");
            }
            else
            {
                if (m.Output.Ok)
                {
                    Tracer.WriteLine("FACTORY_URL: {0} = \r\n {1}", m.Input, m.Output.GetData());
                }
                else {
                    Tracer.WriteLine("FACTORY_URL: {0} = {1}", m.Input, "FAIL....");
                }
            }
        }

        ~JobFactoryUrl()
        {
            //this.JobHandles.ExecuteFunc(JOB_ACTION, JOB_HANDLE.REMOVE);
        }
    }
}
