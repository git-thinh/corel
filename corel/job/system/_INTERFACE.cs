using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace corel
{
    public interface IJobContext
    {
        JobInfo[] f_getAllJobs();
        int f_getTotalJob();
        IMessageContext MessageContext { get; }

        IJobHandle f_createNew(IJob job);
        bool f_sendRequestMessages(JOB_TYPE type, Message[] ms, Func<IJobHandle, Guid, bool> responseCallbackDoneAll);
        void f_jobSingletonStateChanged(IJob job, JOB_HANDLE state);

        void f_removeAll();
    }
    
    public interface IJobFactory
    {
        JobInfo[] f_getAllJobs();
        void f_actionJobs(JOB_HANDLE action);
        int f_count();

        IJobHandle f_createNew(IJob job);

        Message f_getMessage(Message msgDefault);

        void f_sendRequests(Message[] messages);
        void f_sendResponseEvent(Message m);

        void f_jobFactoryStateChanged(IJob job, JOB_HANDLE state);
    }

    public interface IJobHandle
    {
        IJob Job { get; }
        IJobFactory Factory { get; }
        JOB_HANDLE State { get; }
        
        void f_actionJob(JOB_HANDLE action);
        void f_actionJobCallback();

        void f_receiveMessage(Message m);
        void f_receiveMessages(Message[] ms);
    }
    
    public interface IJob
    {
        int f_getId();

        IJobHandle Handle { get; }
        IJobContext JobContext { get; }
        JOB_STATE State { get; }
        JOB_TYPE Type { get; }

        void f_receiveMessage(Message m);
        void f_receiveMessages(Message[] ms);
        
        void f_stop();
        void f_runLoop(IJobHandle handle);
    }
    
    public interface IMessageContext
    {
        void f_responseMessage(Message m);
        Message f_responseMessageGet(Guid id);

        void f_sendRequestMessages(JOB_TYPE type, Message[] ms, Func<IJobHandle, Guid, bool> responseCallbackDoneAll);

        //void f_eventRequestGroupMessageComplete(Guid groupId);
        //void f_eventRequestMessageTimeOut(Guid[] IdsExpired);
    }

}
