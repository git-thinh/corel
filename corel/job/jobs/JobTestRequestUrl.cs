﻿using Salar.Bois;
using System;
using System.IO;
using System.Threading;

namespace corel
{
    public class JobTestRequestUrl : JobBaseUrl
    {
        public JobTestRequestUrl(IJobContext jobContext) : base(jobContext, JOB_TYPE.REQUEST_URL)
        {
        }
        
        public override void f_INIT()
        {
            Tracer.WriteLine("J{0} TEST -> INITED", this.f_getId());
        }
        public override void f_STOP()
        {
            Tracer.WriteLine("J{0} {1} -> STOPED", this.Type, this.f_getId());
        }
        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m) {
            Tracer.WriteLine("J{0} DONE: {1}-{2} ", this.f_getId(), m.Input, m.GetMessageId());
            this.JobContext.MessageContext.f_responseMessage(m);            
        }
        public override Message f_PROCESS_MESSAGE(Message m)
        { 
            return m;
        }

    }
}
