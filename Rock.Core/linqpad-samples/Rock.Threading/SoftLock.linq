<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
    // The SoftLock class works a little like a true lock, except when the lock
    // is acquired, it doesn't cause other threads to block. Rather the other
    // threads *skip* the protected block.
    
    // This demo starts a specified number of threads. Each thread has an infinite
    // loop, and in this loop the thread tries to acquire the soft lock. If it
    // succeeds, it wait a little while then releases the soft lock and exits.
    // If a thread does not acquire the soft lock, then it pauses before trying
    // again.

    RunThreads(10);
}

// This SoftLock object is shared by all threads for lock-free synchronization.
private readonly SoftLock _softLock = new SoftLock();

private void ThreadStartMethod(object obj)
{
    int threadId = (int)obj;

    while (true)
    {
        // Try to acquire the lock. Only one thread at a time can acquire the
        // lock and enter the protected block.
        if (_softLock.TryAcquire())
        {
            Console.WriteLine("Acquired lock: {0} <------------", threadId);
        
            // Hold the lock for 1.5 seconds to allow the losing threads to
            // print their message and loop around.
            Thread.Sleep(1500);
            
            Console.WriteLine("Releasing lock: {0} <------------", threadId);
            
            // Release the lock so another thread can enter the protected block.
            _softLock.Release();
            
            // Exit the thread when finished (don't loop around again).
            break;
        }
        
        Console.WriteLine("Lock not acquired: {0}", threadId);
        
        // We were unsuccessful in acuiring the lock. Wait 0.5 seconds before
        // trying again. The waiting is for demo purposes - we don't want a
        // flood of "looping" messages to drown out the "finished" messages.
        Thread.Sleep(500);
    }
}

private void RunThreads(int threadCount)
{
    var items = Enumerable.Range(0, threadCount).Select(i => new { ThreadId = i, Thread = new Thread(ThreadStartMethod) }).ToList();
    
    Parallel.ForEach(items, item =>
    {
        item.Thread.Start(item.ThreadId);
    });
    
    foreach (var item in items)
    {
        item.Thread.Join();
    }
}