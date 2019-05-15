using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Common.RowCol;

namespace AutoCoder.Telnet.Common.ScreenLoc
{
  public class OneScreenLoc : ScreenLocBase, IScreenLoc
  {
    public OneScreenLoc(int RowNum,int ColNum)
      : base(LocationFrame.OneBased, RowNum, ColNum)
    {
    }

    public OneScreenLoc(IScreenLoc RowCol)
      : this(RowCol.RowNum, RowCol.ColNum)
    {
    }

    public IScreenLoc NewInstance(int RowNum, int ColNum)
    {
      return new OneScreenLoc(RowNum, ColNum);
    }

    public ZeroRowCol ToZeroRowCol()
    {
      var rowNum = this.RowNum - 1;
      var colNum = this.ColNum - 1;
      return new RowCol.ZeroRowCol(rowNum, colNum);
    }

    public override string ToString()
    {
      return "OneScreenLoc " + this.ValueText();
    }

    public string ValueText( )
    {
      return this.RowNum + "," + this.ColNum;
    }
  }
}
