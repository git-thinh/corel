using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace corel
{
    public enum MLOG_TYPE
    {
        NONE = 0,
        LOG_ALL = 1,
        LOG_UI = 2,
        LOG_CONSOLE = 3,
        LOG_TRACE = 4
    }

    public class MessageResult
    {
        public int PageNumber { set; get; }
        public int PageSize { set; get; }
        public int Counter { set; get; }
        public int Total { set; get; }

        public bool Ok { set; get; }
        public string MessageText { set; get; }

        public MessageResult()
        {
            PageNumber = 1;
            PageSize = 10;
            Counter = 0;
            Total = 0;
            Ok = false;
        }

        private object data;
        public object GetData() { return data; }
        public void SetData(object _data) { data = _data; }
    }

    public enum SENDER_TYPE
    {
        /// <summary>
        /// no response because sender was hidden
        /// </summary>
        HIDE_SENDER, // NO_RESPONSE
        IS_FORM,
        IS_JOB,
    }

    public enum MESSAGE_ACTION
    {
        ITEM_ADD_NEW,
        ITEM_EDIT,
        ITEM_REMOVE,
        ITEM_SEARCH,
        URL_REQUEST_ONLINE,
        URL_REQUEST_CACHE,
        URL_RESPONSE,
    }

    public enum MESSAGE_TYPE
    {
        REQUEST,
        RESPONSE,
    }

    public class Message
    {
        readonly Guid Id = Guid.Empty;
        Guid GroupId = Guid.Empty;

        bool hasErrorTimeOut = false;
        public void f_setErrorTimeOut() { this.hasErrorTimeOut = true; }
        public bool IsTimeOut() { return this.hasErrorTimeOut; }

        int TimeOut = 0;
        public void f_setTimeOutMillisecond(int timeOut) { this.TimeOut = timeOut; }
        public int f_getTimeOutMillisecond() { return this.TimeOut; }
        
        int JobExecuteId = 0;
        public void f_setJobExecuteId(int jobExecuteId) { this.JobExecuteId = jobExecuteId; }
        public int f_getJobExecuteId() { return this.JobExecuteId; }








        readonly int SenderId;
        readonly int[] JobReceiveID;
        readonly SENDER_TYPE SenderType;
        readonly MESSAGE_ACTION Action;
        public MESSAGE_TYPE Type { set; get; }
        public string JobName { set; get; }

        public MessageResult Output { set; get; }
        public object Input { set; get; }

        public Message()
        {
            Id = Guid.NewGuid();
            Output = new MessageResult();
        }

        public Message(int senderId, int[] job_receive_IDs, MESSAGE_ACTION action, object input = null, SENDER_TYPE senderType = SENDER_TYPE.IS_FORM) : base()
        {
            this.Type = MESSAGE_TYPE.REQUEST;
            Action = action;
            SenderType = senderType;
            SenderId = senderId;
            JobReceiveID = job_receive_IDs;

            //Id = Guid.NewGuid();
            //Output = new MessageResult();
            Input = input;
        }

        public Message SetGroupId(Guid groupId)
        {
            this.GroupId = groupId;
            return this;
        }

        public Guid GetGroupId()
        {
            return this.GroupId;
        }

        public Guid GetMessageId()
        {
            return Id;
        }

        public int GetSenderId()
        {
            return SenderId;
        }

        public int[] GetReceiverId()
        {
            return JobReceiveID;
        }

        public MESSAGE_ACTION getAction()
        {
            return Action;
        }
        public SENDER_TYPE getSenderType()
        {
            return SenderType;
        }
    }

}
