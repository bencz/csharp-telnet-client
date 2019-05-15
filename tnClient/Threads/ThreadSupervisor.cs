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
  /// <summary>
  /// ThreadSupervisor is a place to manage both the CommandThread(s) which 
  /// are currently active and the ReadBuffer associated with those
  /// command threads.
  /// </summary>
  public class ThreadSupervisor
  {
    ReadBuffer mCurrentReadBuffer;
    List<ReadBuffer> mDisplacedBufferList;
    object mLockFlag = new object();
    ExtendedManualResetEvent mBackThreadEndFlag = null;

    // list of currently running threads. Use this list to shutdown all threads
    // when the TelnetConnection to the server is ended.
    List<CommandThread> mCommandThreadList;

    // the currently running ReceiveThread.
    ReceiveThread mRunningReceiveThread;

    RunLogListBox mRunLog;

    public ThreadSupervisor(
      ExtendedManualResetEvent InBackThreadEndFlag, RunLogListBox InRunLog)
    {
      mRunLog = new RunLogListBox(InRunLog, "Supervisor");
      mBackThreadEndFlag = InBackThreadEndFlag;
      mCurrentReadBuffer = null;
      mDisplacedBufferList = new List<ReadBuffer>();
      mCommandThreadList = new List<CommandThread>();
    }

    public void AddToCommandThreadList(CommandThread InCmdThread)
    {
      mCommandThreadList.Add(InCmdThread);
    }

    public void AssignCurrentReadBuffer(ReadBuffer InBuf)
    {
      lock (LockFlag)
      {
        if (mCurrentReadBuffer != null)
        {
          mDisplacedBufferList.Add(mCurrentReadBuffer);
        }
        mCurrentReadBuffer = InBuf;
        InBuf.Supervisor = this;
      }
    }

    public void AssignRunningReceiveThread(ReceiveThread InThread)
    {
      if (mRunningReceiveThread != null)
        throw new Exception("ReceiveThread is already assigned as running");
      mRunningReceiveThread = InThread;
    }

    public void AssureReceiveThread(
      TelnetConnection InConn, RunLogListBox InRunLog)
    {
      if (RunningReceiveThread == null)
      {
        AutoResetEvent startReceivingEvent;

        // start the receive from telnet server thread.
        ThreadCommon.StartReceiveFromTelnetServerThread(
          //          out receiveQueue, 
          out startReceivingEvent, InConn,
          this,
          mBackThreadEndFlag, InRunLog);
      }
    }

    public ReadBuffer CurrentReadBuffer
    {
      get
      {
        lock (LockFlag)
        {
          return mCurrentReadBuffer;
        }
      }
      set
      {
        lock (LockFlag)
        {
          mCurrentReadBuffer = value;
        }
      }
    }

    public void DetachReadBuffer(ReadBuffer InBuf)
    {
      lock (LockFlag)
      {
        // unassign as the current ReadBuffer.
        if (CurrentReadBuffer == InBuf)
          UnassignCurrentReadBuffer();

        // remove from the displaced buffer list.
        if (mDisplacedBufferList.Contains(InBuf) == true)
        {
          mDisplacedBufferList.Remove(InBuf);
        }
      }
    }

    public object LockFlag
    {
      get { return mLockFlag; }
    }

    public void RemoveFromCommandThreadList(CommandThread InCmdThread)
    {
      int ix = mCommandThreadList.IndexOf(InCmdThread);
      if (ix != -1)
      {
        mCommandThreadList.RemoveAt(ix);
      }
    }

    public ReceiveThread RunningReceiveThread
    {
      get { return mRunningReceiveThread; }
    }

    public void ShutdownThreads()
    {
      foreach (CommandThread ct in mCommandThreadList)
      {
        ct.InitiateThreadShutdown();

        // wait for the command thread to end.
        ct.ThreadEndedEvent.WaitOne();
      }

      // instruct the receive thread to shutdown.
      ReceiveThread rrt = RunningReceiveThread;
      if ((rrt != null) && (rrt.IsRunning == true))
      {
        rrt.InitiateThreadShutdown();
        mRunLog.Write("Wait for ReceiveThread to end ...");
        rrt.ThreadEndedEvent.WaitOne();
      }
    }

    public void UnassignCurrentReadBuffer()
    {
      if (mCurrentReadBuffer == null)
        throw new Exception("ReadBuffer is not assigned");
      mCurrentReadBuffer = null;

      if (mDisplacedBufferList.Count > 0)
      {
        int lastIx = mDisplacedBufferList.Count - 1;
        mCurrentReadBuffer = mDisplacedBufferList[lastIx];
        mDisplacedBufferList.RemoveAt(lastIx);
      }
    }

    public void UnassignRunningReceiveThread(ReceiveThread InThread)
    {
      if (mRunningReceiveThread != InThread)
        throw new Exception("Receive thread to unassign is not assigned as running");
      mRunningReceiveThread = null;
    }
  }

}
