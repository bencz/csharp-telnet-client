using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.File ;
using AutoCoder.Parse;
using System.Collections;

namespace AutoCoder.Text
{

  // ---------------------------------- Stmt ------------------------------------
  public class Stmt : IEnumerable<StmtElem>
  {
    string mStmtText;

    // in a multi line stmttext string, the startpos of this stmt in that string.
    int mStmtBeginPos = 0;

    StmtTraits mStmtTraits ;
    
    // stmt words are created from the first pass of the stmt text.
    StmtWord mTopWord;
    
    // stmt elements are created on the 2nd pass.  Created from stmt words.
    StmtElem mTopElem;      // the StmtElem which owns all the StmtElem of this stmt. 

    public Stmt( string InStmtText, StmtTraits InTraits, int InStmtBeginPos )
    {
      mStmtText = InStmtText;
      mStmtTraits = InTraits;
      mStmtBeginPos = InStmtBeginPos;
    }

    /// <summary>
    /// Construct the Stmt object from the statement string.
    /// </summary>
    /// <param name="InString"></param>
    public Stmt(string InStmtText)
    {
      StmtTraits traits = new StmtTraits();
      StmtWord topWord = StmtElem.FirstPass(InStmtText, traits);
      StmtWordListCursor csr = topWord.SubWords.PositionBegin( ) ;
      Parse_Common(InStmtText, csr, traits );
    }

    /// <summary>
    /// Construct Stmt object from statement string and TextTraits 
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InTraits">Traits that govern how the statement
    /// is parsed.</param>
    public Stmt( string InStmtText, StmtTraits InTraits )
    {
      StmtWordListCursor csr = FirstPass_Common(InStmtText, InTraits);
      Parse_Common(InStmtText, csr, InTraits);
    }

    public Stmt(TextLines InLines, StmtTraits InTraits)
    {
      string stmtText = InLines.ToString();
      StmtWordListCursor csr = FirstPass_Common(stmtText, InTraits);
      Parse_Common(stmtText, csr, InTraits);
    }

    /// <summary>
    /// Construct the Stmt from string with the cursor position AT the first word
    /// of the statement.
    /// TextTraits are the traits of the first word cursor. 
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InCsr"></param>
    public Stmt(string InStmtText, WordCursor InStartCsr)
    {
      StmtTraits traits = new StmtTraits(InStartCsr.TextTraits);
      StmtWord topWord = StmtElem.FirstPass(InStmtText, InStartCsr, traits);
      StmtWordListCursor wlCsr = topWord.SubWords.PositionBegin();
      Parse_Common(InStmtText, wlCsr, traits);
    }

