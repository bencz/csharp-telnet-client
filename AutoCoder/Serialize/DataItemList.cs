using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Serialize
{
  /// <summary>
  /// a single row of dataItem
  /// </summary>
  public class DataItemList : List<DataItem>
  {
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("DataItemList. " + this.Count + " items. ");
      bool firstItem = true;
      foreach( var item in this)
      {
        if (firstItem == false)
          sb.Append(", ");
        sb.Append(item.Value);
        firstItem = false;
      }
      return sb.ToString();
    }
  }
}
