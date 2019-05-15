using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Collections
{

  /// <summary>
  /// extension methods of LinkedList. Position in the LinkedList and return a
  /// LinkedListCursor.
  /// </summary>
  public static class LinkedListExt
  {
    public static LinkedListCursor<T> PositionBegin<T>(this LinkedList<T> List) where T : class
    {
      var firstNode = List.First;
      LinkedListCursor<T> cursor = new LinkedListCursor<T>(firstNode, RelativePosition.Before);
      return cursor;
    }
  }

}
