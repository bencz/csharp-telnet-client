using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api.Structs;
using AutoCoder.Ehllapi.Api;
using Ehllapi.Messages;
using AutoCoder.Ehllapi.Sessions.Master;

namespace AutoCoder.Ehllapi.Sessions.HostNotification
{
  /// <summary>
  /// Capture session item.
  /// </summary>
  public class HostNotificationItem
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

    /// <summary>
    /// the TaskId returned by ehllapi when the StartNotification is run. 
    /// </summary>
    public int TaskId
    { get; set; }

    public string ErrorText
    { get; set; }

    public bool NotificationIsStarted
    { get; set; }

    public HostNotificationItem()
    {
      this.MasterItem = null;
      this.NotificationIsStarted = false;
      this.TaskId = 0;
      this.ErrorText = null;
    }

    public HostNotificationItem(MasterSessionItem MasterItem)
      : this( ) 
    {
      this.MasterItem = MasterItem;
    }

    public void StartNotification(IntPtr hRequestWnd)
    {
      try
      {
        var taskId = Ehllapier.StartHostNotification(this.SessId, hRequestWnd);
        this.NotificationIsStarted = true;
        this.ErrorText = null;
        this.TaskId = taskId;
      }
      catch (ApplicationException Excp)
      {
        this.NotificationIsStarted = false;
        this.ErrorText = Excp.Message;
      }
    }

    public SessIdMessage StopNotification()
    {
      SessIdMessage msg = null;
      if (this.NotificationIsStarted == true)
      {
        string errmsg = null;
        try
        {
          Ehllapier.StopHostNotification(this.SessId);
        }
        catch (ApplicationException Excp)
        {
          errmsg = Excp.Message;
        }
        if (errmsg == null)
        {
          msg = new SessIdMessage()
          {
            SessId = this.SessId,
            Message = "Host notification is stopped."
          };
          this.NotificationIsStarted = false;
        }

        else
        {
          msg = new SessIdMessage()
          {
            SessId = this.SessId,
            Message = "StopCapture failed. " + errmsg 
          };
        }
      }

      return msg;
    }

  }
}
