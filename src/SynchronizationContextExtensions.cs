using System;
using System.Threading;

namespace SerializingSynchronizationContextDemo
{
    public static class SynchronizationContextExtensions
    {
        #region Extensions

        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext @this)
        {
            return new SynchronizationContextAwaiter(@this);
        }

        public static void Invoke(this SynchronizationContext @this, Action callback)
        {
            @this.Send(_ => callback(), null);
        }

        public static T Invoke<T>(this SynchronizationContext @this, Func<T> callback)
        {
            T result = default(T);
            @this.Send(_ => result = callback(), null);

            return result;
        }

        #endregion
    }
}