    #region IEnumerable Members
    // ---------------------------- GetEnumerator --------------------------------------
    IEnumerator<StmtElem> IEnumerable<StmtElem>.GetEnumerator()
    {
      if (mTopElem == null)
        yield break;
      else
      {
        foreach ( StmtElem elem in mTopElem)
        {
          yield return elem;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion

    // ----------------------------- Elements ----------------------------------
    public List<StmtElem> Elements
    {
      get{ return mTopElem.SubElements ; }
    }

    // --------------------------------- Name -------------------------------------
    /// <summary>
    /// the name of the statement ( if it has one )
    /// </summary>
    public string Name
    {
      get
      {
        if (( mTopElem != null ) && ( mTopElem.ElemForm == StmtElemForm.Function ))
          return mTopElem.StartCursor.WordCursor.Word.Value;
        else
          return null;
      }
    }

    public StmtElem TopElem
    {
      get { return mTopElem; }
    }

    public StmtWord TopWord
    {
      get { return mTopWord; }
      set { mTopWord = value; }
    }

    /// <summary>
    /// in a multi line stmttext string, the startpos of this stmt in that string.
    /// </summary>
    public int StmtBeginPos
    {
      get { return mStmtBeginPos; }
    }

    public StmtElemForm StmtForm
    {
      get
      {
        StmtElem stmtElem = GetStmtElem();
        if (stmtElem == null)
          return StmtElemForm.None;
        else
          return stmtElem.ElemForm;
      }
    }

    /// <summary>
    /// Name of the statement. Either the command name or the function name.
    /// </summary>
    public string StmtName
    {
      get
      {
        StmtElem stmtElem = GetStmtElem();
        if (stmtElem == null)
          return null;
        else
        {

          switch (stmtElem.ElemForm)
          {
            case StmtElemForm.Function:
              return stmtElem.StartCursor.WordCursor.Word.Value;
            case StmtElemForm.Command:
              return stmtElem.StartCursor.WordCursor.Word.Value;
            default:
              return null;
          }
        }
      }
    }

    public string StmtParms
    {
      get
      {
        StmtElem stmtElem = GetStmtElem();
        if (stmtElem == null)
          throw new ApplicationException("statement missing single stmt element");
        else if ((stmtElem.ElemForm == StmtElemForm.Function)
          || (stmtElem.ElemForm == StmtElemForm.Command))
        {
          int elemCx = stmtElem.SubElements.Count;
          if (elemCx == 0)
            return "";
          else
          {
            StmtElem fsElem = stmtElem.SubElements[0];
            StmtElem lsElem = stmtElem.SubElements[elemCx - 1];
            int Bx = fsElem.ElemBx;
            int Ex = lsElem.ElemEx;
            int Lx = Ex - Bx + 1;
            return StmtText.Substring(Bx, Lx);
          }
        }
        else throw new ApplicationException("StmtForm does not support parameters");
      }
    }

    public string StmtText
    {
      get { return mStmtText; }
    }

    /// <summary>
    /// TextTraits of the stmt text. 
    /// Can get at any time, have to set in the constructor by supplying 
    /// either a first word cursor ( which contains the traits used to 
    /// identify that word ) or a TextTraits argument.
    /// </summary>
    public StmtTraits StmtTraits
    {
      get { return mStmtTraits; }
    }

    public static bool ElemFormHasSubElements(StmtElemForm InSef)
    {
      switch (InSef)
      {
        case StmtElemForm.Empty:
          return false;
        case StmtElemForm.NumericLiteral:
          return false;
        case StmtElemForm.QuotedLiteral:
          return false;
        case StmtElemForm.Variable:
          return false;
        default:
          return true;
      }
    }

    private StmtWordListCursor FirstPass_Common(string InStmtText, StmtTraits InTraits)
    {
      StmtWord topWord = StmtElem.FirstPass(InStmtText, InTraits);
      StmtWordListCursor csr = topWord.SubWords.PositionBegin();
      return csr;
    }

    private StmtElem GetStmtElem()
    {
      StmtElem stmtElem = TopElem;
      if ((stmtElem != null) && stmtElem.ElemForm == StmtElemForm.TopStmt)
      {
        if (TopElem.SubElements.Count == 0)
          stmtElem = null;
        else if (TopElem.SubElements.Count == 1)
          stmtElem = TopElem.SubElements[0];
        else
          throw new ApplicationException("Stmt contains multiple statements.");
      }
      return stmtElem;
    }

    private void Parse_Common(string InStmtText, StmtWordListCursor InCsr, StmtTraits InTraits)
    {
      StmtWordListCursor csr = null;
      mStmtText = InStmtText;
      mStmtTraits = InTraits;

      csr = InCsr.Next();
      if (csr == null)
        throw new ApplicationException("stmt is empty");

      // the TopStmt holds a collection of statements.
      mTopElem = new StmtElem(this, StmtElemForm.TopStmt, csr);

      // parse the collection of statements.
      csr = mTopElem.Parse();

#if skip
      StmtElemForm sef = StmtElemForm.None;
      sef = StmtElem.CalcElemForm(this, null, csr ) ;
      mTopElem = new StmtElem( this, sef, csr);

      if (Stmt.ElemFormHasSubElements(mTopElem.ElemForm) == true)
      {
        csr = mTopElem.Parse();
      }
#endif

    }

    public static StmtElemForm StmtFormToElemForm(StmtForm InStmtForm)
    {
      switch (InStmtForm)
      {
        case AutoCoder.Text.StmtForm.Function:
          return StmtElemForm.Function;
        case AutoCoder.Text.StmtForm.Assignment:
          return StmtElemForm.ValueExpression;
        case AutoCoder.Text.StmtForm.Boolean:
          return StmtElemForm.BooleanExpression;
        case AutoCoder.Text.StmtForm.Command:
          return StmtElemForm.Command;
        case AutoCoder.Text.StmtForm.CSV:
          return StmtElemForm.CSV;
        case AutoCoder.Text.StmtForm.None:
          return StmtElemForm.None;
        default:
          throw new ApplicationException("unexpected StmtForm " + InStmtForm.ToString());
      }
    }


  }
}
