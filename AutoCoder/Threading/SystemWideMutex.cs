using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// encapsulates a named mutex. Includes a method that tests if the mutex is
  /// set or not by attempting to lock and release it. 
  /// ( Mutex is sealed, so it cannot be a base class. )
  /// </summary>
  public class SystemWideMutex : IDisposable
  {
    Mutex mMutex;
    private int _lockCx = 0 ;

    public SystemWideMutex(string MutexName)
    {
      mMutex = new Mutex(false, MutexName);
    }

    public void Dispose()
    {
      while (_lockCx > 0)
      {
        ReleaseMutex();
      }
    }

    public void ReleaseMutex()
    {
      mMutex.ReleaseMutex();
      if (_lockCx > 0)
      {
        int remLockCx = Interlocked.Decrement(ref _lockCx);
      }
    }

    public bool TestIsSet()
    {
      bool gotLock = false;
      try
      {
        gotLock = mMutex.WaitOne(0);
      }
      finally
      {
        if (gotLock == true)
        {
          mMutex.ReleaseMutex();
        }
      }
      if (gotLock == true)
        return false;
      else
        return true;
    }

    public bool WaitOne( )
    {
      bool rc = mMutex.WaitOne( );
      if (rc == true)
        Interlocked.Increment(ref _lockCx);
      return rc;
    }

    public bool WaitOne(int WaitTime)
    {
      bool rc = mMutex.WaitOne(WaitTime);
      if (rc == true)
        Interlocked.Increment(ref _lockCx);
      return rc;
    }
  }
}
