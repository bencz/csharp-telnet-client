using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TextCanvasLib.Keybd
{
#if skip
  /// <summary>
  /// a shifted instance of a Key code.  that is, the Key code + the key code of
  /// the shift key.
  /// </summary>
  public class ShiftedKey
  {
    public ShiftedKey( Key keyCode, Key shiftCode )
    {
      this.KeyCode = keyCode;
      this.ShiftCode = shiftCode;
    }
    public Key KeyCode
    { get; set; }

    public Key ShiftCode
    { get; set; }


  }
#endif
}
