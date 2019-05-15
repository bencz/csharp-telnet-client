using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ehllapi
{
  public class KeyboardKey
  {

    public string _KeyText;
    public string KeyText
    {
      get { return _KeyText; }
      set { _KeyText = value; }
    }

    public KeyboardKey(string KeyText)
    {
      this.KeyText = KeyText;
    }

    public static KeyboardKey Enter
    {
      get { return new KeyboardKey("@E"); }
    }

    public static KeyboardKey FieldExit
    {
      get { return new KeyboardKey("@A@E"); }
    }

    public static KeyboardKey NewLine
    {
      get { return new KeyboardKey("@N"); }
    }

    public static KeyboardKey F3
    {
      get { return new KeyboardKey("@3"); }
    }

    public static KeyboardKey F4
    {
      get { return new KeyboardKey("@4"); }
    }

    public static KeyboardKey F6
    {
      get { return new KeyboardKey("@6"); }
    }

    public static KeyboardKey Shift
    {
      get { return new KeyboardKey("@S"); }
    }

    public static KeyboardKey TabLeft
    {
      get { return new KeyboardKey("@B"); }
    }

    public static KeyboardKey TabRight
    {
      get { return new KeyboardKey("@T"); }
    }

    public static KeyboardKey SystemRequest
    {
      get { return new KeyboardKey("@A@H"); }
    }

    public override string ToString()
    {
      return this.KeyText;
    }
  }
}
