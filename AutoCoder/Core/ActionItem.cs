using AutoCoder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core
{
  /// <summary>
  /// an item that an action has been applied against.
  /// A method that applies actions against items in a list can use the ActionItem
  /// class to return a list of items affected by the method with the ActionCode
  /// of the performed action. ( some items are added, others deleted from the list )
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ActionItem<T> where T : class
  {
    public ActionCode ActionCode
    { get; set; }

    public T Item
    { get; set; }

    public ActionItem(ActionCode ActionCode, T Item)
    {
      this.ActionCode = ActionCode;
      this.Item = Item;
    }
  }
}
