using System;
using System.Threading;
using System.Linq;

namespace corel
{
    public class JobLink : JobBase
    {
        readonly DictionaryThreadSafe<string, string> urlData;
        readonly QueueThreadSafe<Message> msg;
        readonly ListThreadSafe<oLink> list;

        public JobLink(IJobAction jobAction) : base(JOB_TYPE.LINK, jobAction)
        {
            list = new ListThreadSafe<oLink>();
            msg = new QueueThreadSafe<Message>();
            urlData = new DictionaryThreadSafe<string, string>();
        }

        public override void f_sendMessage(Message m)
        {
            //if (this.StoreJob != null)
            //    this.StoreJob.f_job_sendMessage(m);
        }

        public override void f_receiveMessage(Message m)
        {
            msg.Enqueue(m);
        }

        public override void f_runLoop(object state, bool timedOut)
        {
            if (!this.m_inited)
            {
                this.m_Handle = (IJobHandle)state;
                this.m_inited = true;
                this.m_state = JOB_STATE.RUNNING;

                list.ReadFile("data/link.dat");
                // Tracer.WriteLine("J{0} executes on thread {1}: INIT ...");
                return;
            }

            JobHandle ti = (JobHandle)state;
            if (!timedOut)
            {
                // Tracer.WriteLine("J{0} executes on thread {1}: SIGNAL -> STOP ...", Id, Thread.CurrentThread.GetHashCode().ToString());
                ti.f_stopJob();
                return;
            }

            // Tracer.WriteLine("J{0} executes on thread {1}:DO SOMETHING ...", Id, Thread.CurrentThread.GetHashCode().ToString());
            // Do something ...

            if (msg.Count > 0)
            {
                Message m = msg.Dequeue(null);
                if (m != null)
                {
                    switch (m.getAction())
                    {
                        case MESSAGE_ACTION.ITEM_SEARCH:
                            #region
                            if (true)
                            {
                                oLink[] a = new oLink[] { };
                                if (m.Input != null)
                                {
                                    string key = m.Input as string;
                                    a = list.Where(x => x.Link.Contains(key) || x.Title.Contains(key) || x.Tags.Contains(key), false, int.MaxValue);
                                    m.Output.Counter = a.Length;
                                }
                                else
                                {
                                    a = list.Take(10).ToArray();
                                    m.Output.Counter = list.Count;
                                }

                                m.Type = MESSAGE_TYPE.RESPONSE;
                                m.JobName = this.f_getGroupName();

                                m.Output.Ok = true;
                                m.Output.PageSize = 10;
                                m.Output.PageNumber = 1;
                                m.Output.Total = list.Count;
                                m.Output.SetData(a);

                                //this.StoreJob.f_responseMessageFromJob(m);
                            }
                            #endregion
                            break;
                        case MESSAGE_ACTION.URL_REQUEST_CACHE:
                            #region
                            if (m.Input != null)
                            {
                                string url = m.Input as string;
                                if (urlData.ContainsKey(url))
                                {
                                    string htm = urlData[url];

                                    m.Type = MESSAGE_TYPE.RESPONSE;
                                    m.JobName = this.f_getGroupName();

                                    m.Output.Ok = true;
                                    m.Output.SetData(htm);

                                    //this.StoreJob.f_responseMessageFromJob(m);
                                }
                                else
                                {
                                    UrlService.GetAsync(url, m, UrlService.Func_GetHTML_UTF8_FORMAT_BROWSER, (result) =>
                                    {
                                        if (result.Ok)
                                        {
                                            string htm = result.Html;
                                            if (!urlData.ContainsKey(url)) urlData.Add(url, htm);

                                            m.Type = MESSAGE_TYPE.RESPONSE;
                                            m.JobName = this.f_getGroupName();

                                            m.Output.Ok = true;
                                            m.Output.SetData(htm);

                                            //this.StoreJob.f_responseMessageFromJob(m);
                                        }
                                    });
                                }
                            }
                            #endregion
                            break;
                    }
                }
            }//end if

        } // end loop
    }
}
