using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.Forms
{
  public static class DataGridViewCeller
  {
    /// <summary>
    /// test if cell contains null or spaces
    /// </summary>
    /// <param name="InCell"></param>
    /// <returns></returns>
    public static bool CellIsBlank(DataGridViewCell InCell)
    {
      if (InCell.Value == null)
        return true;
      else if (InCell.Value is string)
      {
        string s1 = (string)InCell.Value;
        if (s1.IsBlank())
          return true;
      }
      return false;
    }
  }
}
