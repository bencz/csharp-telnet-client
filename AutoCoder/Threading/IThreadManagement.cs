using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Threading
{
  public interface IThreadManagement
  {
    /// <summary>
    /// ThreadEndedEvent is signaled when the thread exits its EntryPoint method.
    /// </summary>
    ExtendedManualResetEvent ThreadEndedEvent
    {
      get;
    }

  }
}
