using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Interfaces
{
  public interface ILogItem
  {
    DateTime LogTime
    { get; set; }
    string LogText { get; }
    string MergeName { get; set; }
    bool NewGroup { get; set; }
    ILogItem NewCopy();
    ILogItem NewItem();
  }

  public static class ILogItemExt
  {
    public static void ApppendMergeName(this ILogItem item, ILogList list)
    {
      var mergeName = item.MergeName;
      if (list.ListName.IsNullOrEmpty() == false)
      {
        if (mergeName.IsNullOrEmpty() == true)
          mergeName = list.ListName;
        else
          mergeName += "." + list.ListName;

        item.MergeName = mergeName;
      }
    }
  }
}
