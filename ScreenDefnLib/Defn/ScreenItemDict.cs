using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenLoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public class ScreenItemDict : Dictionary<OneScreenLoc,IScreenItem>
  {
  }

  public static class ScreenItemDictExt
  {
    public static void Load(this ScreenItemDict ItemDict, IEnumerable<IScreenItem> ItemList)
    {
      ItemDict.Clear();
      foreach( var item in ItemList)
      {
        ItemDict.Add(item.ScreenLoc, item);
      }
    }
  }
}
