using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api;
using AutoCoder.Core;
using AutoCoder.Ehllapi.Sessions.Master;

namespace AutoCoder.Ehllapi.Sessions.HostNotification
{
  /// <summary>
  /// list of HostNotification sessions.
  /// </summary>
  public class HostNotificationList : List<HostNotificationItem>
  {

    private readonly static HostNotificationList _GlobalList = 
      new HostNotificationList();

    /// <summary>
    /// 
    /// </summary>
    public static HostNotificationList GlobalList
    {
      get { return _GlobalList; }
    }

    /// <summary>
    /// build HostNotificationList containing an iterm for each ehllapi session.
    /// </summary>
    public List<ActionItem<HostNotificationItem>> RefreshList()
    {
      var actionList = new List<ActionItem<HostNotificationItem>>();

      // first, build a new list from the MasterSessionList.
      var tempList = new HostNotificationList();
      {
        foreach (var si in MasterSessionList.MasterList)
        {
          var item = new HostNotificationItem(si);
          tempList.Add(item);
        }
      }

      // using the list of sessions, 

      // remove from this actual list all the session items not in the temp list.
      {
        List<HostNotificationItem> removeList = new List<HostNotificationItem>();
        foreach (var item in this)
        {
          var found = MasterSessionList.MasterList.FirstOrDefault(
            c => c.ItemId == item.ItemId && c.IsEnded == false);
          if (found == null)
          {
            removeList.Add(item);
          }
        }

        foreach (var item in removeList)
        {
          this.Remove(item);
          actionList.Add(new ActionItem<HostNotificationItem>(ActionCode.Delete, item));
        }
      }

      // add new temp list items to actual list.
      foreach (var item in MasterSessionList.MasterList)
      {
        if (item.IsEnded == false)
        {
          var found = this.FirstOrDefault(c => c.ItemId == item.ItemId);
          if (found == null)
          {
            var newItem = new HostNotificationItem(item);
            this.Add(newItem);
            actionList.Add(new ActionItem<HostNotificationItem>(ActionCode.Add, newItem));
          }
        }
      }

      return actionList;
    }

    /// <summary>
    /// run StopHostNotification on all the sessions on which close intercept is started.
    /// </summary>
    public void StopNotification()
    {
      foreach (var item in this)
      {
        if (item.NotificationIsStarted == true)
        {
          item.StopNotification();
        }
      }
    }

    /// <summary>
    /// Find the session in this list of Host notification sessions. If found and if
    /// notification is active, stop notification.
    /// </summary>
    /// <param name="MasterItem"></param>
    public void StopNotification(MasterSessionItem MasterItem)
    {
      var found = this.FirstOrDefault(c => c.ItemId == MasterItem.ItemId);
      if ((found != null) && ( found.NotificationIsStarted == true ))
      {
        found.StopNotification();
      }
    }

    /// <summary>
    /// run StartHostNotification on all the session on which close intercept is not
    /// started.
    /// </summary>
    /// <param name="hWnd"></param>
    public void StartNotification(IntPtr hRequestWnd)
    {
      foreach (var item in this)
      {
        if (item.NotificationIsStarted == false)
        {
          item.StartNotification(hRequestWnd);
        }
      }
    }

  }
}
