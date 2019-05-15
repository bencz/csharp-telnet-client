using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoCoder.Threading
{
  // class cannot derive from ManulResetEvent. That class is sealed. 

  /// <summary>
  /// Extension of features of ManualResetEvent class.  Adds ability to query the
  /// current state of the event. Also, KeepSignaled enables the Event to always
  /// stay signaled.
  /// Another feature - StateChangedEvent. event that is signaled whenever this 
  /// event is set or reset.
  /// </summary>
  public class ExtendedManualResetEvent
  {
    /// <summary>
    /// the actual event object. 
    /// </summary>
    ManualResetEvent mEvent;

    // event signaled when the state of the this event is either set or reset.
    AutoResetEvent mStateChangedEvent;

    bool mState;

    // set this flag when the event should always be signaled. When the Reset
    // method is called, do a Set instead of Reset to make sure the event is
    // signaled.
    bool mKeepSignaled = false;

    public ExtendedManualResetEvent( bool InitalState = false )
    {
      mEvent = new ManualResetEvent(InitalState);
      mState = InitalState;
    }

    /// <summary>
    /// the current state of the event. whether it is Set or Reset.
    /// </summary>
    public bool State
    {
      get { return mState; }
      set { mState = value; }
    }


    public ManualResetEvent EventObject
    {
      get { return mEvent; }
    }

    public bool KeepSignaled
    {
      get { return mKeepSignaled; }
      set { mKeepSignaled = value; }
    }

    public void Reset()
    {
      if (KeepSignaled == true)
        Set();
      else
      {
        lock (mEvent)
        {
          mEvent.Reset();
          mState = false;

          // signal the fact that the state of the event has changed.
          if (mStateChangedEvent != null)
            mStateChangedEvent.Set();
        }
      }
    }

    public void Set()
    {
      lock (mEvent)
      {
        mEvent.Set();
        mState = true;

        // signal the fact that the state of the event has changed.
        if (mStateChangedEvent != null)
          mStateChangedEvent.Set();
      }
    }

    /// <summary>
    /// Event that will be signaled when this event is either set or reset.
    /// </summary>
    public AutoResetEvent StateChangedEvent
    {
      get
      {
        if (mStateChangedEvent == null)
          mStateChangedEvent = new AutoResetEvent(false);
        return mStateChangedEvent;
      }
    }

    public bool WaitOne(int InTimeout)
    {
      bool rc = mEvent.WaitOne(InTimeout);
      return rc;
    }

    public bool WaitOne( )
    {
      bool rc = mEvent.WaitOne( );
      return rc;
    }

  }
}
