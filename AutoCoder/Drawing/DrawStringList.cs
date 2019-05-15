using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Drawing
{
  /// <summary>
  ///  Collection of draw strings. 
  /// </summary>
  public class DrawStringList : List<DrawString>
  {

    public event delListChanged ListChanged;

    public DrawStringList()
    {
    }

    public new void Add(DrawString InDrawString)
    {
      base.Add(InDrawString);
      
      // signal that the list has been changed.
      if (ListChanged != null)
      {
        ListChanged(this);
      }
    }

    public new void Clear()
    {
      base.Clear();

      // signal that the list has been changed.
      if (ListChanged != null)
      {
        ListChanged(this);
      }
    }

    /// <summary>
    /// Compare that the list matches another DrawStringList. Don't compare draw 
    /// properties like ForeColor. Only compare on the draw text.
    /// </summary>
    /// <param name="InOther"></param>
    /// <returns></returns>
    public bool DrawTextIsEqual(DrawStringList InOther)
    {
      if (this.Count != InOther.Count)
        return false;
      else
      {
        for (int ix = 0; ix < this.Count; ++ix)
        {
          if (this[ix].Text != InOther[ix].Text)
            return false;
        }
        return true;
      }
    }

    private void SignalListChanged()
    {
      if (ListChanged != null)
      {
        ListChanged(this);
      }
    }
  }
}
