using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace AutoCoder.Ext.Windows.Input
{
  public static class KeyEventArgsExt
  {
    /// <summary>
    /// return the Key of the actual key pressed. 
    /// When the alt key is pressed the actual ALT key. 
    /// When numeric key pad  key, return that key regardless of whether numlock
    /// is down.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Key GetActualKey(this KeyEventArgs args)
    {
      if (args.Key == Key.System)
        return args.SystemKey;
      else
        return args.ToExtendedKey( );
    }

    public static bool GetExtendedFlag(this KeyEventArgs args)
    {
      bool isExtended = (bool)typeof(KeyEventArgs).InvokeMember("IsExtendedKey",
        BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance,
        null, args, null);
      return isExtended;
    }

    static Key[] extendedTrueKeys = {
      Key.Insert, Key.Delete,
      Key.End, Key.Down, Key.Next,
      Key.Left, Key.Clear, Key.Right,
      Key.Home, Key.Up, Key.PageUp };

    static Key[] extendedFalseKeys = {
      Key.NumPad0, Key.Decimal,
      Key.NumPad1, Key.NumPad2, Key.NumPad3,
      Key.NumPad4, Key.NumPad5, Key.NumPad6,
      Key.NumPad7, Key.NumPad8, Key.NumPad9 };

    /// <summary>
    /// return the args.Key key enum when the extended flag is taken into 
    /// account. Some keys on the keyboard have the same Key enum. Depending on
    /// if NUM LOCK is down the key pad keys return either the keypad code or
    /// the key code of a key on the keyboard.The end key 
    /// enum is the same for the end key and for the numpad1 key. Look to the
    /// IsExtendedKey value to determine the difference.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Key ToExtendedKey( this KeyEventArgs args )
    {
      var fx = Array.IndexOf<Key>(extendedTrueKeys, args.Key);
      bool isExtended = args.GetExtendedFlag();
      if ( fx != -1 )
      {
        if (isExtended == true)
          return args.Key;
        else
          return extendedFalseKeys[fx];
      }
      else if ((args.Key == Key.Return ) && ( isExtended == true ))
      {
        return Key.Separator;
      }
      else
        return args.Key;
    }
  }
}
