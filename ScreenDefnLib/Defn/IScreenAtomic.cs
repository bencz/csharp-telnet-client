using AutoCoder.Enums;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public interface IScreenAtomic : IScreenItem
  {
    int Length { get; set; }
    DsplyAttr[] DsplyAttr { get; set; }
  }

  public static class IScreenAtomicExt
  {
    public static IScreenLoc EndLoc(this IScreenAtomic item)
    {
      var rowNum = item.ScreenLoc.RowNum;
      var colNum = item.ScreenLoc.ColNum + item.Length - 1;
      return item.ScreenLoc.NewInstance(rowNum, colNum);
    }

    public static ScreenLocRange ToRange(this IScreenAtomic item)
    {
      var range = new ScreenLocRange(item.ScreenLoc, item.EndLoc(), RangeForm.Linear);
      return range;
    }
  }
}
