using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// encapsulates a named auto reset event. 
  /// Includes a method that tests if the event is set or not by attempting to 
  /// wait for the event with a zero timeout.
  /// </summary>
  public class SystemWideAutoResetEvent : IDisposable
  {
    EventWaitHandle mEvent;

    public SystemWideAutoResetEvent(bool InitialState, string EventName)
    {
      mEvent = new EventWaitHandle(InitialState, EventResetMode.AutoReset, EventName ) ;
    }

    public void Dispose()
    {
    }

    public bool TestIsSet()
    {
      bool isSet = false;
      isSet = mEvent.WaitOne(0);

      // the auto reset aspect of the event will clear the event after a wait
      // completes. Since this wait was done only to see if it was set, if the
      // event was set, it has just been reset and should be set on once again.
      if (isSet == true)
      {
        this.WaitHandle.Set();
      }

      return isSet;
    }

    public EventWaitHandle WaitHandle
    {
      get { return mEvent ; }
    }

    public bool WaitOne( )
    {
      bool rc = mEvent.WaitOne( );
      return rc;
    }

    public bool WaitOne(int WaitTime)
    {
      bool rc = mEvent.WaitOne(WaitTime);
      return rc;
    }
  }
}
