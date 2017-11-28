using System;
using System.Threading;

namespace SimpleBlockChain.Interop
{
    internal class UsageCounter
    {
        private const int MaxCount = 2147483647;
        private const int Timeout = 120000;
        private readonly Mutex _lock;
        private readonly Semaphore _count;

        public UsageCounter(string nameFormat, params object[] arguments)
        {
            string str = string.Format(nameFormat, arguments);
            this._lock = new Mutex(false, str + ".Lock");
            this._count = new Semaphore(int.MaxValue, int.MaxValue, str + ".Count");
        }

        public void Increment<T>(Action<T> beginUsage, T arg)
        {
            if (!this._lock.WaitOne(120000, false))
                throw new TimeoutException();
            try
            {
                if (!this._count.WaitOne(120000, false))
                    throw new TimeoutException();
                if (!this._count.WaitOne(120000, false))
                {
                    this._count.Release();
                    throw new TimeoutException();
                }
                int num = 1 + this._count.Release();
                if (beginUsage == null || num != 2147483646)
                    return;
                beginUsage(arg);
            }
            finally
            {
                this._lock.ReleaseMutex();
            }
        }

        public void Decrement(ThreadStart endUsage)
        {
            if (!this._lock.WaitOne(120000, false))
                throw new TimeoutException();
            try
            {
                int num = 1 + this._count.Release();
                if (endUsage == null || num != int.MaxValue)
                    return;
                endUsage();
            }
            finally
            {
                this._lock.ReleaseMutex();
            }
        }
    }
}
