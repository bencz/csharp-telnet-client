using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class RowDefinitionCollectionExt
  {
    public static RowDefinition AddAutoRow(this RowDefinitionCollection defn)
    {
      var rowdef = new RowDefinition();
      rowdef.Height = GridLength.Auto;
      defn.Add(rowdef);
      return rowdef;
    }
    public static RowDefinition AddStarRow(this RowDefinitionCollection defn, int NumberUnits = 1)
    {
      var rowdef = new RowDefinition();
      rowdef.Height = new GridLength(NumberUnits, GridUnitType.Star);
      defn.Add(rowdef);
      return rowdef;
    }
  }
}
