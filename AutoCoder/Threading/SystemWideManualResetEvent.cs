using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  /// <summary>
  /// encapsulates a named manul reset event. 
  /// Includes a method that tests if the event is set or not by attempting to 
  /// wait for the event with a zero timeout.
  /// </summary>
  public class SystemWideManualResetEvent : IDisposable
  {
    EventWaitHandle mEvent;

    public SystemWideManualResetEvent(bool InitialState, string EventName)
    {
      mEvent = new EventWaitHandle(InitialState, EventResetMode.ManualReset, EventName ) ;
    }

    public void Dispose()
    {
    }

    public bool TestIsSet()
    {
      bool isSet = false;
        isSet = mEvent.WaitOne(0) ;
      return isSet ;
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
