using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  public class YieldUntilLock : IDisposable
  {
    IGenericLock _Lock;
    bool _GotLock = false;
    IGenericLock[] _ManyLocks;

    public YieldUntilLock(IGenericLock Lock)
    {
      _Lock = Lock;
      _ManyLocks = null;
      _GotLock = this.DoYieldUntilLock( ) ;
    }

    public YieldUntilLock(params IGenericLock[] ManyLocks)
    {
      _Lock = null;
      _ManyLocks = ManyLocks;
    }

    private bool DoYieldUntilLock()
    {
      bool gotLock = false;
      int yieldCount = 0;
      while (true)
      {
        // either get the single lock. or get all the many locks.
        {
          if (_Lock != null)
          {
            gotLock = _Lock.Lock();
          }
          else if (_ManyLocks != null)
          {
            gotLock = _ManyLocks.LockAll();
          }
          else
            throw new ApplicationException("nothing to lock");
        }

        // got the lock. return to caller.
        if (gotLock == true)
        {
          break;
        }

        // did not get the lock. Yield to other threads. Then loop back and
        // try to lock again.
        Thread.Yield();

        // too many yields.
        {
          yieldCount += 1;
          if (yieldCount > 1000000)
            throw new ApplicationException("too many yields. could be deadlock.");
        }
      }
      return gotLock;
    }

    public void Dispose()
    {
      if (_GotLock == true)
      {
        if (_Lock != null)
        {
          _Lock.Release();
        }
        if (_ManyLocks != null)
        {
          _ManyLocks.ReleaseAll();
        }
      }
    }
  }
}
