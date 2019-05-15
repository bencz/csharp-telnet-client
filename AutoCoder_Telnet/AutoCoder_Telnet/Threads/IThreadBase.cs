using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Threads
{
  public interface IThreadBase
  {
    // event signaled by the main thread that tells the background threads to end.
    ExtendedManualResetEvent ShutdownFlag
    { get; }

    // event that is set when the thread exits the EntryPoint method.
    ExtendedManualResetEvent ThreadEndedEvent
    { get; }
    TelnetLogList LogList
    { get; }

    /// <summary>
    /// message queue thru which messages are sent to the thread.
    /// </summary>
    ConcurrentMessageQueue InputQueue
    { get; }

    /// <summary>
    /// entry point of the thread. method called when thread is started.
    /// </summary>
    void EntryPoint();
  }

  public static class IThreadBaseExt
  {
    public static void PostInputMessage(
      this IThreadBase ThreadBase, ThreadMessageCode MessageCode)
    {
      var message = new GeneralThreadMessage(MessageCode);
      ThreadBase.PostInputMessage(message);
    }
    public static void PostInputMessage(
      this IThreadBase ThreadBase, ThreadMessageCode MessageCode, 
      ScreenContent ScreenContent)
    {
      var message = new GeneralThreadMessage(MessageCode, ScreenContent);
      ThreadBase.PostInputMessage(message);
    }

    public static void PostInputMessage(
      this IThreadBase ThreadBase, ThreadMessageBase Message)
    {
      ThreadBase.InputQueue.Enqueue(Message);
    }
  }
}

