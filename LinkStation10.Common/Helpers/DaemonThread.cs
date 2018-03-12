using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class DaemonThread
    {
        private Action StartAction;
        private Action ThreadAction;
        private Action StopAction;
        private string DaemonName;
        public bool IsRunning;
        private bool IsAlive;
        public DaemonThread(string DaemonName, Action ThreadAction, Action StartAction = null, Action StopAction = null)
        {
            this.IsRunning = false;
            this.DaemonName = DaemonName;
            this.ThreadAction = ThreadAction;
            if (ThreadAction == null)
            {
                throw new NullReferenceException("Thread action could not be null.");
            }
            if (StartAction == null)
            {
                this.StartAction = () => { };
            }
            else
            {
                this.StartAction = StartAction;
            }
            if (StopAction == null)
            {
                this.StopAction = () => { };
            }
            else
            {
                this.StopAction = StopAction;
            }
        }
        public void Start() {
            if (IsRunning) {
                Debug.WriteLine("[DaemonThread." + DaemonName + "] Warning: This thread is already runing.");
                return;
            }
            IsRunning = true;
            ThreadPool.QueueUserWorkItem(s=> {
                try
                {
                    this.IsAlive = true;
                    StartAction();
                    while (this.IsAlive) {
                        ThreadAction();
                    }
                    StopAction();
                    this.IsAlive = false;
                    this.IsRunning = false;
                    return;
                } catch (Exception e) {
                    Debug.Fail("[DaemonThread." + DaemonName + "] Fatal: Exception `" + e.Message + "` happens.");
                    Debug.Fail(e.StackTrace);
                    IsRunning = false;
                    throw e;
                }
            });
        }

        public static DaemonThread StartNew(string DaemonName, Action ThreadAction, Action StartAction = null, Action StopAction = null) {
            var thread = new DaemonThread(DaemonName, ThreadAction, StartAction, StopAction);
            thread.Start();
            return thread;
        }

        public static DaemonThread WaitStartNew(string DaemonName, Action ThreadAction, Action StartAction = null, Action StopAction = null) {
            var thread = new DaemonThread(DaemonName, ThreadAction, StartAction, StopAction);
            thread.WaitStart();
            return thread;
        }

        public void Stop() {
            IsAlive = false;
        }

        public void Restart() {
            WaitStop();
            Start();
        }

        public void WaitStop() {
            Stop();
            while (this.IsRunning) {
                Thread.Sleep(100);
            }
        }

        public void WaitStart() {
            Start();
            while (!this.IsAlive) {
                Thread.Sleep(100);
            }
        }
    }
}
