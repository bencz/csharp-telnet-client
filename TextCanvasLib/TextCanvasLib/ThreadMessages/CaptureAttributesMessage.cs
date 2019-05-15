using AutoCoder.Telnet.ThreadMessages;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.ThreadMessages
{
  public class CaptureAttributesMessage : ThreadMessageBase
  {
    public string CaptureFolderPath { get; set; }

    /// <summary>
    /// auto capture to capture database on every match.
    /// </summary>
    public bool CaptureAuto { get; set; }

    public IScreenDefn ScreenDefn { get; set;  }

    public CaptureAttributesMessage(string CaptureFolderPath, bool CaptureAuto)
    {
      this.CaptureAuto = CaptureAuto;
      this.CaptureFolderPath = CaptureFolderPath;
    }
  }
}
