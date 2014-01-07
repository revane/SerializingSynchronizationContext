using System;
using System.Threading;
using System.Threading.Tasks;

namespace SerializingSynchronizationContextDemo
{
    class Program
    {
        static async Task DeadLockTest()
        {
            DeadlockTest test = new DeadlockTest();
            await test.Entry();
            Console.WriteLine("Test complete {0}", Thread.CurrentThread.ManagedThreadId);
        }

        static async Task ContextTest()
        {
            ContextTest test = new ContextTest();
            await test.Entry();
            Console.WriteLine("Test complete {0}", Thread.CurrentThread.ManagedThreadId);
        }

        static async Task ContextDemo()
        {
            ContextDemo demo = new ContextDemo();
            Task t = demo.UpdateStateSlowlyAsync();
            demo.SharedState = 9;
            await t;
            Console.WriteLine("Final state: {0}", demo.SharedState);
        }

        static void Main(string[] args)
        {
            ContextDemo().Wait();
        }
    }
}
