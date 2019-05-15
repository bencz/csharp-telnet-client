using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon
{
  public enum ReportItemType
  {
    RepeatToAddress,
    Field,
    Literal,
    sba,
    CrtWdw,
    EraseToAddress
  }

  public static class ReportItemTypeExt
  {
    public static string ToString(ReportItemType? ItemType)
    {
      if (ItemType == null)
        return "";
      else
        return ItemType.Value.ToString();
    }
  }
}
