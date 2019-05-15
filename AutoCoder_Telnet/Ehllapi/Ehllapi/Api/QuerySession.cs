using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Enums;

namespace AutoCoder.Ehllapi.Api
{
  /// <summary>
  /// session item returned from pcsQuerySessionList
  /// </summary>
  public class QuerySession
  {
    public string SessId
    { get; set; }

    public SessionStatus Status
    { get; set; }
  }
}
