using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class ClearUnitMessage :ThreadMessageBase
  {
    public int ContentNum
    { get; set; }

    public ClearUnitMessage( int ContentNum )
    {
      this.ContentNum = ContentNum;
    }
  }
}
