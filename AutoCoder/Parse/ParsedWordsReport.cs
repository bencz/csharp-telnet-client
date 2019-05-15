using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.Parse
{
  using StmtWordCursor = AutoCoder.Collections.TreeCursor<StmtWord>;

  public class ParsedWordsReport : List<string>
  {

    void AddReport(ParsedWordsReport InReport)
    {
      foreach (string s1 in InReport)
      {
        this.AddTextLine(s1);
      }
    }

    void AddSepLine()
    {
      string s1 = BuildSepLine();
      AddTextLine(s1); 
    }

    void AddTextLine(string InText)
    {
      // skip if the 2nd consecutive sep line.
      bool isDup = false;

      if (InText == BuildSepLine())
      {
        if (this.Count > 0)
        {
          string s2 = this[this.Count - 1];
          if (InText == s2)
            isDup = true;
        }
      }

      if (isDup == false)
      {

        // replace any NewLine patterns in the text with blanks.
        string s3 = InText.Replace(Environment.NewLine, "  ");
       
        this.Add(s3);
      }
    }

    static string BuildLegend()
    {
      bool oeFlag = false;
      StringBuilder sb = new StringBuilder();
      for (int ix = 0; ix < 14; ++ix)
      {
        if (oeFlag == false)
          sb.Append("x    ");
        else
          sb.Append("-    ");
        oeFlag = !(oeFlag);
      }
      return sb.ToString();
    }

    static string BuildSepLine()
    {
      return "- - - - - - - - -";
    }

    // -------------------------- CursorToPresentationString -----------------------
    static string CursorToPresentationString(StmtWordCursor Cursor)
    {
      StringBuilder sb = new StringBuilder();

      // the StmtWord the cursor is at.
      if ((Cursor.Position != RelativePosition.At) &&
        ( Cursor.Position != RelativePosition.After))
        throw new ApplicationException("Cursor must be positioned at a StmtWord");
      StmtWord sw = Cursor.Node;

      sb.Append(sw.Depth.ToString() + " " + sw.CompositeCode.ToString());

      // the trailing edge of a composite word
      if (Cursor.Position == RelativePosition.After)
        sb.Append(" TrailingEdge.") ;

      // nbr of sub words
      if (sw.HasSubWords == true)
        sb.Append(" " + "wCx:" + sw.SubWords.Count.ToString());

      // the delimeter of the word is found in the EndCursor of the word.
      if ((sw.IsComposite == false) || (Cursor.Position == RelativePosition.After))
      {
        if (sw.EndCursor != null)
          sb.Append(" " + sw.EndCursor.ToDelimPresentationString());
      }

      // word classification
      if ((sw.IsComposite == false) || (Cursor.Position == RelativePosition.At ))
      {
        if (sw.WordCursor != null)
        {
          sb.Append(" wc: " + sw.WordCursor.WordClassification.ToString());
        }
      }

      // the text of the word
      if ((sw.IsComposite == false) || (Cursor.CompositeNodeEdge == WhichEdge.LeadEdge))
      {
        string showText = null;
        if (sw.WordText.Length < 60)
          showText = sw.WordText;
        else
          showText = sw.WordText.Substring(0, 60) + "...";
        sb.Append(" wt: " + showText) ;
      }

      return sb.ToString();
    }

    // ----------------------- ReportParsedWords -------------------------------
    public static ParsedWordsReport ReportParsedWords(List<Stmt> InStmts)
    {
      ParsedWordsReport r0 = new ParsedWordsReport();
      foreach (Stmt stmt in InStmts)
      {
        ParsedWordsReport r1 = ReportParsedWords(stmt);
        r0.AddReport(r1);
      }
      return r0;
    }

    // ----------------------- ReportParsedWords -------------------------------
    public static ParsedWordsReport ReportParsedWords(Stmt InStmt)
    {
      ParsedWordsReport r1;
      int lvl = 0;
      StmtWord parentWord = InStmt.TopWord;
      r1 = ReportParsedWords(InStmt, parentWord, lvl);
      return r1;
    }

    // ----------------------- ReportParsedWords -------------------------------
    public static ParsedWordsReport ReportParsedWords(StmtWord TopWord)
    {
      ParsedWordsReport r1 = new ParsedWordsReport();

      StmtWordCursor c1 = new StmtWordCursor(TopWord);

      r1.AddTextLine(BuildLegend());

      // show the first 5 lines of the stmt text.
      {
        string s1 =
          TopWord.StmtText.SubstringLenient(TopWord.BeginCursor.WordBx, 500);
        string[] lines = 
          s1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        int cx = 0;
        int bx = 0;
        foreach (string s2 in lines)
        {
          if (s2.IsNullOrEmpty() == false)
          {
            r1.AddTextLine(bx.ToString( ) + ") " + s2);

            cx += 1;
            if (cx > 5)
              break;
          }
          bx += (s2.Length + Environment.NewLine.Length);
        }

        r1.AddSepLine();
      }

      StmtWord w1 = null;
      StmtWord pvWord = null;
      while (true)
      {
        pvWord = w1;

        c1 = c1.NextDeep();
        if ( c1 == null )
          break;
        w1 = c1.Node;

        if ((pvWord != null) && (pvWord.Depth != w1.Depth))
          r1.AddSepLine();

        r1.AddTextLine(CursorToPresentationString(c1));
      }

      return r1;
    }

    // ----------------------- ReportParsedWords -------------------------------
    public static ParsedWordsReport ReportParsedWords(
      StmtWord InParentWord, int InLevel)
    {
      ParsedWordsReport r1 = new ParsedWordsReport();
      StmtWord parentWord = InParentWord;

      if (InLevel == 0)
      {
        r1.AddTextLine(BuildLegend());

        // show the stmt text ( up until any NewLine pattern in the string )
        string s1 = 
          parentWord.StmtText.SubstringLenient(parentWord.BeginCursor.WordBx, 100);
        int fx = s1.IndexOf(Environment.NewLine);
        if (fx != -1)
          s1 = s1.Substring(0, fx);
        r1.AddTextLine(s1);

        r1.AddTextLine(StmtWordToPresentationString(parentWord, InLevel));

        ParsedWordsReport r2 = null;
        r2 = ReportParsedWords(parentWord, InLevel + 1);
        r1.AddReport(r2);
      }

      else
      {
        r1.AddSepLine();

        foreach (StmtWord subWord in parentWord.SubWords)
        {
          r1.AddTextLine(StmtWordToPresentationString(subWord, InLevel));

          if (subWord.HasSubWords == true)
          {
            var r3 = ReportParsedWords(subWord, InLevel + 1);
            r1.AddReport(r3);
          }
        }
        r1.AddSepLine();
      }
      return r1;
    }

    // ----------------------- ReportParsedWords -------------------------------
    public static ParsedWordsReport ReportParsedWords(
      Stmt InStmt, StmtWord InParentWord, int InLevel)
    {
      ParsedWordsReport r1 = new ParsedWordsReport();
      StmtWord parentWord = InParentWord;

      if (InLevel == 0)
      {
        r1.AddTextLine(BuildLegend());

        // show the stmt text ( up until any NewLine pattern in the string )
        string s1 = InStmt.StmtText.SubstringLenient(InStmt.StmtBeginPos, 100);
        int fx = s1.IndexOf(Environment.NewLine);
        if (fx != -1)
          s1 = s1.Substring(0, fx);
        r1.AddTextLine(s1);

        r1.AddTextLine( StmtWordToPresentationString(parentWord, InLevel));

        ParsedWordsReport r2 = null;
        r2 = ReportParsedWords(InStmt, parentWord, InLevel + 1);
        r1.AddReport(r2);
      }

      else
      {
        r1.AddSepLine();

        foreach (StmtWord subWord in parentWord.SubWords)
        {
          r1.AddTextLine(StmtWordToPresentationString(subWord, InLevel));

          if (subWord.HasSubWords == true)
          {
            var r3 = ReportParsedWords(InStmt, subWord, InLevel + 1);
            r1.AddReport(r3);
          }
        }
        r1.AddSepLine();
      }
      return r1;
    }

    // -------------------------- StmtWordToPresentationString -----------------------
    static string StmtWordToPresentationString(StmtWord InWord, int InLevel)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(InLevel.ToString() + " " + InWord.CompositeCode.ToString());

      // nbr of sub words
      if (InWord.HasSubWords == true)
        sb.Append(" " + "wCx:" + InWord.SubWords.Count.ToString());

      // the delimeter of the word is found in the EndCursor of the word.
      if (InWord.EndCursor != null)
        sb.Append(" " + InWord.EndCursor.ToDelimPresentationString());

      // word classification
      if (InWord.WordCursor != null)
      {
        sb.Append(" wc: " + InWord.WordCursor.WordClassification.ToString());
      }

      // the text of the word
      sb.Append(" wt: " + InWord.WordText);

      return sb.ToString();
    }


  }
}
