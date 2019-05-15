using AutoCoder.Ext.System.Text;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  /// <summary>
  /// message to send to ToThread. send response to telnet server containing screen
  /// entry data.
  /// </summary>
  public class AidKeyResponseMessage : ThreadMessageBase
  {
    public ScreenContent ScreenContent
    { get; set; }

    public AidKey AidKey
    { get; set; }

    public AidKeyResponseMessage(AidKey AidKey)
    {
      this.AidKey = AidKey;
    }

    public AidKeyResponseMessage(AidKey AidKey, ScreenContent ScreenContent)
    {
      this.AidKey = AidKey;
      this.ScreenContent = ScreenContent;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("AidKeyResponseMessage. AidKey:" + this.AidKey.ToString( ));
      if (this.ScreenContent != null)
        sb.SentenceAppend(this.ScreenContent.ToString());
      return sb.ToString();
    }
  }
}
