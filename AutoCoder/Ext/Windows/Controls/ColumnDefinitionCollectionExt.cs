using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class ColumnDefinitionCollectionExt
  {
    public static ColumnDefinition AddColumn(this ColumnDefinitionCollection defn, double Width)
    {
      var coldef = new ColumnDefinition();
      coldef.Width = new GridLength( Width) ;
      defn.Add(coldef);
      return coldef;
    }

    public static ColumnDefinition AddAutoColumn(this ColumnDefinitionCollection defn)
    {
      var coldef = new ColumnDefinition();
      coldef.Width = GridLength.Auto;
      defn.Add(coldef);
      return coldef;
    }
    public static ColumnDefinition AddStarColumn(this ColumnDefinitionCollection defn, int NumberUnits = 1)
    {
      var coldef = new ColumnDefinition();
      coldef.Width = new GridLength(NumberUnits, GridUnitType.Star);
      defn.Add(coldef);
      return coldef;
    }

  }
}

