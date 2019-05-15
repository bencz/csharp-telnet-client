using AutoCoder.Telnet.Common.ScreenLoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Ext.System;

namespace ScreenDefnLib.Defn
{
  /// <summary>
  /// the instance of a screen item on the screen. ( includes the row number of
  /// a detail row )
  /// </summary>
  public class ScreenItemInstance
  {
    public IScreenItem Item
    { get; set; }

    public int RepeatNum
    { get; set; }

    /// <summary>
    /// absolute location of the screen item.
    /// </summary>
    public IScreenLoc ItemLoc
    { get; set; }

    public ScreenItemInstance(IScreenItem item, IScreenLoc itemLoc )
    {
      this.Item = item;
      this.ItemLoc = itemLoc;
      this.RepeatNum = 0;
    }

    public string GetItemName( )
    {
      if (this.Item != null)
        return this.Item.ItemName;
      else
        return null;
    }

    public string GetValue(ScreenContent content)
    {
      string text = null;
      var contentItemBase = content.GetContentItem(this.ItemLoc);
      if ( contentItemBase != null)
      {
        text = contentItemBase.GetShowText(content);
      }
      return text;
    }
  }
}
