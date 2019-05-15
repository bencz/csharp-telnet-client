using AutoCoder.Telnet.Common;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tnClient.Brand;
using tnClient.Runtime;

namespace tnClient.Threads
{
  public delegate void delThreadEnded();

  public class CommandThread : IDisposable, IThreadManagement
  {
    ExtendedManualResetEvent mShutdownFlag;
    ThreadSupervisor mSupervisor;
    TelnetConnection mConn;
    TelnetCommandRoute mCommandRoute;
    RunLogListBox mRunLog;
    delThreadEnded mThreadEndedCallback;
    string mCommandText;

    // the read buffer this command thread owns and uses. The ThreadSupervisor can 
    // reach into the CommandThread and shut it down by grabbing a reference to this
    // buffer and signaling the "GotDataEvent". The CommandThread then unblocks from 
    // its read of the buffer, sees that the shutdown flag has been set, and ends itself.
    ReadBuffer mReadBuffer;

    // method to call when this CommandThread does its actual work of running a command
    // on the Telnet server. If this method is null, then the CommandText is run.
    delRunTelnetServerCommand mCommandMethod = null;

    // signal that is set when the thread exits the EntryPoint method.
    ExtendedManualResetEvent mThreadEndedEvent = new ExtendedManualResetEvent(false);

    public CommandThread(
      ThreadSupervisor InSupervisor, TelnetConnection InConn,
      TelnetCommandRoute InCommandRoute,
      RunLogListBox InRunLog)
    {
      mShutdownFlag = new ExtendedManualResetEvent(false);
      mConn = InConn;
      mCommandRoute = InCommandRoute;
      mRunLog = InRunLog;
      Supervisor = InSupervisor;
    }

    public delRunTelnetServerCommand CommandMethod
    {
      get { return mCommandMethod; }
      set { mCommandMethod = value; }
    }

    public string CommandText
    {
      get { return mCommandText; }
      set { mCommandText = value; }
    }

    public TelnetCommandRoute CommandRoute
    {
      get { return mCommandRoute; }
    }

    public TelnetConnection Conn
    {
      get { return mConn; }
    }

    public void Dispose()
    {
      if (mSupervisor != null)
      {
        Supervisor.RemoveFromCommandThreadList(this);
      }
    }

    public virtual void EntryPoint()
    {
      try
      {
        using (this)
        {
          using (ReadBuffer readBuf = new ReadBuffer())
          {
            // store the readBuf as a property so the ThreadSupervisor has access.
            ReadBuffer = readBuf;

            Supervisor.AssignCurrentReadBuffer(ReadBuffer);

            Supervisor.AssureReceiveThread(Conn, mRunLog);

            // run the actual command on the telnet server.
            string[] respLines = null;
            if (CommandMethod != null)
            {
              respLines = CommandMethod(this, ReadBuffer, WriteEventLog, CommandText);
            }
            else
            {
              respLines = this.CommandRoute.RunCommand(
                this, ReadBuffer, WriteEventLog, CommandText);
            }

            this.RunLog.Write(respLines);

            RunLog.Write(respLines);
          }
          RunLog.Write("Exit.");
        }
      }
      finally
      {

        // call the thread ended callback method.
        if (ThreadEndedCallback != null)
        {
          ThreadEndedCallback();
        }

        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();
      }
    }

    /// <summary>
    /// Signal events within the thread to cause it to fall out of its work
    /// loop and end.
    /// </summary>
    public void InitiateThreadShutdown()
    {
      ShutdownFlag.Set();

      ReadBuffer.GotDataEvent.KeepSignaled = true;
      ReadBuffer.GotDataEvent.Set();
    }

    public ReadBuffer ReadBuffer
    {
      get { return mReadBuffer; }
      set { mReadBuffer = value; }
    }

    public RunLogListBox RunLog
    {
      get { return mRunLog; }
    }

    public ExtendedManualResetEvent ShutdownFlag
    {
      get { return mShutdownFlag; }
    }

    public ThreadSupervisor Supervisor
    {
      get { return mSupervisor; }
      set
      {
        mSupervisor = value;
        Supervisor.AddToCommandThreadList(this);
      }
    }

    public delThreadEnded ThreadEndedCallback
    {
      get { return mThreadEndedCallback; }
      set { mThreadEndedCallback = value; }
    }

    /// <summary>
    /// event signaled when the thread exits its EntryPoint method.
    /// </summary>
    public ExtendedManualResetEvent ThreadEndedEvent
    {
      get { return mThreadEndedEvent; }
    }

    // ------------------------- ThreadStart -----------------------------
    public static void ThreadStart(CommandThread InThread)
    {
      ThreadStart job = new ThreadStart(InThread.EntryPoint);
      Thread t = new Thread(job);
      t.IsBackground = true;
      t.Start();
    }

    public void WriteEventLog(object xx, string InText, string InActivity)
    {
      this.RunLog.Write(InText);
    }

  }

}
