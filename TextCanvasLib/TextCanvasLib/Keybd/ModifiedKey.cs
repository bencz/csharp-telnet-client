using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TextCanvasLib.Keybd
{
  /// <summary>
  /// the combination of keys. Such as Shift + A. Or Ctl + Alt + Delete.
  /// The combo consists of the initial key and then the qualifying keys.
  /// The combination of keys cannot contain more than 1 text or action keys. But
  /// contain multiple gesture keys such as ALT, SHIFT and CTRL.
  /// </summary>
  public class ModifiedKey
  {
    public Key Key
    { get; set; }

    public ModifierKeys Modifiers
    { get; set; }

    public ModifiedKey(Key key)
    {
      this.Key = key;
      this.Modifiers = ModifierKeys.None;
    }

    public ModifiedKey(Key key, ModifierKeys modifiers)
    {
      this.Key = key;
      this.Modifiers = modifiers;
    }
  }

  public static class ModifiedKeyExt
  {
    public static ModifiedKey ToModifiedKey(this KeyEventArgs args)
    {
      var modkey = new ModifiedKey(args.Key, Keyboard.Modifiers);
      return modkey;
    }
  }
}
