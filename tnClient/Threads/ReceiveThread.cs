using AutoCoder.Telnet.Common;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tnClient.Runtime;

namespace tnClient.Threads
{
  public class ReceiveThread : IDisposable, IThreadManagement
  {
    ThreadSupervisor mSupervisor;

    // connection to the telnet server.
    TelnetConnection mConn;

    // receive thread waits on this event to know that it should start receiving.
    AutoResetEvent mStartReceivingEvent;

    // event signaled by the main thread that tells the background threads to end.
    ExtendedManualResetEvent mShutdownFlag;

    // signal that is set when the thread exits the EntryPoint method.
    ExtendedManualResetEvent mThreadEndedEvent = new ExtendedManualResetEvent(false);

    RunLogListBox mRunLog;

    public ReceiveThread(
      TelnetConnection InConn,
      AutoResetEvent InStartReceivingEvent,
      ThreadSupervisor InSupervisor,
      ExtendedManualResetEvent InShutdownFlag,
      RunLogListBox InRunLog)
    {
      mConn = InConn;

      mSupervisor = InSupervisor;
      mSupervisor.AssignRunningReceiveThread(this);

      mStartReceivingEvent = InStartReceivingEvent;
      mRunLog = new RunLogListBox(InRunLog, "ReceiveThread");
      mShutdownFlag = InShutdownFlag;
    }

    public void Dispose()
    {
      if (mSupervisor.RunningReceiveThread == this)
        mSupervisor.UnassignRunningReceiveThread(this);
    }

    // The ReceiveThread is relatively simple. 
    // It continuously reads from the socket connected to the telnet server. Whatever
    // is received is appended to a Read buffer and an event is signaled to alert
    // CommandThreads to come and get the data.  The CommandThread which gets the data
    // then empties the ReadBuffer and waits again on the "DataArrived" event to be
    // signaled by this thread.
    public void EntryPoint()
    {
      try
      {
        mRunLog.Write("Start reading from telnet server");

        while (mShutdownFlag.State == false)
        {
          if (mConn.IsConnected == false)
            break;

          string s1 = mConn.Read();

          // append what was just read to the ReadBuffer.
          lock (mSupervisor.LockFlag)
          {
            if (mSupervisor.CurrentReadBuffer != null)
            {
              mSupervisor.CurrentReadBuffer.Buffer.Append(s1);
            }

            // Signal command threads that there is read data available. ( The command
            // thread then resets the event after it checks/removes from the data buffer. )
            mSupervisor.CurrentReadBuffer.GotDataEvent.Set();
          }
        }

        mRunLog.Write("Exit.");
      }
      finally
      {

        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();

      }
    }

    /// <summary>
    /// instruct the ReceiveThread to shutdown by disconnecting the socket it is using
    /// to talk to the telnet server.  By disconnecting the socket the current read from
    /// the socket that this thread is blocked on will unblock and return zero bytes. 
    /// </summary>
    public void InitiateThreadShutdown()
    {
      if (ThreadEndedEvent.State == false)
      {
        if (mConn != null)
          mConn.Disconnect();
      }
    }

    /// <summary>
    /// flag indicating if the thread is running.
    /// </summary>
    public bool IsRunning
    {
      get
      {
        return !ThreadEndedEvent.State;
      }
    }

    /// <summary>
    /// event signaled when the thread exits its EntryPoint method.
    /// </summary>
    public ExtendedManualResetEvent ThreadEndedEvent
    {
      get { return mThreadEndedEvent; }
    }

    // ------------------------- ThreadStart -----------------------------
    public static void ThreadStart(ReceiveThread InThread)
    {
      ThreadStart job = new ThreadStart(InThread.EntryPoint);
      Thread t = new Thread(job);
      t.IsBackground = true;
      t.Start();
    }

  }

}
