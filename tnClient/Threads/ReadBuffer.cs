using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Ext.System;

namespace tnClient.Threads
{
  // ------------------------------ ReadBuffer ----------------------------
  public class ReadBuffer : IDisposable
  {
    StringBuilder mBuf = null;
    ThreadSupervisor mSupervisor;

    // event signaled when read text is placed in the buffer.
    ExtendedManualResetEvent mGotDataEvent;

    public ReadBuffer()
    {
      mGotDataEvent = new ExtendedManualResetEvent(false);
    }

    public StringBuilder Buffer
    {
      get
      {
        if (mBuf == null)
          mBuf = new StringBuilder();
        return mBuf;
      }
    }

    /// <summary>
    /// ReadBuffer no longer being used. Make sure it is removed from list of
    /// ReadBuffer(s) maintained by the ThreadSupervisor.
    /// </summary>
    public void Dispose()
    {
      if (Supervisor != null)
        Supervisor.DetachReadBuffer(this);
    }

    public ExtendedManualResetEvent GotDataEvent
    {
      get
      {
        ThrowNoSupervisor();
        return mGotDataEvent;
      }
    }

    public object LockFlag
    {
      get
      {
        ThrowNoSupervisor();
        return Supervisor.LockFlag;
      }
    }


    /// <summary>
    /// Wait and read whatever is in the ReadBuffer.
    /// </summary>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public string[] Read()
    {
      StringBuilder accum = new StringBuilder();

      while (true)
      {
        // extract whatever text there is in the ReadBuffer.  
        // ( the ReceiveThread appends to this buffer as data arrives from the
        //   telnet server. )
        lock (LockFlag)
        {
          accum.Append(Buffer.ToString());
          Buffer.Length = 0;
          GotDataEvent.Reset();
        }

        // leave when there is something there.
        if (accum.Length > 0)
          break;

        // wait for more data to arrive and be placed in the read buffer.
        GotDataEvent.WaitOne();
      }

      string[] lines = TelnetCore.SplitReadText(accum.ToString());
      return lines;
    }


    /// <summary>
    /// Wait and read ReadBuffer contents until the read text ends with
    /// the pattern.
    /// </summary>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public string[] ReadUntilEndsWith(string[] InAnyPattern)
    {
      StringBuilder accum = new StringBuilder();

      while (true)
      {
        // extract whatever text there is in the ReadBuffer.  
        // ( the ReceiveThread appends to this buffer as data arrives from the
        //   telnet server. )
        lock (LockFlag)
        {
          accum.Append(Buffer.ToString());
          Buffer.Length = 0;
          GotDataEvent.Reset();
        }

        // check for the "ends with" pattern.
        if (accum.ToString().TrimEnd().EndsWithAny(InAnyPattern) == true)
          break;

        // wait for more data to arrive and be placed in the read buffer.
        GotDataEvent.WaitOne();
      }

      string[] lines = TelnetCore.SplitReadText(accum.ToString());
      return lines;
    }

    public ThreadSupervisor Supervisor
    {
      get { return mSupervisor; }
      set { mSupervisor = value; }
    }

    public void ThrowNoSupervisor()
    {
      if (mSupervisor == null)
        throw new Exception("ReadBuffer is not assigned to a thread supervisor");
    }
  }

}
