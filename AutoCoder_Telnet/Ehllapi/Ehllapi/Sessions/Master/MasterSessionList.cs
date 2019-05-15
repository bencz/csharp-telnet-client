using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api;
using AutoCoder.Core;
using System.Threading;

namespace AutoCoder.Ehllapi.Sessions.Master
{
  /// <summary>
  /// list of CloseIntercept sessions.
  /// </summary>
  public class MasterSessionList : List<MasterSessionItem>
  {

    private readonly static MasterSessionList _MasterList = new MasterSessionList();

    /// <summary>
    /// 
    /// </summary>
    public static MasterSessionList MasterList
    {
      get { return _MasterList; }
    }

    static readonly EventWaitHandle _ChangedSignal =
      new EventWaitHandle(false, EventResetMode.AutoReset);

    /// <summary>
    /// this event is signaled whenever the MasterSessionList is changed.
    /// </summary>
    public static EventWaitHandle ChangedSignal
    {
      get { return _ChangedSignal; }
    }

    /// <summary>
    /// build MasterSessionList containing an iterm for each ehllapi session.
    /// </summary>
    public List<ActionItem<MasterSessionItem>> RefreshList()
    {
      var actionList = new List<ActionItem<MasterSessionItem>>();

      // first, build a new list from QuerySessionList output.
      var tempList = new MasterSessionList();
      {
        foreach (var si in pcsapi.QuerySessionList())
        {
          var item = new MasterSessionItem(si);
          tempList.Add(item);

          // get the session long name.
          var sa = Ehllapier.QuerySessionStatus(item.SessId);
          item.LongName = sa.LongName;
        }
      }

      // using the list of sessions, 

      // mark as IsEnded all the actual list items not found in the temp list.
      {
        List<MasterSessionItem> endedList = new List<MasterSessionItem>();
        foreach (var item in this)
        {
          if (item.IsEnded == false)
          {
            var found = tempList.FirstOrDefault(c => c.SessId == item.SessId);
            if (found == null)
            {
              item.IsEnded = true;
              ChangedSignal.Set();
            }
          }
        }
      }

      // mark as IsEnded any matching SessId items where the hWnd is different.
      foreach (var item in tempList)
      {
        var found = this.FirstOrDefault(
          c => c.SessId == item.SessId && c.hSessWnd != item.hSessWnd 
            && c.IsEnded == false);
        if (found == null)
        {
          item.IsEnded = true;
          ChangedSignal.Set();
        }
      }

      // add new temp list items to actual list.
      foreach (var item in tempList)
      {
        var found = this.FirstOrDefault(c => c.SessId == item.SessId && c.IsEnded == false);
        if (found == null)
        {
          var newItem = new MasterSessionItem(item);
          this.Add(newItem);
          ChangedSignal.Set();
          actionList.Add(new ActionItem<MasterSessionItem>(ActionCode.Add, newItem));
        }
      }

      // change any items with a different window text.
      foreach (var item in this)
      {
        if (item.IsEnded == false)
        {
          var found = tempList.FirstOrDefault(
            c => c.SessId == item.SessId && c.WindowText != item.WindowText);
          if (found != null)
          {
            item.WindowText = found.WindowText;
            ChangedSignal.Set();
            actionList.Add(new ActionItem<MasterSessionItem>(ActionCode.Change, item));
          }
        }
      }

      return actionList;
    }
  }
}
