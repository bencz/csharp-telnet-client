using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoCoder.Windows.Controls
{
  /// <summary>
  /// a ComboBoxModel object that adds entered values to the dropdown values list.
  /// </summary>
  public class ComboBoxAccumValueModel : ComboBoxModel
  {
    public ComboBoxAccumValueModel()
      : base(true)
    {
    }

    public ComboBoxAccumValueModel(ComboBoxModel ComboModel)
      : base(true)
    {
      ApplyRecentToTop = ComboModel.ApplyRecentToTop;
      AcceptBlanks = ComboModel.AcceptBlanks;
      Value = ComboModel.Value;
      ValuesList = ComboModel.ValuesList;
    }
  }

  public static class ComboBoxAccumValueModelExt
  {
    public static ComboBoxAccumValueModel ToComboBoxAccumValueModel(
      this XElement Elem, XNamespace Namespace)
    {
      var comboModel = Elem.ToComboBoxModel(Namespace);
      ComboBoxAccumValueModel accumModel = null;
      if (comboModel != null)
      {
        accumModel = new ComboBoxAccumValueModel()
        {
          ApplyRecentToTop = comboModel.ApplyRecentToTop,
          Value = comboModel.Value,
          ValuesList = comboModel.ValuesList
        };
      }
      return accumModel;
    }
  }
}

