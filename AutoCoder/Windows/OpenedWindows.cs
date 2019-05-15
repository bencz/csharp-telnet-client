using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoCoder.Windows
{
  /// <summary>
  /// List of Window objects that have been opened by an owning window.
  /// Used to close all those windows when an owning window is closed.
  /// </summary>
  public class OpenedWindows : LinkedList<global::System.Windows.Window>
  {

    public OpenedWindows()
    {
    }

    /// <summary>
    /// Remove from the list of opened windows all windows which are closed.
    /// This is a housekeeping function called by the owner of the list. 
    /// </summary>
    public void Cleanup( )
    {
      LinkedListNode<System.Windows.Window> node = this.First;
      while (true)
      {
        if (node == null)
          break;
        LinkedListNode<Window> nx = node.Next;
        if (node.Value.IsLoaded == false)
          this.Remove(node);
        node = nx;
      }
    }

    /// <summary>
    /// Call the Close( ) method on each of the windows in the list of
    /// opened windows.
    /// </summary>
    public void CloseAll( )
    {
      // close all the DocumentConstruction windows opened from this window.
      foreach (Window openedWindow in this)
      {
        if (openedWindow.IsLoaded == true)
        {
          openedWindow.Close();
        }
      }
    }


  }
}
