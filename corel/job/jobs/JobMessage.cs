using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace corel
{
    public class JobMessage : JobBase, IMessageContext
    {
        #region [ VARIABLE ]

        //readonly ConcurentQueue<Guid> ResponseIds;
        //readonly DictionaryThreadSafe<Guid, Message> ResponseMessages;

        //readonly ConcurentQueue<Message> Messages;

        //readonly DictionaryThreadSafe<Guid, List<Guid>> RequestMessageGroup;
        //readonly DictionaryThreadSafe<Guid, int> RequestMessageGroupTotal;
        //readonly DictionaryThreadSafe<Guid, Func<IJobHandle, Guid, bool>> RequestMessageGroupAction
        //    = new DictionaryThreadSafe<Guid, Func<IJobHandle, Guid, bool>>();


        readonly ConcurrentDictionary<Guid, Message> MessagesRequest;
        readonly ListThreadSafe<Guid> MessagesRequestTimeOutExpire;
        readonly ListDoubleThreadSafe<long, Guid> MessagesRequestTimeOut;
        readonly System.Threading.Timer timer;
        static readonly Func<List<long>, long, int[]> FUNC_TIME_OUT_FILTER = (list, valueCompare) =>
        {
            List<int> ls = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                //Tracer.WriteLine("{0} === {1}", list[i], valueCompare);
                if (list[i] <= valueCompare)
                    ls.Add(i);
            }
            return ls.ToArray();
        };

        #endregion

        public JobMessage(IJobContext jobContext) : base(jobContext, JOB_TYPE.MESSAGE)
        {
            //this.Messages = new ConcurentQueue<Message>();

            //this.ResponseIds = new ConcurentQueue<Guid>();
            //this.ResponseMessages = new DictionaryThreadSafe<Guid, Message>();
            //this.RequestMessageGroup = new DictionaryThreadSafe<Guid, List<Guid>>();
            //this.RequestMessageGroupTotal = new DictionaryThreadSafe<Guid, int>();

            this.MessagesRequest = new ConcurrentDictionary<Guid, Message>();
            this.MessagesRequestTimeOutExpire = new ListThreadSafe<Guid>();
            this.MessagesRequestTimeOut = new ListDoubleThreadSafe<long, Guid>();
            this.timer = new System.Threading.Timer(new System.Threading.TimerCallback((obj) =>
            {
                if (this.MessagesRequestTimeOut.Count == 0) return;

                long timeStart = DateTime.Now.Ticks / 1000;
                Guid[] ids = this.MessagesRequestTimeOut.FindItem1LessThanAndRemove(FUNC_TIME_OUT_FILTER, timeStart);
                if (ids.Length > 0)
                {
                    foreach (var t in ids)
                        Tracer.WriteLine("RESPONSE TIME_OUT: {0}", t);

                    this.MessagesRequestTimeOutExpire.AddRange(ids);
                }
            }), null, 100, 100);
        }
        
        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
        }
        public override void f_STOP()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> STOP", this.f_getId(), this.Type, this.GetType().Name);
        }
        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m)
        {
            Thread.Sleep(1000);
        }
        public override Message f_PROCESS_MESSAGE(Message m)
        {
            //Message m = null;
            //m = this.Messages.Dequeue(null);
            //if (m != null)
            //{
            //    //Guid id = m.GetMessageId();
            //    //if (this.MsgTimeOutExpire.Count > 0 && this.MsgTimeOutExpire.IndexOf(id) != -1)
            //    //{
            //    //    this.MsgTimeOutExpire.Remove(id);
            //    //    return;
            //    //}

            //    //this.ResponseIds.Enqueue(id);
            //    //this.ResponseMessages.Add(id, m);

            //    //Guid groupId = m.GetGroupId();
            //    //if (this.RequestMessageGroup.ContainsKey(groupId))
            //    //{
            //    //    this.RequestMessageGroup.Remove(groupId);
            //    //    if (this.RequestMessageGroup.Count == 0)
            //    //    {
            //    //        System.Tracer.WriteLine("JOB_MESSAGE:  DONE GROUP {0} = {1}", groupId, this.RequestMessageGroupTotal[groupId]);
            //    //        this.RequestMessageGroupAction[groupId](this.MsgContext, this.Handle, groupId);
            //    //    }
            //    //}
            //}
            return m;
        }

        #region [ IMessageContext ]

        public void f_responseMessage(Message m)
        {
            Guid mid = m.GetMessageId();
            //this.ResponseIds.Enqueue(id);
            //this.ResponseMessages.Add(id, m);

            //remove checking timeout
            if (m.f_getTimeOutMillisecond() > 0) {
               bool ok = this.MessagesRequestTimeOut.RemoveItem1AndItem2_byItem2(mid);
            }
        }

        public Message f_responseMessageGet(Guid id)
        {
            //if (this.ResponseMessages.ContainsKey(id))
            //    return this.ResponseMessages[id];
            return null;
        }

        public void f_sendRequestMessages(JOB_TYPE type, Message[] ms, Func<IJobHandle, Guid, bool> responseCallbackDoneAll)
        {
            foreach (Message m in ms)
            {
                Guid id = m.GetMessageId();
                if (m.f_getTimeOutMillisecond() > 0)
                {
                    long timeOut = DateTime.Now.Ticks / 1000 + m.f_getTimeOutMillisecond();
                    this.MessagesRequestTimeOut.Add(timeOut, id);
                }
                this.MessagesRequest.TryAdd(id, m);
            }
        }

        #endregion

        ~JobMessage()
        {
            this.timer.Dispose();
            this.MessagesRequest.Clear();
            this.MessagesRequestTimeOutExpire.Clear();
            this.MessagesRequestTimeOut.Clear();

            //this.ResponseMessages.Clear();
            //this.RequestMessageGroup.Clear();
        }
    }
}
