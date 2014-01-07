using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SerializingSynchronizationContextDemo
{
    // This example is identical to DeadlockTest except it uses SerializingSynchronizationContext
    // instead of AsyncLock. Here there is no deadlock because of different semantics: the lock
    // provided by the context is released when an await is reached and re-established for the
    // continuation. Essentially avoids the Hold-and-Wait Coffman condition.
    class ContextTest
    {
        SerializingSynchronizationContext _context = new SerializingSynchronizationContext();

        public async Task Entry()
        {
            Console.WriteLine("In Entry() {0}", Thread.CurrentThread.ManagedThreadId);

            await _context;
            ContextTestHelper helper = new ContextTestHelper();
            helper.DoWork += DoWork;
            Console.WriteLine("Calling PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
            await helper.PerformCallback();
            Console.WriteLine("Returned from PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
        }

        async Task DoWork()
        {
            Console.WriteLine("In DoWork() {0}", Thread.CurrentThread.ManagedThreadId);
            await _context;

            Console.WriteLine("Delay... {0}", Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(1000);
        }
    }

    class ContextTestHelper
    {
        SerializingSynchronizationContext _context = new SerializingSynchronizationContext();

        public delegate Task DoWorkDelegate();
        public event DoWorkDelegate DoWork;

        public async Task PerformCallback()
        {
            Console.WriteLine("In PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
            await _context;

            // Ensure a different thread calls the delegate.
            await Task.Delay(100);
            Console.WriteLine("Calling delegate {0}", Thread.CurrentThread.ManagedThreadId);
            await DoWork();
            Console.WriteLine("Returned from delegate {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}