using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Parse;
using AutoCoder.Core;
using AutoCoder.Text.Enums;

namespace AutoCoder.Text
{
  public partial class StmtElem
  {

    public static StmtElemForm CalcElemForm(
      Stmt InTopStmt, StmtElem InParent,
      StmtWordListCursor InCsr)
    {
      StmtElemForm sef = StmtElemForm.None;
      delIsMemberEnd me = null;
      sef = CalcElemForm(me, InTopStmt, InParent, InCsr);
      return sef;
    }

    // If start of function, must scan to close function and the advance to
    // delim that follows the function.

    // Could be standalone function or the function could be member of 
    // expression element ( or any other stmt level element. a sentence being most common. )

    // When parent elem is an expression, dont return an expression char delimeted
    // elem as start of new expression.

    // note: no need to be error checking the delimeter at this point.
    // The parent element parsing code will do that. Here we just say that 
    // if the delim is expected within the parent elem the elem is standalone.
    // otherwise, the elem is consider a stmt within the stmt.
    static StmtElemForm CalcElemForm(
      delIsMemberEnd InIsMemberEnd,
      Stmt InTopStmt, StmtElem InParent,
      StmtWordListCursor InCsr)
    {
      StmtElemForm sef = StmtElemForm.None;


      // the cursor StmtWord has sub words. that is, it is braced. elem form could be
      // function, could be expression, ...
      if (InCsr.StmtWord.HasSubWords == true)
      {

        // the next word in the string.
        // note: since the word is braced, the next word is the delim only word which
        //       follows the closing brace. ( see StmtElem_FirstPass processing ).
        StmtWordListCursor nxCsr = InCsr.Next();

        sef = CalcElemForm_Actual(InIsMemberEnd, InTopStmt, InParent, InCsr, nxCsr.WordCursor);
      }

      else
      {
        sef = CalcElemForm_Actual(InIsMemberEnd, InTopStmt, InParent, InCsr, InCsr.WordCursor);
      }


      return sef;
    }

    // calc the elem form based on the isolated delim of the element.
    // ( see the caller. the isolated delim is either the direct delim of the
    //   word in the string, or is the delim that follows a braced word. )
    static StmtElemForm CalcElemForm_Actual(
      delIsMemberEnd InIsMemberEnd,
      Stmt InTopStmt, StmtElem InParent,
      StmtWordListCursor InCsr,
      WordCursor InDelimCursor)
    {
      StmtElemForm sef = StmtElemForm.None;

      // how to handle InParent when InParent is ElemForm == StmtElemForm.TopStmt?

      // actual end of text of string.
      if ((InDelimCursor.DelimClass == DelimClassification.EndOfString)
        || (InTopStmt.StmtTraits.IsEndStmtDelim(InDelimCursor.DelimValue) == true))
      {
        if ((InParent != null) && ( InParent.ElemForm != StmtElemForm.TopStmt )) 
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else if (InTopStmt.StmtTraits.HasSentenceDelimStrings == true )
          sef = StmtElemForm.Sentence;
        else
          sef = StmtElemForm.Command;
      }

        // lhs of assignment statement.
      else if (InDelimCursor.DelimIsAssignmentSymbol == true)
      {
        if (InParent == null)
          sef = StmtElemForm.Assignment;
        else if ((InParent.ElemForm == StmtElemForm.lhs)
          || ( InParent.ParentElemForm == StmtElemForm.lhs ))
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.Assignment;
      }

        // a qualified name ( a directory path )
      else if (InDelimCursor.IsPathPart == true)
      {
        if (InParent == null)
          sef = StmtElemForm.QualifiedSequence;
        else if (InParent.ElemForm == StmtElemForm.QualifiedSequence)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.QualifiedSequence;
      }

      else if ((InDelimCursor.DelimIsPathPart == true) 
        && ( InDelimCursor.WhitespaceFollowsWord == true )) 
      {
        if (InParent == null)
          sef = StmtElemForm.Command;
        else if (InParent.ElemForm == StmtElemForm.Command)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.Command;
      }

        // the delim of the StmtWord is an expression symbol. this word is either the
      // start of an expression, or a word within an expression.
      // ( for now, assume is a value expression. if turns out to be boolean, should
      // be able to change the elem form of the expression element. )
      else if (InDelimCursor.DelimIsExpressionSymbol == true)
      {
        if (InParent == null)
          sef = StmtElemForm.ValueExpression;
        else if (InParent.IsExpression == true)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.ValueExpression;
      }

      // word is part of a space delim command string
      // note: have to set the CommandCapable switch in StmtTraits in order to parse
      //       commands.
      else if ((InTopStmt.StmtTraits.CommandCapable == true)
        && ( InDelimCursor.DelimIsWhitespace == true ))
      {
        if (InParent == null)
          sef = StmtElemForm.Command;
        else if (InParent.ElemForm == StmtElemForm.Command)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.Command;
      }

        // sentence delimiter. 
      else if (InTopStmt.StmtTraits.IsSentenceDelim(InDelimCursor.DelimValue) == true)
      {
        sef = CalcElemForm_Actual_Sentence(
          InIsMemberEnd, InTopStmt, InParent, InCsr, InDelimCursor);
      }

      // comma seperated values
      else if (InDelimCursor.DelimValue == ",")
      {
        if (InParent == null)
          sef = StmtElemForm.CSV;
        else if (InParent.CommaIsMemberDelim)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else if ((InParent.Parent != null)
          && (InParent.Parent.CommaIsMemberDelim))
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          sef = StmtElemForm.CSV;
      }

      else if (InTopStmt.StmtTraits.IsFunctionCloseBrace(InDelimCursor.DelimValue) == true)
      {
        if (InParent == null)
          throw new ApplicationException("unexpected close brace char");
        else if (InParent.ElemForm == StmtElemForm.Function)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else if ((InParent.ElemForm == StmtElemForm.Sentence)
          && (InParent.ParentElemForm == StmtElemForm.Function))
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          throw new ApplicationException("unexpected function close brace char");
      }

      else if (InTopStmt.StmtTraits.IsSentenceCloseBraceChar(InDelimCursor.DelimValue) == true)
      {
        if (InParent == null)
          throw new ApplicationException("unexpected close brace char");
        else if (InParent.ElemForm == StmtElemForm.Sentence)
          sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
        else
          throw new ApplicationException("unexpected function close brace char");
      }

      return sef;
    }

