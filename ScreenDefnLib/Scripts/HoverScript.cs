using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Scripts
{
  public static class HoverScript
  {
    public static object GetHoverData(DataTable itemTable, int SelectedRowNum)
    {
      var row = itemTable.Rows[SelectedRowNum];
      var srcmbr = row["MbrName"].ToString();
      var srcfName = row["SrcfName"].ToString();
      var srcfLib = row["SrcfLib"].ToString();
      var hoverData = SupportCode.GetSrcmbrLines(srcfName, srcfLib, srcmbr);
      return hoverData;
    }
  }
}
