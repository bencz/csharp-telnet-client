using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// Creates and lock the mutex in the constructor. Releases the lock in the Dispose
  /// method.
  /// </summary>
  public class DisposableMutex : IDisposable
  {
    Mutex _Mutex = null;
    bool _GotLock = false;

    public DisposableMutex(string MutexName)
    {
      bool createdNew;
      _Mutex = new Mutex(false, MutexName, out createdNew);
      _GotLock = _Mutex.WaitOne();
    }

    public void Dispose()
    {
      if (_GotLock == true)
      {
        _Mutex.ReleaseMutex();
        _GotLock = false;
      }
    }

  }
}

