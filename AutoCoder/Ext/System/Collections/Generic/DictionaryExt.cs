using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Collections.Generic
{
  public static class DictionaryExt
  {
    public static void EnsureAdded<K, V>(this Dictionary<K, V> Dict, K Key, V Value)
    {
      if (Dict.ContainsKey(Key) == true)
      {
        Dict[Key] = Value;
      }
      else
      {
        Dict.Add(Key, Value);
      }
    }

    public static void EnsureRemoved<K,V>(this Dictionary<K,V> Dict, K Key)
    {
      if ( Dict.ContainsKey(Key) == true )
      {
        Dict.Remove(Key);
      }
    }


  }
}
