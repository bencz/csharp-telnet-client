using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Ext.System.Threading
{
  public static class MutexExt
  {

    public static bool IsLocked(this Mutex Mutex)
    {
      bool isLocked = false;
      bool gotLock = Mutex.WaitOne(0);
      if (gotLock == true)
      {
        isLocked = true;
        Mutex.ReleaseMutex();
      }
      return isLocked;
    }

  }
}
