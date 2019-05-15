using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.ThreadMessages;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.ThreadMessages
{
  public class CaptureContentMessage : ThreadMessageBase
  {
    public string CaptureFolderPath { get; set; }

    /// <summary>
    /// auto capture to capture database on every match.
    /// </summary>
    public bool CaptureAuto { get; set; }
    public IScreenDefn ScreenDefn { get; set; }
    public ScreenContent ScreenContent { get; set; }

    public CaptureContentMessage(string CaptureFolderPath, bool CaptureAuto, 
      IScreenDefn ScreenDefn, ScreenContent ScreenContent)
    {
      this.CaptureAuto = CaptureAuto;
      this.CaptureFolderPath = CaptureFolderPath;
      this.ScreenDefn = ScreenDefn;
      this.ScreenContent = ScreenContent;
    }
  }
}
