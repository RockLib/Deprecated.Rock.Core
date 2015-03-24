using System.Threading;

namespace Rock.Threading
{
    /// <summary>
    /// An object that enables exclusive access to critical sections of code. Unlike a true lock, where
    /// a thread will block while another thread has the lock, a "soft lock" will cause a thread to skip over
    /// a critical section of code if another thread has the lock.
    /// </summary>
    public class SoftLock
    {
        private int _lock;

        /// <summary>
        /// Try to acquire the lock. Returns true if the lock is acquired. Returns false if the lock has
        /// already been acquired.
        /// </summary>
        /// <returns>True, if the lock was acquired. False, if another thread currently has the lock</returns>
        public bool TryAcquire()
        {
            return Interlocked.Exchange(ref _lock, 1) == 0;
        }

        /// <summary>
        /// Release the lock. Should only be called after successfully acquiring the lock.
        /// </summary>
        public void Release()
        {
            Interlocked.Exchange(ref _lock, 0);
        }
    }
}
