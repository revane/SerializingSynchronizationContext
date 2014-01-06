using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerializingSynchronizationContextDemo
{
    class Class1
    {
        private SerializingSynchronizationContext _context = new SerializingSynchronizationContext();
        private int _sharedState;

        public async Task DoWorkAsync()
        {
            await _context;

            Console.WriteLine("DoWorkAsync Pre: Thread {0}", Thread.CurrentThread.ManagedThreadId);
            _sharedState = await NextValueAsync(_sharedState);
            Console.WriteLine("DoWorkAsync Post: Thread {0}", Thread.CurrentThread.ManagedThreadId);
        }

        private async Task<int> NextValueAsync(int sharedState)
        {
            await _context;
            await Task.Delay(2000);
            Console.WriteLine("NextValueAsync: Thread {0}", Thread.CurrentThread.ManagedThreadId);
            return sharedState + 1;
        }

        public int SharedState
        {
            get { return _sharedState; }
            set
            {
                Console.WriteLine("Set SharedState: Thread {0}", Thread.CurrentThread.ManagedThreadId);
                _context.Invoke(() => { _sharedState = value; });
            }
        }
    }

    class Program
    {
        static async Task MainAsync()
        {
            Class1 c = new Class1();
            Task t = c.DoWorkAsync();
            c.SharedState = 9;
            await t;
            Console.WriteLine("Result: {0}", c.SharedState);
        }

        static void Main(string[] args)
        {
            MainAsync().Wait();
        }
    }
}
