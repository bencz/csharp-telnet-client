using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AutoCoder.Ext.Windows.Documents;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class RichTextBoxExt
  {
    public static int GetLineCount(this RichTextBox TextBox)
    {
      var lines = TextBox.ToTextLines();
      return lines.Count();
    }

    public static string[] ToTextLines(this RichTextBox TextBox)
    {
      TextRange tr = new TextRange(
        TextBox.Document.ContentStart, TextBox.Document.ContentEnd);
      string textBuf = tr.Text.TrimEnd(new char[] { ' ', '\r', '\n' });
      string[] lines = textBuf.Split(
        new string[] { Environment.NewLine }, StringSplitOptions.None);
      
      return lines;
    }

    /// <summary>
    /// assign the TextLines to the Document property of the RichTextBox.
    /// </summary>
    /// <param name="TextBox"></param>
    /// <param name="TextLines"></param>
    public static void DocumentTextLines(this RichTextBox TextBox, IEnumerable<string> TextLines)
    {
      FlowDocument flowDoc = new FlowDocument();

      var para = new Paragraph();

      foreach (var textLine in TextLines)
      {
        Run run = new Run(textLine);
        para.Inlines.Add(run);
        para.Inlines.Add(new LineBreak());

      }
      flowDoc.Blocks.Add(para);
      TextBox.Document = flowDoc;
    }

    /// <summary>
    /// return the text lines content of the Document property of the RichTextBox.
    /// </summary>
    /// <param name="TextBox"></param>
    /// <returns></returns>
    public static IEnumerable<string> DocumentTextLines(this RichTextBox TextBox)
    {
      TextRange tr = new TextRange(
        TextBox.Document.ContentStart, TextBox.Document.ContentEnd);
      string textBuf = tr.Text.TrimEnd(new char[] { ' ', '\r', '\n' });
      string[] lines = textBuf.Split(
        new string[] { Environment.NewLine }, StringSplitOptions.None);

      return lines;
    }

    /// <summary>
    /// return the location on the text line of the specified coordinate point within
    /// the RichTextBox.
    /// </summary>
    /// <param name="TextBox"></param>
    /// <param name="Point"></param>
    /// <returns></returns>
    public static Tuple<TextPointer, int, int> GetLocationFromPoint(
      this RichTextBox TextBox, Point Point)
    {
      TextPointer tp = TextBox.GetPositionFromPoint(Point, true);

      var px = tp.GetPositionInLine();
      if ( px > 0 )
        px = px -1;

      var lineBx = tp.GetLineNumber(TextBox.Document);

      return new Tuple<TextPointer, int, int>(tp, lineBx, px);
    }

  }
}
