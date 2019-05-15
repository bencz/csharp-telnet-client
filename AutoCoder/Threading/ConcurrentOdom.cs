using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Threading
{
  /// <summary>
  /// a common counter that is thread safe.
  /// </summary>
  public class ConcurrentOdom
  {
    int Counter;

    object locker;

    public ConcurrentOdom( )
    {
      locker = new object();
      this.Counter = 0;
    }

    public int Assign( )
    {
      int odom;
      lock(locker)
      {
        this.Counter += 1;
        if (this.Counter == int.MaxValue)
          this.Counter = 1;
        odom = this.Counter;
      }
      return odom;
    }
  }
}
