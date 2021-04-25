using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Helper class for running asynchronous methods synchronously without deadlocking the current
    /// thread. Should be used when VISABApi should be used in non async methods.
    /// </summary>
    public static class AsyncHelper
    {
        public static TReturn RunSynchronously<TReturn>(Func<Task<TReturn>> asyncMethod)
        {
            return Task.Run(async () => await asyncMethod.Invoke()).Result;
        }

        public static TReturn RunSynchronously<TReturn, T1>(Func<T1, Task<TReturn>> asyncMethod, T1 value)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value)).Result;
        }

        public static TReturn RunSynchronously<TReturn, T1, T2>(Func<T1, T2, Task<TReturn>> asyncMethod, T1 value1, T2 value2)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value1, value2)).Result;
        }

        public static TReturn RunSynchronously<TReturn, T1, T2, T3>(Func<T1, T2, T3, Task<TReturn>> asyncMethod, T1 value1, T2 value2, T3 value3)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value1, value2, value3)).Result;
        }
    }
}