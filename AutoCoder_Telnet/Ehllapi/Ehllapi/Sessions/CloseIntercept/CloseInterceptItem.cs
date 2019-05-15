using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api.Structs;
using AutoCoder.Ehllapi.Api;
using Ehllapi.Messages;
using AutoCoder.Ehllapi.Sessions.Master;
using AutoCoder.Ehllapi.Sessions.HostNotification;

namespace AutoCoder.Ehllapi.Sessions.CloseIntercept
{
  /// <summary>
  /// CloseIntercept session item.
  /// </summary>
  public class CloseInterceptItem
  {
    public MasterSessionItem MasterItem
    { get; private set; }

    public Guid ItemId
    {
      get { return this.MasterItem.ItemId; }
    }

    public string SessId
    {
      get
      {
        return this.MasterItem.SessId;
      }
    }

    public bool InterceptIsStarted
    { get; set; }

    /// <summary>
    /// the TaskId returned by ehllapi when the StartCloseIntercept is run. 
    /// </summary>
    public int TaskId
    { get; set; }

    public string ErrorText
    { get; set; }

    public CloseInterceptItem()
    {
      this.MasterItem = null;
      this.InterceptIsStarted = false;
      this.TaskId = 0;
      this.ErrorText = null;
    }

    public CloseInterceptItem(MasterSessionItem MasterItem)
      : this( ) 
    {
      this.MasterItem = MasterItem;
    }

    /// <summary>
    /// Session close has been intercepted.  Process the close request by making sure
    /// the session is signed off and then end the session.
    /// </summary>
    /// <returns></returns>
    public List<SessIdMessage> ProcessCloseRequest()
    {
      List<SessIdMessage> msgs = new List<SessIdMessage>();

      {
        var msg = this.StopCloseIntercept();
        if (msg != null)
          msgs.Add(msg);
      }

      // if active, stop host notification for the session.
      {
        HostNotificationList.GlobalList.StopNotification(this.MasterItem);
      }

      {
        var msg =
          AutoCoder.Ehllapi.CommonScreens.SignonScreen.AssureSignedOff(this.SessId);
        if (msg != null)
          msgs.Add(msg);
      }

      {
        var msg = pcsapi.StopSession(this.SessId);
        msgs.Add(msg);
      }

      this.InterceptIsStarted = false;
      return msgs;
    }

    public void StartCloseIntercept(IntPtr hRequestWnd)
    {
      try
      {
        var taskId = Ehllapier.StartCloseIntercept(this.SessId, hRequestWnd);
        this.InterceptIsStarted = true;
        this.ErrorText = null;
        this.TaskId = taskId;
      }
      catch (ApplicationException Excp)
      {
        this.InterceptIsStarted = false;
        this.ErrorText = Excp.Message;
      }
    }

    public SessIdMessage StopCloseIntercept()
    {
      SessIdMessage msg = null;
      if (this.InterceptIsStarted == true)
      {
        var rc = Ehllapier.StopCloseIntercept(this.SessId);
        if (rc == 0)
        {
          msg = new SessIdMessage()
          {
            SessId = this.SessId,
            Message = "Close intercept is stopped."
          };
          this.InterceptIsStarted = false;
        }

        else
        {
          msg = new SessIdMessage()
          {
            SessId = this.SessId,
            Message = "StopCloseIntercept failed. rc:" + rc
          };
        }
      }

      return msg;
    }

  }
}
