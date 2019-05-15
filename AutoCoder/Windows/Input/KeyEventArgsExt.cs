using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AutoCoder.Windows.Input
{
  // moved to AutoCoder.Ext.Windows.Input

#if skip
  public static class KeyEventArgsExt
  {
    /// <summary>
    /// return the actual keypress. When the alt key is pressed the Key value is
    /// System and the actual keypress is stored in SystemKey.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Key GetActualKey(this KeyEventArgs args)
    {
      if (args.Key != Key.System)
        return args.Key;
      else
        return args.SystemKey;
    }
  }
#endif

}
