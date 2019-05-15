using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace AutoCoder.Ext.Windows.Documents
{
  public static class TextSelectionExt
  {

    /// <summary>
    /// Set the FontFamily property of the TextSelection.
    /// </summary>
    /// <param name="Selection"></param>
    /// <param name="FontFamily"></param>
    public static void ApplyFontFamily(this TextSelection Selection, string FontFamily)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var textRange = new TextRange(Selection.Start, Selection.End);
        textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamily);
      }
    }

    /// <summary>
    /// Set the font weight property of the TextSelection.
    /// </summary>
    /// <param name="Selection"></param>
    /// <param name="FontWeight"></param>
    public static void ApplyFontWeight(this TextSelection Selection, FontWeight FontWeight)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeight);
      }
    }

    public static void ApplyFontSize(this TextSelection Selection, string FontSize)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var textRange = new TextRange(Selection.Start, Selection.End);
        textRange.ApplyPropertyValue(TextElement.FontSizeProperty, FontSize);
      }
    }

    public static void ApplyFontStyle(this TextSelection Selection, FontStyle FontStyle)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        tr.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyle);
      }
    }

    public static void ApplyTextAlignment(
      this TextSelection Selection, TextAlignment TextAlignment)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        tr.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment);
      }
    }

    public static void ApplyTextDecoration(
      this TextSelection Selection, TextDecorationCollection TextDecoration)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        tr.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecoration);
      }
    }

    private static List FindListAncestor(DependencyObject element)
    {
      while (element != null)
      {
        List list = element as List;
        if (list != null)
        {
          return list;
        }
        element = LogicalTreeHelper.GetParent(element);
      }
      return null;
    }

    public static FontFamily GetFontFamily(this TextSelection Selection)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var textRange = new TextRange(Selection.Start, Selection.End);
        var fontFamily = textRange.GetPropertyValue(TextElement.FontFamilyProperty);
        if ((fontFamily == null) || (fontFamily == DependencyProperty.UnsetValue))
          return null;
        else
          return fontFamily as FontFamily;
      }
      else
        return null;
    }

    public static double? GetFontSize(this TextSelection Selection)
    {
      if ((Selection != null) && (Selection.Start != null))
      {
        var textRange = new TextRange(Selection.Start, Selection.End);
        var objValue = textRange.GetPropertyValue(TextElement.FontSizeProperty);
        if (objValue == null)
          return null;
        else if (objValue == DependencyProperty.UnsetValue)
          return null;
        else
        {
          var fontSize = (double)objValue;
          return fontSize;
        }
      }
      else
        return null;
    }

    public static TextMarkerStyle? GetListMarkerStyle(this TextSelection Selection)
    {
      TextMarkerStyle? ms = null;

      if (Selection != null)
      {
        List list = FindListAncestor(Selection.Start.Parent);
        if (list != null)
        {
          ms = list.MarkerStyle;
        }
      }
      return ms;
    }

    public static int Length(this TextSelection Selection)
    {
      if (Selection == null)
        return 0;
      else
      {
        var start = Selection.Start;
        var end = Selection.End;
        var tr = new TextRange(start, end);
        return tr.Text.Length;
      }
    }

    /// <summary>
    /// Is TextAlignment property of the selection set to AlignLeft.
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsAlignLeft(this TextSelection Selection)
    {
      bool isAlignLeft = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(FlowDocument.TextAlignmentProperty);
        if (pv != null)
        {
          isAlignLeft = pv.Equals(TextAlignment.Left);
        }
      }
      return isAlignLeft;
    }

    /// <summary>
    /// Is TextAlignment property of the selection set to AlignCenter.
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsAlignCenter(this TextSelection Selection)
    {
      bool isAlignCenter = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(FlowDocument.TextAlignmentProperty);
        if (pv != null)
        {
          isAlignCenter = pv.Equals(TextAlignment.Left);
        }
      }
      return isAlignCenter;
    }

    /// <summary>
    /// Is TextAlignment property of the selection set to AlignRight.
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsAlignRight(this TextSelection Selection)
    {
      bool isAlignRight = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(FlowDocument.TextAlignmentProperty);
        if (pv != null)
        {
          isAlignRight = pv.Equals(TextAlignment.Left);
        }
      }
      return isAlignRight;
    }

    /// <summary>
    /// Is TextAlignment property of the selection set to AlignJustify.
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsAlignJustify(this TextSelection Selection)
    {
      bool isAlignJustify = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(FlowDocument.TextAlignmentProperty);
        if (pv != null)
        {
          isAlignJustify = pv.Equals(TextAlignment.Left);
        }
      }
      return isAlignJustify;
    }

    /// <summary>
    /// is the FontWeight property of the Selection set to Bold
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsBold(this TextSelection Selection)
    {
      bool isBold = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var fwp = tr.GetPropertyValue(TextElement.FontWeightProperty);
        if (fwp != null)
        {
          isBold = fwp.Equals(FontWeights.Bold);
        }
      }
      return isBold;
    }

    /// <summary>
    /// is the FontStyle property of the Selection set to Italic
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsItalic(this TextSelection Selection)
    {
      bool isItalic = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(TextElement.FontStyleProperty);
        if (pv != null)
        {
          isItalic = pv.Equals(FontStyles.Italic);
        }
      }
      return isItalic;
    }

    public static bool IsListMarkerStyleBulleted(this TextSelection Selection)
    {
      var ms = Selection.GetListMarkerStyle();
      if (ms == null)
        return false;
      else if (ms.Value == TextMarkerStyle.Disc)
        return true;
      else
        return false;
    }

    public static bool IsListMarkerStyleNumbered(this TextSelection Selection)
    {
      var ms = Selection.GetListMarkerStyle();
      if (ms == null)
        return false;
      else if (ms.Value == TextMarkerStyle.Decimal)
        return true;
      else
        return false;
    }

    /// <summary>
    /// is the TextDecorations property of the Selection set to Underline
    /// </summary>
    /// <param name="Selection"></param>
    /// <returns></returns>
    public static bool IsUnderline(this TextSelection Selection)
    {
      bool isUnderline = false;
      if (Selection != null)
      {
        var tr = new TextRange(Selection.Start, Selection.End);
        var pv = tr.GetPropertyValue(Inline.TextDecorationsProperty);
        if (pv != null)
        {
          isUnderline = pv.Equals(TextDecorations.Underline);
        }
      }
      return isUnderline;
    }
  }
}



