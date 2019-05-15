using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// class that encapsulates the locking of an integer using the
  /// Interlocked.CompareExchange method.
  /// Call Lock to obtain the lock. Call Release to release the lock. 
  /// </summary>
  public class ReadLock : IGenericLock
  {
    // the actual integer variable which will be set to 0 or 1 using the
    // Interlocked.CompareExchange method. 
    int _ReadLockFlag;

    // the depth of the lock. that is, the number of times lock has been called
    // by the thread with the lock. Have to call unlock an equal number of times
    // to actually unlock the flag.
    int _LockDepth;

    public ReadLock()
    {
      _ReadLockFlag = 0;
      _LockDepth = 0;
    }

    public bool YieldUntilLock()
    {
      bool gotLock = false;
      while (true)
      {
        gotLock = Lock();
        if (gotLock == true)
          break;
        Thread.Yield();
      }
      return gotLock;
    }

    public bool Lock()
    {
      bool gotLock = false;
      int threadId = Thread.CurrentThread.ManagedThreadId;

      // if the lock flag contains the current thread threadId, then this thread
      // already has the lock.
      // Inc the lock depth. To unlock the flag, have to call Unlock just as many
      // times as Lock.
      if (_ReadLockFlag == threadId)
      {
        _LockDepth += 1;
        gotLock = true;
      }

      else
      {
        // attempt to set the LockFlag to the current thread threadId. 
        int wasValue = Interlocked.CompareExchange(ref _ReadLockFlag, threadId, 0);
        if (wasValue == 0)
        {
          gotLock = true;
          _LockDepth += 1;
        }
      }
      return gotLock;
    }

    public int Release()
    {
      if (_LockDepth > 0)
        _LockDepth -= 1;
      else
        throw new ApplicationException("releasing a lock the thread does not have");

      if (_LockDepth == 0)
      {
        _ReadLockFlag = 0;
      }

      return _LockDepth;
    }
  }
}
