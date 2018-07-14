using System;
using System.Threading;
using System.Linq;
using NHttp;
using System.IO;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Fleck;
using System.Collections.Generic;

namespace corel
{
    public class JobWebSocketServerTest : JobBase
    {
        int Port = 8181;
        readonly WebSocketServer server;
        readonly List<IWebSocketConnection> allSockets;

        readonly ConcurrentDictionary<string, string> fileData;

        public JobWebSocketServerTest(IJobContext jobContext) : base(jobContext, JOB_TYPE.WEB_SOCKET_SERVER)
        {
            fileData = new ConcurrentDictionary<string, string>();

            server = new WebSocketServer("ws://0.0.0.0:" + this.Port.ToString());
            this.allSockets = new List<IWebSocketConnection>();
        }

        void f_sendBroadCast(string text)
        {
            foreach (var socket in allSockets)
                socket.Send(text);
        }

        public override void f_INIT()
        {
            Tracer.WriteLine("J{0}_{1} {2} -> INIT", this.f_getId(), this.Type, this.GetType().Name);
            Tracer.WriteLine(String.Format("SERVER: http://127.0.0.1:{0}/", Port));

            //FleckLog.Level = LogLevel.Debug;
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    allSockets.Add(socket);
                    f_sendBroadCast("SERVER SAY: WELCOME CLIENT_" + allSockets.Count.ToString() + " ...!");
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                };
            });

        }

        public override void f_STOP()
        {
            fileData.Clear();

            foreach (var socket in allSockets) { socket.Close(); }
            server.Dispose();

            Tracer.WriteLine("J{0}_{1} {2} -> STOP", this.f_getId(), this.Type, this.GetType().Name);
        }
        public override void f_PROCESS_MESSAGE_CALLBACK_RESULT(Message m)
        {
        }
        public override Message f_PROCESS_MESSAGE(Message m)
        {
            return m;
        }
    }
}
