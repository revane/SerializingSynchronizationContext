using System;
using System.Threading;
using System.Threading.Tasks;

namespace SerializingSynchronizationContextDemo
{
    // This class demonstrates that SerializingSynchronizationContext does not
    // act as a monitor. Although the context is activated in UpdateStateSlowlyAsync(),
    // the lock is released when the await is reached thus allowing other users of the
    // class access to state. In this case, during the delay, the driver function sets
    // the state to a new value.
    //
    // With SerializingSynchronizationContext, one can expect class state between calls to
    // await to remain consistent. However, over an await, state cannot be relied upon to
    // stay unchanged.
    class ContextDemo
    {
        private SerializingSynchronizationContext _context = new SerializingSynchronizationContext();
        private int _sharedState;

        public async Task UpdateStateSlowlyAsync()
        {
            await _context;

            Console.WriteLine("Capturing _sharedState {0} thread {1}", _sharedState, Thread.CurrentThread.ManagedThreadId);
            _sharedState = await NewValueAfterDelayAsync(_sharedState);
            Console.WriteLine("New _sharedState value {0} thread {1}", _sharedState, Thread.CurrentThread.ManagedThreadId);
        }

        private async Task<int> NewValueAfterDelayAsync(int sharedState)
        {
            await _context;

            await Task.Delay(2000);
            Console.WriteLine("Delay complete thread {0}", Thread.CurrentThread.ManagedThreadId);
            return sharedState + 1;
        }

        public int SharedState
        {
            get { return _sharedState; }
            set
            {
                Console.WriteLine("Setting SharedState {0} thread {1}", value, Thread.CurrentThread.ManagedThreadId);
                _context.Invoke(() => { _sharedState = value; });
            }
        }
    }
}