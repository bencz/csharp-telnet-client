using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Enums
{
  /// <summary>
  /// action caused by a keystroke. as opposed to a resulting character. A key 
  /// combo results in either an action or text.
  /// 
  /// enum values match those of System.Windows.Input.Key enum.
  /// </summary>
  public enum KeyAction
  {
    None = 0,
    Back = 2,
    Tab = 3,
    Clear = 5,
    Enter = 6,
    Escape = 13,
    PageUp = 19,
    PageDown = 20,
    End = 21,
    Home = 22,
    Left = 23,
    Up = 24,
    Right = 25,
    Down = 26,
    Print = 28,
    PrintScreen = 30,
    Insert = 31,
    Delete = 32,
    Help = 33,

    F01 = 501,
    F02 = 502,
    F03 = 503,
    F04 = 504,
    F05 = 505,
    F06 = 506,
    F07 = 507,
    F08 = 508,
    F09 = 509,
    F10 = 510,
    F11 = 511,
    F12 = 512,
    F13 = 513,
    F14 = 514,
    F15 = 515,
    F16 = 516,
    F17 = 517,
    F18 = 518,
    F19 = 519,
    F20 = 520,
    F21 = 521,
    F22 = 522,
    F23 = 523,
    F24 = 524,

    Macro = 541
  }
}
