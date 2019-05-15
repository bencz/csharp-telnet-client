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

    delegate bool delIsMemberEnd( StmtWordListCursor InCsr ) ;

    // parse the member elements of the element.
    // we know the ElemForm and the TopStmt contains the StmtText and StmtTraits.
    // Depending on the ElemForm, advance the WordCursor from word to word in the StmtText
    // and build a StmtElem for each word.
    public StmtWordListCursor Parse()
    {
      StmtWordListCursor csr = null;
      delIsMemberEnd me = null;
      csr = Parse(me);
      return csr;
    }

    StmtWordListCursor Parse(delIsMemberEnd InIsMemberEnd)
    {
      StmtWordListCursor csr = null;
      switch (mElemForm)
      {
        case StmtElemForm.Function:
          csr = Parse_Function(InIsMemberEnd);
          break;
        case StmtElemForm.Sentence:
          csr = Parse_Sentence(InIsMemberEnd);
          break;
        case StmtElemForm.BracedSentence:
          csr = Parse_Sentence(InIsMemberEnd);
          break;
        case StmtElemForm.BracedContent:
          csr = Parse_BracedContent(InIsMemberEnd);
          break;
        case StmtElemForm.CSV:
          csr = Parse_CSV(InIsMemberEnd);
          break;
        case StmtElemForm.Assignment:
          csr = Parse_Assignment(InIsMemberEnd);
          break;
        case StmtElemForm.ValueExpression:
          csr = Parse_ValueExpression(InIsMemberEnd);
          break;
        case StmtElemForm.Command:
          csr = Parse_Command(InIsMemberEnd);
          break;
        case StmtElemForm.QualifiedSequence:
          csr = Parse_QualifiedSequence(InIsMemberEnd);
          break;
        case StmtElemForm.TopStmt:
          csr = Parse_TopStmt(InIsMemberEnd);
          break;
        default:
          throw new ApplicationException("Parse Stmt failed. StmtForm is unsupported.");
      }
      return csr;
    }

    StmtWordListCursor Parse_Assignment(delIsMemberEnd InIsMemberEnd)
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore( WhichEdge.LeadEdge );

      // first word is the lhs of the assignment stmt.
      csr = csr.Next();

      StmtElem lhsElem = new StmtElem(TopStmt, StmtElemForm.lhs, csr);
      AddMemberElem(lhsElem);
  
      // parse the lhs word
      sef = StmtElem.CalcElemForm(null, TopStmt, lhsElem, csr);
      elem = new StmtElem(TopStmt, sef, csr);
      lhsElem.AddMemberElem(elem);
      if ( Stmt.ElemFormHasSubElements( sef ) == true )
      {
        csr = elem.Parse();
      }

      // right hand side. 
      csr = csr.Next();
      if ((csr == null) || (csr.WordCursor.IsEndOfString == true))
        throw new ApplicationException("assignment stmt missing right hand side");
      StmtElem rhsElem = new StmtElem(TopStmt, StmtElemForm.rhs, csr);
      AddMemberElem(rhsElem);

      sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, rhsElem, csr);
      elem = new StmtElem(TopStmt, sef, csr);
      rhsElem.AddMemberElem(elem);
      if ( Stmt.ElemFormHasSubElements( sef ) == true )
      {
        StmtWordListCursor csr2 = null;
        csr2 = elem.Parse();
        if (elem.ElemForm == StmtElemForm.Function)
          csr = csr.Next();
        else
          csr = csr2;
      }
      return csr;
    }

    bool Parse_Assignment_IsMemberDelim(WordCursor InCsr)
    {
      if (InCsr.DelimClass != DelimClassification.DividerSymbol)
        return false;
      else if 
        (TopStmt.StmtTraits.ExpressionPatterns.Contains(InCsr.DelimValue[0]) == true)
        return true;
      else
        return false;
    }

    // braced content consists of a list of statements.
    // examples of braced on content:
    //    string func( arg InFac1 ) { int xx = 5 ; return xx.ToString( ) ; } // two stmts.
    //    char[] nwc = new char[] { 'a', 'b', 'c' } ; //  one stmt.
    StmtWordListCursor Parse_BracedContent(delIsMemberEnd InIsMemberEnd)
    {
      StmtWordListCursor csr = null;
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;

      csr = StartCursor.StmtWord.SubWords.PositionBegin();
      while (true)
      {
        csr = csr.Next();
        if (csr == null)
          break;

        // is the closing brace delim of a function with no args.
        else if (csr.WordCursor.IsDelimOnly == true)
        {
        }

          // function arg sub element.
        else
        {
          sef = StmtElem.CalcElemForm(Parse_Function_IsMemberEnd, TopStmt, this, csr);
          elem = new StmtElem(TopStmt, sef, csr);
          AddMemberElem(elem);

          if (Stmt.ElemFormHasSubElements(sef) == true)
          {
            csr = Parse_SubElement(csr, elem, Parse_Function_IsMemberEnd);
          }
        }

        CloseCursor = csr;
      }
      return csr;
    }

    // parse the Command form element. 
    // return the WordCursor that terminates the element.
    StmtWordListCursor Parse_Command( delIsMemberEnd InIsMemberEnd )
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionAfter(WhichEdge.LeadEdge);
      while (true)
      {
        csr = csr.Next();
        if (( csr == null ) || ( csr.WordCursor.IsEndOfString == true))
          break ;

        sef = StmtElem.CalcElemForm( InIsMemberEnd, TopStmt, this, csr);
        elem = new StmtElem(TopStmt, sef, csr);
        AddMemberElem(elem);

        if (Stmt.ElemFormHasSubElements(elem.ElemForm) == true)
        {
          csr = elem.Parse( Parse_Command_IsMemberEnd );
        }

        // the command stmt ends when an element is not space delimeted.
        if (csr.WordCursor.DelimClass != DelimClassification.Whitespace)
          break;
      }
      return csr;
    }

    // method called via delegate to know when sub element parsing should end because
    // delim is an end of member delim.
    bool Parse_Command_IsMemberEnd(StmtWordListCursor InCsr)
    {
      if (InCsr.WordCursor.DelimIsWhitespace == true)
        return true;
      else
        return false;
    }

    // parse the CSV form element. 
    // return the WordCursor that terminates the element.
    private StmtWordListCursor Parse_CSV( delIsMemberEnd InIsMemberEnd )
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore(WhichEdge.LeadEdge);
      while (true)
      {
        csr = csr.Next();
        if ((csr == null ) || (csr.WordCursor.IsEndOfString == true))
          break;

        sef = StmtElem.CalcElemForm( InIsMemberEnd, TopStmt, this, csr);
        elem = new StmtElem(TopStmt, sef, csr);
        AddMemberElem(elem);

        if (Stmt.ElemFormHasSubElements(elem.ElemForm) == true)
        {
          csr = elem.Parse();
        }

        // the CSV stmt ends when an element is not comma delimeted.
        if (csr.WordCursor.DelimClass != DelimClassification.DividerSymbol)
          break ;
        if ( csr.WordCursor.DelimValue != ",")
          break ;
      }
      return csr;
    }

    bool Parse_CSV_IsAcceptableDelimiter(WordCursor InCsr)
    {
      if (InCsr.DelimClass == DelimClassification.EndOfString)
        return true;
      else if
        ((InCsr.DelimClass == DelimClassification.DividerSymbol) &&
        (InCsr.DelimValue == ","))
        return true;
      else
        return false;
    }

    // parse the function form element. 
    // return the WordCursor that terminates the element.
    StmtWordListCursor Parse_Function( delIsMemberEnd InIsMemberEnd )
    {
      StmtWordListCursor csr = null ;
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;

      csr = StartCursor.StmtWord.SubWords.PositionBegin();
      while (true)
      {
        csr = csr.Next();
        if (csr == null)
          break;

          // new line between function arguments. is ok. treat as whitespace.
        else if ((csr.WordCursor.IsDelimOnly == true)
          && (csr.WordCursor.DelimClass == DelimClassification.NewLine))
        {
        }

        // is the closing brace delim of a function with no args.
        else if (csr.WordCursor.IsDelimOnly == true)
        {
        }

          // function arg sub element.
        else
        {
          sef = StmtElem.CalcElemForm(Parse_Function_IsMemberEnd, TopStmt, this, csr);
          elem = new StmtElem(TopStmt, sef, csr);
          AddMemberElem(elem);

          if (Stmt.ElemFormHasSubElements(sef) == true)
          {
            csr = Parse_SubElement(csr, elem, Parse_Function_IsMemberEnd);
          }
        }

        CloseCursor = csr;
      }
      return csr;
    }

    // method called via delegate to know when sub element parsing should end because
    // delim is an end of member delim.
    bool Parse_Function_IsMemberEnd(StmtWordListCursor InCsr)
    {
      if ((InCsr.WordCursor.DelimClass == DelimClassification.DividerSymbol )
        && ( InCsr.WordCursor.DelimValue == ","))
        return true;
      else if ((InCsr.WordCursor.DelimClass == DelimClassification.DividerSymbol)
        && (InCsr.WordCursor.DelimValue == TopStmt.StmtTraits.FunctionCloseBraceChar))
        return true;
      else
        return false;
    }
    
    // a qualified sequence are consecutive words separated by path delim: aa/bb/cc
    StmtWordListCursor Parse_QualifiedSequence(delIsMemberEnd InIsMemberEnd)
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore(WhichEdge.LeadEdge);
      while (true)
      {
        csr = csr.Next();
        if ((csr == null) || (csr.WordCursor.IsEndOfString == true))
          break;

        sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, this, csr);
        elem = new StmtElem(TopStmt, sef, csr);
        AddMemberElem(elem);

        if (Stmt.ElemFormHasSubElements(elem.ElemForm) == true)
        {
          csr = Parse_SubElement(csr, elem, null);
        }

        if (csr.WordCursor.IsPathPart == false)
          break;
      }
      return csr;
    }

    StmtWordListCursor Parse_Sentence(delIsMemberEnd InIsMemberEnd)
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore( WhichEdge.LeadEdge ) ;
      while (true)
      {
        csr = csr.Next();
        if ((csr == null) || (csr.WordCursor.IsEndOfString == true))
          break;

        sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, this, csr);

        // skip empty words within a sentence ( a newline char sequence )
        if (sef == StmtElemForm.Empty)
        {
        }

        else
        {

          elem = new StmtElem(TopStmt, sef, csr);
          AddMemberElem(elem);

          if (Stmt.ElemFormHasSubElements(elem.ElemForm) == true)
          {
            csr = Parse_SubElement(csr, elem, null);
          }
        }

        // the sentence ends when the delim after the sentence member elem is
        // not a sentence delim.
        if (TopStmt.StmtTraits.IsSentenceDelim(csr.WordCursor.DelimValue) == false)
          break;
      }
      return csr;
    }

    // parse the sub element, then properly position the StmtWordListCursor to the word
    // which follows the sub element.
    StmtWordListCursor Parse_SubElement(
      StmtWordListCursor InElemCursor, StmtElem InSubElem, delIsMemberEnd InIsMemberEnd)
    {
      StmtWordListCursor csr2 = null;
      StmtWordListCursor elemCursor = InElemCursor;

      csr2 = InSubElem.Parse(InIsMemberEnd);

      // if the sub element just parsed is an expression, the words of the
      // expression were inline ( at the same level ) with the words of this
      // Function element. 
      if ((InSubElem.IsExpression == true)
        || (InSubElem.ElemForm == StmtElemForm.Sentence))
        elemCursor = csr2;

        // an assignment stmt. ( should throw if not delim by stmt end delim. )
      else if (InSubElem.ElemForm == StmtElemForm.Assignment)
        elemCursor = csr2;

        // the sub element is a function. the way the words of the sub function are
      // parsed, the closing brace of the sub function will be the delim of the
      // last element of the sub function. 
      // The next word of this owning function ( assuming it is properly formed )
      // will be a delim only word where the delim is either "," or close brace.
      else if ((InSubElem.ElemForm == StmtElemForm.Function)
        || (InSubElem.ElemForm == StmtElemForm.BracedSentence)
        || ( InSubElem.ElemForm == StmtElemForm.BracedContent ))
      {
        elemCursor = elemCursor.Next();
      }

      return elemCursor;
    }

    // parse a collection of statements.
    StmtWordListCursor Parse_TopStmt(delIsMemberEnd InIsMemberEnd)
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore( WhichEdge.LeadEdge );
      while (true)
      {
        csr = csr.Next();
        if ((csr == null) || (csr.WordCursor.IsEndOfString == true))
          break;

        sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, this, csr);

        // skip empty words within the collection of stmts.
        if (sef == StmtElemForm.Empty)
        {
        }

        else
        {
//        sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, this, csr);
          elem = new StmtElem(TopStmt, sef, csr);

          AddMemberElem(elem);

          csr = elem.Parse();
        }
      }
      return csr;
    }

    StmtWordListCursor Parse_ValueExpression(delIsMemberEnd InIsMemberEnd)
    {
      StmtElem elem = null;
      StmtElemForm sef = StmtElemForm.None;
      StmtWordListCursor csr = this.StartCursor.PositionBefore(WhichEdge.LeadEdge);

      // parse the expression until delim that is not an expression symbol
      while (true)
      {
        csr = csr.Next();
        if ((csr == null ) || (csr.WordCursor.IsEndOfString == true))
          break;

        sef = StmtElem.CalcElemForm(InIsMemberEnd, TopStmt, this, csr);
        elem = new StmtElem(TopStmt, sef, csr);
        AddMemberElem(elem);

        if (Stmt.ElemFormHasSubElements(elem.ElemForm) == true)
        {
          elem.Parse();
          csr = csr.Next();
          if (csr == null)
            break;
        }

        // add the expression string as a operator element of the expression parent.
        if ( csr.WordCursor.DelimIsExpressionSymbol == true )
        {
          elem = new StmtElem(TopStmt, StmtElemForm.ExpressionOperator, csr);
          AddMemberElem(elem);
        }
        else
          break;
      }

      return csr;
    }

  }
}
