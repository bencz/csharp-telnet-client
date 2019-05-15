using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Enums;

namespace TextCanvasLib.Keybd
{
  /// <summary>
  /// map a ModifiedKey to an action or character string.
  /// </summary>
  public class MappedModifiedKey
  {
    public MappedModifiedKey( )
    {
      this.FromKey = null;
      this.ToText = null;
      this.ToAction = KeyAction.None;
    }
    public ModifiedKey FromKey
    { get; set; }

    public string ToText
    { get; set; }

    public KeyAction ToAction
    { get; set; }
  }
}
