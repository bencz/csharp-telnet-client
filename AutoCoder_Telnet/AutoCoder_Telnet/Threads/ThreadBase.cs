using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Threads
{
  public abstract class ThreadBase
  {
    // event signaled by the main thread that tells the background threads to end.
    public ExtendedManualResetEvent ShutdownFlag
    { get; private set; }

    // event that is set when the thread exits the EntryPoint method.
    public ExtendedManualResetEvent ThreadEndedEvent
    { get; private set; }
    public TelnetLogList LogList
    { get; set; }

    public ThreadBase(ExtendedManualResetEvent ShutdownFlag)
    {
      this.ShutdownFlag = ShutdownFlag;

      // the thread ended event starts off as on. Not until it is running is the
      // ended event set off.
      this.ThreadEndedEvent = new ExtendedManualResetEvent(true);
    }
  }
}
