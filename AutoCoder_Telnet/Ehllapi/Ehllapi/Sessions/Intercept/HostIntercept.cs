using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Sessions.CloseIntercept;
using AutoCoder.Ehllapi;

#if skip

namespace Ehllapi.Intercept
{
  public static class HostIntercept
  {
    public static void StartHostIntercept(this CloseInterceptItem SessionItem, IntPtr hWnd)
    {
      if (SessionItem.HostInterceptIsStarted == true)
        throw new ApplicationException("Host notification is already started.");

      var taskId = Ehllapier.StartHostNotification(SessionItem.SessId, hWnd);
      if (taskId != 0)
      {
        SessionItem.HostTaskId = taskId;
        SessionItem.HostInterceptIsStarted = true;
      }
    }

    public static void StopHostNotification(this CloseInterceptItem SessionItem)
    {
      if (SessionItem.HostInterceptIsStarted == false)
        throw new ApplicationException("Host notification is not started");

      Ehllapier.StopHostNotification(SessionItem.SessId);

      SessionItem.HostTaskId = 0;
      SessionItem.HostInterceptIsStarted = false;
    }
  }
}

#endif
