using Salar.Bois;
using System;
using System.IO;
using System.Threading;

namespace corel
{
    public class JobTest : JobBase
    {
        public JobTest(IJobContext jobContext) : base(jobContext, JOB_TYPE.NONE) { }

        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} TEST -> INIT", this.f_getId(), this.Type);
        }
        public override void f_STOP()
        {
            Tracer.WriteLine("J{0}_{1} TEST -> STOP", this.f_getId(), this.Type);
        }

        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m)
        {
            //Tracer.WriteLine("J{0} DONE: {1}-{2} ", this.f_getId(), m.Input, m.GetMessageId());
            this.JobContext.MessageContext.f_responseMessage(m);
        }
        public override Message f_PROCESS_MESSAGE(Message m)
        {
            m.Output = new MessageResult() { Ok = true };
            Thread.Sleep(2000);

            //var boisSerializer = new BoisSerializer();
            //using (var mem = new MemoryStream())
            //{
            //    boisSerializer.Serialize(this, mem);
            //    var mm = boisSerializer.Deserialize(mem.GetBuffer(), m.GetType(), 0, (int)mem.Length);
            //}

            //Message m = null;
            //m = this.Messages.Dequeue(null);
            //if (m != null)
            //{
            //    if (m.f_getTimeOut() > 0)
            //        Thread.Sleep(m.f_getTimeOut() + 3000);

            //    Tracer.WriteLine("J{0} TEST: Do something: {1} ", this.f_getId(), m.GetMessageId());

            //    m.Output = new MessageResult()
            //    {
            //        Ok = true,
            //        MessageText = string.Format("J{0} TEST: Done {1} ", this.f_getId(), m.GetMessageId())
            //    };
            //    //this.JobAction.f_eventJobResponseMessage(this.f_getId(), m);
            //    this.JobContext.MessageContext.f_responseMessage(m);
            //}
            //else Tracer.WriteLine("J{0} TEST: waiting message to execute that ...", this.f_getId());
            return m;
        }

    }
}
