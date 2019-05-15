using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum LogItemSpecial
  {
    ClearLog,
    NewGeneration    // switch to a new generation of the output log file.
                     // Create new log file, where name contains gen number at the
                     // end.
  }
}
