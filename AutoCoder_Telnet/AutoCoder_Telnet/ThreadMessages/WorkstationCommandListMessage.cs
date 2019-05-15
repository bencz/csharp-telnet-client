using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class WorkstationCommandListMessage : ThreadMessageBase
  {
    public WorkstationCommandList WorkstationCommandList
    { get; set; }
    public WorkstationCommandListMessage(
      WorkstationCommandList WorkstationCommandList)
    {
      this.WorkstationCommandList = WorkstationCommandList;
    }
  }
}
