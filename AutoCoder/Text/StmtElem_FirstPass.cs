using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Parse;
using AutoCoder.Text.Enums;

namespace AutoCoder.Text
{
  public partial class StmtElem
  {

    public static StmtWord FirstPass(string InStmtText, StmtTraits InTraits)
    {
      StmtWord topWord = new StmtWord(InStmtText, null, null);

      WordCursor csr = Scanner.PositionBeginWord(InStmtText, InTraits);
      FirstPass(InStmtText, InTraits, csr, topWord);

      return topWord;
    }

    // build the first pass StmtWord tree starting from a WordCursor position in the
    // stmt text.
    public static StmtWord FirstPass(
      string InStmtText, WordCursor InStartCsr, StmtTraits InTraits)
    {
      WordCursor csr = null;
      StmtWord topWord = new StmtWord(InStmtText, null, null);
      csr = Scanner.PositionBeforeWord(InStartCsr);
      FirstPass(InStmtText, InTraits, csr, topWord);
      return topWord;
    }

    /// <summary>
    /// Parse the text of the current stmt element until the closing brace of the element 
    /// or end of string.
    /// </summary>
    /// <param name="InStmtText"></param>
    /// <param name="InTraits"></param>
    /// <param name="InCsr"></param>
    /// <param name="InParentWord"></param>
    /// <returns></returns>
    public static WordCursor FirstPass(
      string InStmtText, StmtTraits InTraits, WordCursor InCsr, StmtWord InParentWord)
    {
      WordCursor csr = InCsr;
      StmtWord fsWord = null;

      while (true)
      {
        csr = Scanner.ScanNextWord(InStmtText, csr);

        StmtWord word = new StmtWord(InStmtText, InParentWord, csr);
        InParentWord.SubWords.AddLast(word);

        if (csr.IsEndOfString == true)
          break;

        // this word is start of stmt.
        if (fsWord == null)
          fsWord = word;

        // the EndStmt delim is considered to seperate stmts within this parent
        // StmtElem. Since we have saved the reference to the first word of the
        // parent, the first and last words of the stmt can be marked. 
        if (csr.DelimClass == DelimClassification.EndStmt)
        {
          if (fsWord != null)
          {
            fsWord.BeginStmtWord = fsWord;
            fsWord.EndStmtWord = word;

            word.BeginStmtWord = fsWord;
            word.EndStmtWord = word;
          }
          fsWord = null;
        }

        // word is braced ( a function ). collect all the words within the braces.
        if ((csr.WordClassification == WordClassification.OpenNamedBraced)
          || ( csr.WordClassification == WordClassification.OpenContentBraced))
        {
          csr = FirstPass(InStmtText, InTraits, csr, word);

          // cursor is located at the closing brace. We want the word after the closing
          // brace to always be a delim only word. In a parent where members are delimed
          // by comma this is no problem. But in a whitespace sep list, this might not
          // be the case without a little helpful adjustment.
          csr = Scanner.ScanNextWord(InStmtText, csr);
          csr.StayAtFlag = true;

          if (csr.IsDelimOnly == true)
          { }
          else if ((csr.WordClassification == WordClassification.OpenNamedBraced)
            || (csr.WordClassification == WordClassification.OpenContentBraced))
          {
            csr.SetVirtualCursor_WhitespaceOnly(csr.WordBx - 1);
          }
        }

          // todo: have to expand this throw exception when the closing brace does
        //       not match the open brace.
        else if ( csr.DelimClass == DelimClassification.CloseBraced )
        {
          break;
        }

      }
      return csr;
    }
  }
}

