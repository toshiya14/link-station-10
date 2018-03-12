using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class NamedPipeHelper
    {
        private const string PIPE_NAME = @"RMEGo.Sunflower.LinkStation10.PipeStream";
        private const int LISTENER_BUFFER_SIZE = 16;
        private static bool isListening;
        private static List<Tuple<Func<string, bool>, Action<string>>> processors = new List<Tuple<Func<string, bool>, Action<string>>>();
        public static Queue<string> MessageQueue { get; private set; }

        static NamedPipeHelper()
        {
            MessageQueue = new Queue<string>();
        }

        private static void ProcessNextMessage()
        {
            var message = MessageQueue.Dequeue();
            foreach (var t in processors)
            {
                if (t.Item1(message))
                {
                    Debug.WriteLine("[NamedPipeHelper.Server] Start to process message: " + message);
                    t.Item2(message);
                    return;
                }
            }
            Debug.WriteLine("[NamedPipeHelper.Server] Cannot process and throw message: " + message);
        }

        private static void WaitData()
        {
            using (NamedPipeServerStream stream = new NamedPipeServerStream(PIPE_NAME, PipeDirection.InOut, 1))
            {
                // Wait for connection();
                Debug.WriteLine("[NamedPipeHelper.Server] Waiting for connection.");
                stream.WaitForConnection();
                stream.ReadMode = PipeTransmissionMode.Byte;
                using (var reader = new StreamReader(stream))
                {
                    var message = reader.ReadToEnd();
                    Debug.WriteLine("[NamedPipeHelper.Server] Receive a message: " + message);
                    MessageQueue.Enqueue(message);
                }
            }
        }

        public static void StartListenThread()
        {
            isListening = true;
            // Listening thread.
            ThreadPool.QueueUserWorkItem(s =>
            {
                while (isListening)
                {
                    WaitData();
                }
                return;
            });
            // Processing thread.
            ThreadPool.QueueUserWorkItem(s =>
            {
                while (true)
                {
                    if (MessageQueue.Count > 0)
                    {
                        ProcessNextMessage();
                        continue;
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public static void StopListenThread()
        {
            isListening = false;
            Trace.WriteLine("Set listening flag to false, wait for the last processing finished.");
        }

        public static void SendMessageThread(string content)
        {
            ThreadPool.QueueUserWorkItem(s =>
            {
                SendMessage(content);
                return;
            });
        }

        public static void SendMessage(string content) {
            using (var stream = new NamedPipeClientStream("localhost", PIPE_NAME, PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.None))
            {
                Debug.WriteLine("[NamedPipeHelper.Client] Wait for connection.");
                stream.Connect();
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();
                    Debug.WriteLine("[NamedPipeHelper.Client] Sent the message: "+content);
                }
            }
        }

        public static void AddProcessor(Func<string, bool> filter, Action<string> action)
        {
            processors.Add(new Tuple<Func<string, bool>, Action<string>>(filter, action));
        }
    }
}
