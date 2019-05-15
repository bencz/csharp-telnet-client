using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// Helper class which waits for a Mutex in the constructor, and then releases
  /// the Mutex in the Dispose method.
  /// Intended to be used within a "using" block. 
  /// </summary>
  public class MutexWaitOne : IDisposable
  {
    Mutex mMutex = null;
    bool mGotLock = false;

    public MutexWaitOne(Mutex InMutex)
    {
      mMutex = InMutex;
      mGotLock = mMutex.WaitOne();
    }

    public void Dispose()
    {
      if (mGotLock == true)
      {
        mMutex.ReleaseMutex();
        mGotLock = false;
      }
    }

  }
}
