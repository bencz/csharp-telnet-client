using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace AutoCoder.Ext.Windows.Documents
{
  public static class ParagraphExt
  {
    /// <summary>
    /// return the plain text of the paragraph.
    /// </summary>
    /// <param name="Para"></param>
    /// <returns></returns>
    public static string GetText(this Paragraph Para)
    {
      var tp1 = Para.ContentStart;
      var tp2 = Para.ContentEnd;
      var rng = new TextRange(tp1, tp2);
      return rng.Text;
    }

    /// <summary>
    /// Set the text of the paragraph.
    /// </summary>
    /// <param name="Para"></param>
    /// <param name="Text"></param>
    public static void SetText(this Paragraph Para, string Text)
    {
      var rng = Para.ToTextRange();
      rng.Text = Text;
    }

    /// <summary>
    /// return a TextRange that runs from the start of the paragraph to the end.
    /// </summary>
    /// <param name="Para"></param>
    /// <returns></returns>
    public static TextRange ToTextRange(this Paragraph Para)
    {
      return new TextRange(Para.ContentStart, Para.ContentEnd);
    }
  }
}
