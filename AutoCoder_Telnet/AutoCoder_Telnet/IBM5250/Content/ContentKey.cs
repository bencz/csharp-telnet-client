using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContentKey
  {
    public int RowNum
    { get; set; }
    public int ColNum
    { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is ContentKey )
      {
        var objKey = obj as ContentKey;
        if ((objKey.RowNum == this.RowNum) && (objKey.ColNum == this.ColNum))
          return true;
      }
      return false;
    }
    public override int GetHashCode()
    {
      var hashCode = (this.RowNum * 100) + this.ColNum;
      return hashCode;
    }

  }
}
