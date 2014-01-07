using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SerializingSynchronizationContextDemo
{
    // This test illustrates a classic deadlock situation using AsyncLock.
    class DeadlockTest
    {
        AsyncLock _lock = new AsyncLock();

        public async Task Entry()
        {
            Console.WriteLine("In Entry() {0}", Thread.CurrentThread.ManagedThreadId);
            using (await _lock.LockAsync())
            {
                DeadlockTestHelper helper = new DeadlockTestHelper();
                helper.DoWork += DoWork;
                Console.WriteLine("Calling PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
                await helper.PerformCallback();
                Console.WriteLine("Returned from PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        async Task DoWork()
        {
            Console.WriteLine("In DoWork() {0}", Thread.CurrentThread.ManagedThreadId);
            using (await _lock.LockAsync())
            {
                Console.WriteLine("Delay... {0}", Thread.CurrentThread.ManagedThreadId);
                // Never get here. Deadlock.
                await Task.Delay(1000);
            }
        }
    }

    class DeadlockTestHelper
    {
        AsyncLock _lock = new AsyncLock();

        public delegate Task DoWorkDelegate();
        public DoWorkDelegate DoWork;

        public async Task PerformCallback()
        {
            Console.WriteLine("In PerformCallback() {0}", Thread.CurrentThread.ManagedThreadId);
            using (await _lock.LockAsync())
            {
                // Ensure a different thread calls the delegate.
                await Task.Delay(100);
                Console.WriteLine("Calling delegate {0}", Thread.CurrentThread.ManagedThreadId);
                await DoWork();
                Console.WriteLine("Returned from delegate {0}", Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}