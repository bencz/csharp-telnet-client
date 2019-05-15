using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class ConcurrentMessageQueue : ConcurrentQueue<object>
  {
    /// <summary>
    /// this event signaled whenever blocks are added to the block queue.
    /// </summary>
    public AutoResetEvent GotMessageEvent
    { get; private set; }

    public ConcurrentMessageQueue( )
    {
      this.GotMessageEvent = new AutoResetEvent(false);
    }

    public new void Enqueue(object Item)
    {
      base.Enqueue(Item);
      this.GotMessageEvent.Set();
    }

    public object WaitAndDequeue( )
    {
      object item = null;
      while (true)
      {
        var rc = this.TryDequeue(out item);
        if (rc == true)
          break;
        this.GotMessageEvent.WaitOne();
      }

      return item;
    }

    public object WaitAndDequeue( WaitHandle OtherWaitHandle, WaitHandle WaitHandle2 = null )
    {
      object item = null;
      while(true)
      {
        var rc = this.TryDequeue(out item);
        if (rc == true)
          break;

        WaitHandle[] handles = null;
        {
          var listHandles = new List<WaitHandle>();
          listHandles.Add(OtherWaitHandle);
          listHandles.Add(this.GotMessageEvent);
          if (WaitHandle2 != null)
            listHandles.Add(WaitHandle2);
          handles = listHandles.ToArray();
        }

        var ix = WaitHandle.WaitAny(handles);
        if ((ix == 0) || (ix == 2))
          break;
      }
      return item;
    }
    public object WaitAndPeek()
    {
      object item = null;
      while (true)
      {
        var rc = this.TryPeek(out item);
        if (rc == true)
          break;
        this.GotMessageEvent.WaitOne();
      }
      return item;
    }
  }
}
