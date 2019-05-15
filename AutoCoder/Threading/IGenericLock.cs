using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Threading
{
  public interface IGenericLock
  {
    bool Lock();
    int Release();
  }

  public static class IGenericLockExt
  {

    /// <summary>
    /// Attempt to lock all the locks. If all are not available, release those
    /// locks that were locked and return false.
    /// </summary>
    /// <param name="Locks"></param>
    /// <returns></returns>
    public static bool LockAll(this IGenericLock[] Locks)
    {
      int lastLockedIx = -1;
      bool gotAllLocks = true;
      for (int ix = 0; ix < Locks.Length; ++ix)
      {
        var item = Locks[ix];
        if (item != null)
        {
          bool rc = item.Lock();
          if (rc == true)
            lastLockedIx = ix;
          else
          {
            gotAllLocks = false;
            break;
          }
        }
      }

      // got all the locks.
      if (gotAllLocks == true)
        return true;
      else
      {
        for (int ix = 0; ix < lastLockedIx; ++ix)
        {
          var item = Locks[ix];
          if (item != null)
          {
            item.Release();
          }
        }

        return false;
      }
    }

    public static void ReleaseAll(this IGenericLock[] Locks)
    {
      foreach (var item in Locks)
      {
        if (item != null)
        {
          item.Release();
        }
      }
    }
  }
}
