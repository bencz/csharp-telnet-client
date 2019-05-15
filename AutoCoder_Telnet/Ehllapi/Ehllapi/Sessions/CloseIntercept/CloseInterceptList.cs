using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api;
using AutoCoder.Core;
using AutoCoder.Ehllapi.Sessions.Master;

namespace AutoCoder.Ehllapi.Sessions.CloseIntercept
{
  /// <summary>
  /// list of CloseIntercept sessions.
  /// </summary>
  public class CloseInterceptList : List<CloseInterceptItem>
  {

    private readonly static CloseInterceptList _GlobalList = new CloseInterceptList();

    /// <summary>
    /// 
    /// </summary>
    public static CloseInterceptList GlobalList
    {
      get { return _GlobalList; }
    }

    /// <summary>
    /// build CloseInterceptList containing an iterm for each ehllapi session.
    /// </summary>
    public List<ActionItem<CloseInterceptItem>> RefreshList()
    {
      var actionList = new List<ActionItem<CloseInterceptItem>>();

      // first, build a new list from the MasterSessionList.
      var tempList = new CloseInterceptList();
      {
        foreach (var si in MasterSessionList.MasterList)
        {
          var item = new CloseInterceptItem(si);
          tempList.Add(item);
        }
      }

      // using the list of sessions, 

      // remove from this actual list all the session items not in the temp list.
      {
        List<CloseInterceptItem> removeList = new List<CloseInterceptItem>();
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
          actionList.Add(new ActionItem<CloseInterceptItem>(ActionCode.Delete, item));
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
            var newItem = new CloseInterceptItem(item);
            this.Add(newItem);
            actionList.Add(new ActionItem<CloseInterceptItem>(ActionCode.Add, newItem));
          }
        }
      }

      return actionList;
    }

    /// <summary>
    /// run StopCloseIntercept on all the sessions on which close intercept is started.
    /// </summary>
    public void StopCloseIntercept()
    {
      foreach (var item in this)
      {
        if (item.InterceptIsStarted == true)
        {
          item.StopCloseIntercept();
        }
      }
    }

    /// <summary>
    /// run StartCloseIntercept on all the session on which close intercept is not
    /// started.
    /// </summary>
    /// <param name="hWnd"></param>
    public void StartCloseIntercept(IntPtr hRequestWnd)
    {
      foreach (var item in this)
      {
        if (item.InterceptIsStarted == false)
        {
          item.StartCloseIntercept(hRequestWnd);
        }
      }
    }

  }
}
