using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SerializingSynchronizationContextDemo
{
    public struct SynchronizationContextAwaiter : INotifyCompletion
    {
        #region Private members

        private readonly SynchronizationContext _context;
        private readonly SendOrPostCallback _executor;

        #endregion

        #region Construction / Initializations

        public SynchronizationContextAwaiter(SynchronizationContext context)
        {
            _context = context;
            _executor = a => ((Action)a)();
        }

        #endregion

        #region INotifyCompletion implementation

        public bool IsCompleted
        {
            get { return false; }
        }

        public void OnCompleted(Action action)
        {
            if ((_context != null) && (_executor != null))
            {
                _context.Post(_executor, action);
            }
        }

        public void GetResult()
        {
        }

        #endregion
    }
}
