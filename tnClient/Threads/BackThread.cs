using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tnClient.Threads
{

  public class BackThread
  {

    // event signaled by the main thread that tells the background threads to end.
    ExtendedManualResetEvent ShutdownFlag;

    // signal that is set when the thread exits the EntryPoint method.
    ExtendedManualResetEvent ThreadEndedEvent = new ExtendedManualResetEvent(false);

    public void EntryPoint()
    {
      try
      {
        while (ShutdownFlag.State == false)
        {

        }
      }
      finally
      {

        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();

      }
    }

    public BackThread(
      ExtendedManualResetEvent ShutdownFlag)
    {
      this.ShutdownFlag = ShutdownFlag;
    }


  }
}
