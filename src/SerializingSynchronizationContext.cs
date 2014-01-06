using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SerializingSynchronizationContextDemo
{
    public sealed class SerializingSynchronizationContext : SynchronizationContext
    {
        #region Private members

        private readonly object _lock = new object();
        private readonly ConcurrentQueue<CallbackInfo> _queue = new ConcurrentQueue<CallbackInfo>();

        #endregion

        #region Overrides

        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Enqueue(new CallbackInfo(d, state));

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_lock, ref lockTaken);

                if (lockTaken)
                {
                    ProcessQueue();
                }
                else
                {
                    Task.Run((Action)ProcessQueue);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            lock (_lock)
            {
                var outer = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(this);
                    d(state);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(outer);
                }
            }
        }

        #endregion

        #region Private Helpers

        private void ProcessQueue()
        {
            if (!_queue.IsEmpty)
            {
                lock (_lock)
                {
                    var outer = SynchronizationContext.Current;
                    try
                    {
                        SynchronizationContext.SetSynchronizationContext(this);

                        CallbackInfo callback;
                        while (_queue.TryDequeue(out callback))
                        {
                            try
                            {
                                callback.D(callback.State);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception in posted callback on {0}: {1}", 
                                    GetType().FullName, e);                 
                            }
                        }
                    }
                    finally
                    {
                        SynchronizationContext.SetSynchronizationContext(outer);
                    }
                }
            }
        }

        #endregion

        #region Private Structs

        private struct CallbackInfo
        {
            #region Construction / Initializations

            public CallbackInfo(SendOrPostCallback d, object state) : this()
            {
                D = d;
                State = state;
            }

            #endregion

            #region Public members

            public SendOrPostCallback D { get; private set; }

            public object State { get; private set; }

            #endregion
        }

        #endregion
    }
}