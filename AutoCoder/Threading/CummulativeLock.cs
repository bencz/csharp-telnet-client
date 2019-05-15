using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Threading
{
  /// <summary>
  /// class with accumulates locks. When the object is disposed, release all the
  /// accumulated locks. 
  /// When a Lock returns false ( did not get the lock ), the caller has the option
  /// of closing the object, which releases all the accumulated locks.
  /// </summary>
  public class CummulativeLock : IDisposable
  {

    public CummulativeLock( )
    {
      this.GotAllLocks = true;
    }

    #region properties
    public List<IGenericLock> AccumulatedLocks
    {
      get
      {
        if (_AccumulatedLocks == null)
          _AccumulatedLocks = new List<IGenericLock>();
        return _AccumulatedLocks;
      }
    }
    List<IGenericLock> _AccumulatedLocks;

    bool _GotAllLocks;
    public bool GotAllLocks
    {
      get { return _GotAllLocks; }
      private set { _GotAllLocks = value; }
    }
    
    #endregion

    /// <summary>
    /// Attempt to lock the specified lock.
    /// If the lock is acquired, add the lock to the accumulated list of locks.
    /// Return the gotLock result. 
    /// </summary>
    /// <param name="LockToLock"></param>
    /// <returns></returns>
    public bool Lock(IGenericLock LockToLock)
    {
      bool gotLock = LockToLock.Lock();
      if (gotLock == true)
      {
        AccumulatedLocks.Add(LockToLock);
      }
      else
      {
        this.GotAllLocks = false;
      }
      return gotLock;
    }

    public int ReleaseAll()
    {
      int releaseCount = 0;
      if (this.AccumulatedLocks != null)
      {
        foreach (var item in this.AccumulatedLocks)
        {
          item.Release();
          releaseCount += 1;
        }
      }
      return releaseCount;
    }

    public void Dispose()
    {
      ReleaseAll();
    }
  }
}
