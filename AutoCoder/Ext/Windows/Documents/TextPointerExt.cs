using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using AutoCoder.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Ext.Windows.Documents
{

  // GetRunInfo         : return the Run and position in the Run of a TextPointer. 
  // GetRunText         : return the combined text before and after the run 
  // IsAfter            : evaluate if a TextPointer is after another TextPointer. 
  // IsAtOrAfter        : evaluate if a TextPointer is at or after another TextPointer.
  // IsAtOrBefore       : evaluate if a TextPointer is at or before another TextPointer
  // IsBefore           : evaluate if a TextPointer is before another TextPointer.
  // PadLineToLength    : Pad the TextPointer located line with blanks. 
  // SetPositionOnLine  : set TextPointer to char index on text line.

  public static class TextPointerExt
  {

    public static TextPointer GetCharPositionOnLine(this TextPointer Start, int CharOx)
    {
      int rem = CharOx;
      TextPointer tp = Start;
      TextPointer charPos = null;
      while (true)
      {
        var pc = tp.GetPointerContext(LogicalDirection.Forward);

        // LineBreak element means the end of the text line. the element the pointer points to is a LineBreak element. This is an end of
        // the line.
        if (pc == TextPointerContext.ElementStart)
        {
          var elem = tp.GetAdjacentElement(LogicalDirection.Forward) as TextElement;
          if ((elem != null) && (elem is LineBreak))
          {
            charPos = null;
            break;
          }
        }

        if (pc == TextPointerContext.Text)
        {
          var lx = tp.GetTextInRun(LogicalDirection.Forward).Length ;
          if ( rem > lx )
            rem -= lx ;
          else
          {
            charPos = tp.GetPositionAtOffset(rem) ;
            break ;
          }
        }

        // Advance to the next context position.
        tp = tp.GetNextContextPosition(LogicalDirection.Forward);
      }

      return charPos;
    }


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
      var p2 = p.GetLineStartPosition(1);
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

    /// <summary>
    /// return the offset to a TextPointer from the start of the Run that the 
    /// TextPointer points to.
    /// </summary>
    /// <param name="Pointer"></param>
    /// <returns></returns>
    public static int? GetOffsetInRun(this TextPointer Pointer)
    {
      Run run = Pointer.Parent as Run;
      if (run == null)
        return null;
      else
      {
        string s1 = Pointer.GetTextInRun(LogicalDirection.Backward);
        int pos = s1.Length;
        return pos;
      }
    }

    public static TextPointer GetPositionAtActualOffset(
      this TextPointer Pointer, int Offset)
    {
      int ox = Offset + 1;
      var p1 = Pointer.GetPositionAtOffset(ox);
      return p1;
    }

    /// <summary>
    /// get position from start of the document to the location of the TextPointer.
    /// </summary>
    /// <param name="Pointer"></param>
    /// <param name="Document"></param>
    /// <returns></returns>
    public static int GetPositionInDocument(
      this TextPointer Pointer, FlowDocument Document)
    {
      if ((Pointer == null) || (Document.ContentStart == null))
        return 0;
      else
      {
        int ox = Document.ContentStart.GetOffsetToPosition(Pointer);
        return ox;
      }
    }

    public static int GetPositionInLine(this TextPointer Pointer)
    {
      TextPointer tp1 = Pointer.GetLineStartPosition(0);
      int ox = tp1.GetOffsetToPosition(Pointer);
      return ox;
    }

    public static IEnumerable<Tuple<TextPointer, TextPointerContext>> SymbolsOnLine(this TextPointer Current)
    {
      var tp = Current.GetNextContextPosition(LogicalDirection.Forward);
      if (tp == null)
        yield break;
      else
      {
        var pc = tp.GetPointerContext(LogicalDirection.Forward);

        if (pc == TextPointerContext.ElementStart)
        {
          var elem = tp.GetAdjacentElement(LogicalDirection.Forward) as TextElement;
          if ((elem != null) && (elem is LineBreak))
          {
            yield break;
          }
        }

        yield return new Tuple<TextPointer, TextPointerContext>(tp, pc);
      }
    }

    public static TextPointer GetPositionLastCharOnLine(this TextPointer Start)
    {
      TextPointer tp = Start;
      TextPointer lastChar = null;

      foreach( var rv in tp.SymbolsOnLine())
      {
        tp = rv.Item1;
        var pc = rv.Item2;

        if (pc == TextPointerContext.Text)
        {
          var lx = tp.GetTextInRun(LogicalDirection.Forward).Length;
          lastChar = tp.GetPositionAtOffset(lx - 1);
        }
      }

      return lastChar;
    }

    /// <summary>
    /// Return the Run the TextPointer points at, and return the position in the Run of
    /// the TextPointer.
    /// </summary>
    /// <param name="Pointer"></param>
    /// <returns></returns>
    public static Tuple<Run, int> GetRunInfo(this TextPointer Pointer)
    {
      Run run = null;
      int pos = 0;

      if (Pointer.Parent is Run)
      {
        run = Pointer.Parent as Run;
        string s1 = Pointer.GetTextInRun(LogicalDirection.Backward);
        pos = s1.Length;
      }

      return new Tuple<Run, int>(run, pos);
    }

    public static string GetRunText(this TextPointer TextPointer)
    {
      string afterText = TextPointer.GetTextInRun(LogicalDirection.Forward);
      string beforeText = TextPointer.GetTextInRun(LogicalDirection.Backward);
      string fullText = beforeText + afterText;

      Run run = TextPointer.Parent as Run;
      if (run != null)
        return run.Text;
      else
        return fullText;
    }

    /// <summary>
    /// Get the text of the text pointer that extends from the pointer until
    /// the end of the line.
    /// </summary>
    /// <param name="From"></param>
    /// <returns></returns>
    public static string GetTextToLineEnd(this TextPointer From)
    {
      var sb = new StringBuilder();

      // Position a "navigator" pointer before the opening tag of the element.
      var tp = From;

      while (true)
      {
        var pc = tp.GetPointerContext(LogicalDirection.Forward);
        var s1 = pc.ToString();

        // the element the pointer points to is a LineBreak element. This is an end of
        // the line.
        if (pc == TextPointerContext.ElementStart)
        {
          var elem = tp.GetAdjacentElement(LogicalDirection.Forward) as TextElement;
          if ((elem != null) && (elem is LineBreak))
            break;
        }

          // accumulate the text the pointer points to.
        else if (pc == TextPointerContext.Text)
          sb.Append(tp.GetTextInRun(LogicalDirection.Forward));

        // Advance to the next context position.
        tp = tp.GetNextContextPosition(LogicalDirection.Forward);
      }

      return sb.ToString();
    }

    /// <summary>
    /// TextPointer is located after the compare to TextPointer.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="CompareToPointer"></param>
    /// <returns></returns>
    public static bool IsAfter(
      this TextPointer TextPointer, TextPointer CompareToPointer)
    {
      if (TextPointer == null)
        return false;
      else if (CompareToPointer == null)
        return false;
      else
      {
        int rv = TextPointer.CompareTo(CompareToPointer);
        if (rv > 0)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// TextPointer is located at or after the compare to TextPointer.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="CompareToPointer"></param>
    /// <returns></returns>
    public static bool IsAtOrAfter(
      this TextPointer TextPointer, TextPointer CompareToPointer)
    {
      if (TextPointer == null)
        return false;
      else if (CompareToPointer == null)
        return false;
      else
      {
        int rv = TextPointer.CompareTo(CompareToPointer);
        if (rv >= 0)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// TextPointer is located before or at location of compare to TextPointer.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="CompareToPointer"></param>
    /// <returns></returns>
    public static bool IsAtOrBefore(
      this TextPointer TextPointer, TextPointer CompareToPointer)
    {
      if (TextPointer == null)
        return false;
      else if (CompareToPointer == null)
        return false;
      else
      {
        int rv = TextPointer.CompareTo(CompareToPointer);
        if (rv <= 0)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// TextPointer is located before the compare to TextPointer.
    /// </summary>
    /// <param name="TextPointer"></param>
    /// <param name="CompareToPointer"></param>
    /// <returns></returns>
    public static bool IsBefore(
      this TextPointer TextPointer, TextPointer CompareToPointer)
    {
      if (TextPointer == null)
        return false;
      else if (CompareToPointer == null)
        return false;
      else
      {
        int rv = TextPointer.CompareTo(CompareToPointer);
        if (rv < 0)
          return true;
        else
          return false;
      }
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
