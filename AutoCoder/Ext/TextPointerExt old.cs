using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using AutoCoder.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Ext
{
  public static class TextPointerExt
  {

    /// <summary>
    /// return TextPointer position before the last char of the line of the input
    /// TextPointer.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="Doc"></param>
    /// <returns></returns>
    public static TextPointer GetLineEndPosition(
      this TextPointer TextPointer, FlowDocument Doc,
      RelativePosition Relative = RelativePosition.After )
    {
      TextPointer tp1 = TextPointer.GetLineStartPosition(1);
      if (tp1 == null)
      {
        tp1 = Doc.ContentEnd;
        tp1 = tp1.GetNextInsertionPosition(LogicalDirection.Backward);
      }
      else
      {
        tp1 = tp1.GetNextInsertionPosition(LogicalDirection.Backward);

        // position before the last char of the line.
        if (( tp1 != null ) && ( Relative == RelativePosition.Before ))
          tp1 = tp1.GetNextInsertionPosition(LogicalDirection.Backward);
      }
      return tp1;
    }

    /// <summary>
    /// calc the line number of the line the TextPointer is located on in the
    /// FlowDocument.
    /// </summary>
    /// <param name="Pointer"></param>
    /// <param name="Doc"></param>
    /// <returns></returns>
    public static int GetLineNumber(this TextPointer Pointer, FlowDocument Doc)
    {
      TextPointer pointerStart = Pointer.GetLineStartPosition(0);
      TextPointer p = Doc.ContentStart.GetLineStartPosition(0);
      int lineNumber = 0;

      while (true)
      {
        if (pointerStart.CompareTo(p) <= 0)
        {
          break;
        }

        p = p.GetLineStartPosition(1);
        if (p == null)
          break;

        lineNumber++;
      }

      return lineNumber;
    }

    /// <summary>
    /// Get the text of the line that the text pointer is positioned at.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="Doc">The FlowDocument that contains the lines.</param>
    /// <param name="LineRltv">Number of lines relative to the text pointer.</param>
    /// <param name="Default">Value to return when TextPointer is null.</param>
    /// <returns></returns>
    public static string GetLineText(
      this TextPointer TextPointer,
      FlowDocument Doc, 
      int LineRltv = 0, string Default = null)
    {
      if (TextPointer == null)
        return Default;
      else
      {
        TextPointer tp1 = TextPointer.GetLineStartPosition(LineRltv);
        if (tp1 == null)
        {
          return Default;
        }

        else
        {
          TextRange tr = null;
          TextPointer tp2 = tp1.GetLineStartPosition(1);
          if (tp2 == null)
          {
            tp2 = Doc.ContentEnd;
          }
          tr = new TextRange(tp1, tp2);

          // trim the NewLine from end of the line.
          string lineText = tr.Text;
          if ((lineText.Length >= 2) && (lineText.Tail(2) == Environment.NewLine))
          {
            lineText = lineText.Substring(0, lineText.Length - 2);
          }

          return lineText;
        }
      }
    }

    public static int GetPositionInLine(this TextPointer Pointer)
    {
      TextPointer tp1 = Pointer.GetLineStartPosition(0);
      int ox = tp1.GetOffsetToPosition(Pointer);
      return ox - 1;
    }

    public static string GetRunText(this TextPointer TextPointer)
    {
      string afterText = TextPointer.GetTextInRun(LogicalDirection.Forward);
      string beforeText = TextPointer.GetTextInRun(LogicalDirection.Backward);
      string fullText = beforeText + afterText;

      return fullText;
    }

    /// <summary>
    /// Pad the line of the text pointer to the length specified.
    /// </summary>
    /// <param name="Doc"></param>
    /// <param name="Pointer"></param>
    /// <param name="Length"></param>
    public static void PadLineToLength(
      this TextPointer Pointer, FlowDocument Doc, int Length, char PadChar = ' ')
    {
      TextPointer tp1 = Pointer.GetLineStartPosition(0);
      TextPointer tp2 = Pointer.GetLineStartPosition(1);
      if (tp2 != null)
      {
        tp2 = tp2.GetNextInsertionPosition(LogicalDirection.Backward);
      }
      else
      {
        tp2 = Doc.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
      }

      // current line text.
      TextRange r1 = new TextRange(tp1, tp2);
      string lineText = r1.Text;

      // pad length
      string padText = null;
      int padLx = Length - lineText.Length;
      if (padLx > 0)
      {
        padText = new StringBuilder().AppendRepeat(PadChar, padLx).ToString();
        tp2.InsertTextInRun(padText);
      }
    }

    /// <summary>
    /// return a TextPointer positioned before a specified position on the line.
    /// If the position exceeds the end of the line, either pad the line with
    /// blanks or set the position to the last char on the line.
    /// </summary>
    /// <param name="Doc"></param>
    /// <param name="Pointer"></param>
    /// <param name="Position"></param>
    /// <param name="Pad"></param>
    /// <returns></returns>
    public static TextPointer SetPositionOnLine(
      this TextPointer Pointer,
      FlowDocument Doc, int Position,
      bool Pad = false)
    {
      TextPointer tp = Pointer.GetLineStartPosition(0);
      int pos = Position;

      // make sure length of the line can accomodate the position.
      string lineText = Pointer.GetLineText(Doc, 0, "");
      if (pos > (lineText.Length - 1))
      {
        if (Pad == false)
          pos = lineText.Length - 1;
        else
        {
          Pointer.PadLineToLength(Doc, pos + 1);
          lineText = Pointer.GetLineText(Doc);
        }
      }

      // position to the last char on the line.
      if (lineText.Length == (pos + 1))
      {
        tp = Pointer.GetLineEndPosition(Doc);
      }

        // advance a char at a time from the start of the line.
      else
      {
        for (int ix = 0; ix < pos; ++ix)
        {
          var nextTp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
          if (nextTp != null)
            tp = nextTp;
          else
            break;
        }
      }

      return tp;
    }
  }
}