    // calc the elem form based on the isolated delim of the element.
    // ( see the caller. the isolated delim is either the direct delim of the
    //   word in the string, or is the delim that follows a braced word. )
    static StmtElemForm CalcElemForm_Actual_Sentence(
      delIsMemberEnd InIsMemberEnd,
      Stmt InTopStmt, StmtElem InParent,
      StmtWordListCursor InCsr,
      WordCursor InDelimCursor)
    {
      StmtElemForm sef = StmtElemForm.None;

      if ((InParent != null) && (InParent.IsSentence == true))
        sef = CalcElemForm_Standalone(InTopStmt, InParent, InCsr);
      else
      {
        StmtWordListCursor c2 = null;
        sef = StmtElemForm.Sentence;

        // lookahead in the stmt string. look to see if the sentence ends with
        // a braced section or not.
        c2 = InCsr.PositionBefore( WhichEdge.LeadEdge );
        while (true)
        {
          c2 = c2.Next();
          if (c2 == null)
            break;

          if (c2.StmtWord.HasSubWords == true)
          {
            if (InTopStmt.StmtTraits.IsSentenceOpenBraceChar(c2.WordCursor.DelimValue) == true)
            {
              sef = StmtElemForm.BracedSentence;
              break;
            }

            c2 = c2.Next();
          }

          // word is an assignment symbol. stmt is not a sentence. it is an assignment.
          if ((InParent != null) && (InParent.ElemForm != StmtElemForm.lhs))
          {
            if (c2.WordCursor.DelimIsAssignmentSymbol == true)
            {
              sef = StmtElemForm.Assignment;
              break;
            }
          }

          // last word in the sentence.
          if (InTopStmt.StmtTraits.IsSentenceDelim(c2.WordCursor.DelimValue) == false)
            break;
        }
      }
      return sef;
    }

    // calc the form of the element without the context of the delim that separates it
    // from the next element, and without consideration of the parent elem.  
    private static StmtElemForm CalcElemForm_Standalone(
      Stmt InTopStmt, StmtElem InParent,
      StmtWordListCursor InCsr)
    {
      StmtElemForm form = StmtElemForm.None;
      WordCursor wc = InCsr.WordCursor;
      if (wc.IsDelimOnly == true)
        form = StmtElemForm.Empty;
      else if (wc.WordClassification == WordClassification.OpenNamedBraced)
      {
        form = StmtElemForm.Function;
      }
      else if (wc.WordClassification == WordClassification.OpenContentBraced)
        form = StmtElemForm.BracedContent;
      else if (wc.WordClassification == WordClassification.Identifier)
        form = StmtElemForm.Variable;
      else if (wc.WordClassification == WordClassification.Quoted)
        form = StmtElemForm.QuotedLiteral;
      else if (wc.WordClassification == WordClassification.Numeric)
        form = StmtElemForm.NumericLiteral;
      else
        throw new ApplicationException(
          "Cannot calc StmtElem form of word " +
          wc.Word.ToString() + " starting at position " +
          wc.WordBx.ToString());
      return form;
    }
  }
}
