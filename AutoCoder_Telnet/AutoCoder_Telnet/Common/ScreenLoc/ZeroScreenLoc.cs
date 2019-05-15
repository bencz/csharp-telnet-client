using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.Enums;

namespace AutoCoder.Telnet.Common.ScreenLoc
{
  public class ZeroScreenLoc : ScreenLocBase, IScreenLoc
  {
    public ZeroScreenLoc(int RowNum, int ColNum)
      : base(LocationFrame.ZeroBased, RowNum, ColNum)
    {
    }

    public IScreenLoc NewInstance(int RowNum, int ColNum)
    {
      return new ZeroScreenLoc(RowNum, ColNum);
    }

    public override bool Equals(object obj)
    {
      if (obj is ZeroScreenLoc)
      {
        var objKey = obj as ZeroScreenLoc;
        if ((objKey.RowNum == this.RowNum) && (objKey.ColNum == this.ColNum))
          return true;
      }
      else if (obj is IScreenLoc)
      {
        var key = obj as IScreenLoc;
        if (key.LocationFrame == LocationFrame.OneBased)
        {
          key = key.ToZeroBased();
        }
        if ((key.RowNum == this.RowNum) && (key.ColNum == this.ColNum))
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
