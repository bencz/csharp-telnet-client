using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Parse;
using System.Collections;

namespace AutoCoder.Text
{
  // the stmt text is stored in the Stmt class.
  // When the Stmt is parsed, StmtElem objects are created for each hierarchical
  // element of the Stmt.

  // Dont confuse StmtWord with StmtElem. StmtWord(s) are the first pass output of
  // the Stmt.Parse process where each StmtWord corresponds to WordCursor words in
  // the stmt text. 
  public partial class StmtElem : IEnumerable<StmtElem>
  {
    StmtElem mParent = null;
    
    // element form tells us what kind of an element this is.
    // function, csv, numeric literal, ... 
    StmtElemForm mElemForm = StmtElemForm.None;

    Stmt mTopStmt = null;

    // the member elements of the element. ( the individual values of a CSV stmt. )
    List<StmtElem> mMembers = null;
    
    // the word cursor that starts the element. in the case of a function elem,
    // the word cursor will encompass the entire elem. in a csv, this is the
    // first value.
    StmtWordListCursor mStartCursor = null;

    // closing cursor identifies the text which follows the last member element of
    // this parent elem. ( ex: the close paren of a function element )
    StmtWordListCursor mCloseCursor = null;

    public StmtElem( Stmt InTopStmt )
    {
      mTopStmt = InTopStmt;
      mParent = null;
      mElemForm = StmtElemForm.None;
      mStartCursor = null;
      mCloseCursor = null;
    }

    public StmtElem( Stmt InTopStmt, StmtElemForm InForm, StmtWordListCursor InStartCursor)
    {
      if (InForm == StmtElemForm.None)
        throw new ApplicationException("StmtElem form is not set");
      mTopStmt = InTopStmt;
      mElemForm = InForm;
      mStartCursor = InStartCursor;
      mCloseCursor = null;
    }

    public StmtElem this[int InIx]
    {
      get
      {
        return SubElements[InIx];
      }
      set
      {
        SubElements[InIx] = value;
      }
    }

    // ------------------------ GetEnumerator ------------------------------------
    IEnumerator<StmtElem> IEnumerable<StmtElem>.GetEnumerator()
    {
      if (mMembers == null)
        yield break;
      else
      {
        foreach (StmtElem elem in mMembers)
        {
          yield return elem;
        }
      }
    }

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion

    public StmtWordListCursor CloseCursor
    {
      get { return mCloseCursor; }
      set { mCloseCursor = value; }
    }

    /// <summary>
    /// the comma delim is a valid member element delim in this element.
    /// </summary>
    public bool CommaIsMemberDelim
    {
      get
      {
        if (ElemForm == StmtElemForm.Function)
          return true;
        else if (ElemForm == StmtElemForm.CSV)
          return true;
        else
          return false;
      }
    }

    public int ElemBx
    {
      get
      {
        if (ElemForm == StmtElemForm.ExpressionOperator)
          return mStartCursor.WordCursor.DelimBx;
        else
          return mStartCursor.WordCursor.ValueBx ;
      }
    }

    /// <summary>
    /// the end position of this element.
    /// </summary>
    public int ElemEx
    {
      get
      {
        int Ex = -1;
        if (mCloseCursor != null)
        {
          if (mElemForm == StmtElemForm.Function)
            Ex = mCloseCursor.WordCursor.DelimEx;
          else
            Ex = mCloseCursor.WordCursor.ValueEx;
        }
        else if (HasSubElements == true)
        {
          StmtElem lsElem = null;
          foreach (StmtElem elem in SubElements)
          {
            lsElem = elem;
          }
          Ex = lsElem.ElemEx;
        }
        else if (ElemForm == StmtElemForm.ExpressionOperator)
          Ex = mStartCursor.WordCursor.DelimEx;
        else
          Ex = mStartCursor.WordCursor.ValueEx;
        return Ex;
      }
    }

    public StmtElemForm ElemForm
    {
      get { return mElemForm; }
      set { mElemForm = value; }
    }

    public bool HasSubElements
    {
      get
      {
        if (mMembers == null)
          return false;
        else if (mMembers.Count == 0)
          return false;
        else
          return true;
      }
    }

    public bool IsExpression
    {
      get
      {
        switch (mElemForm)
        {
          case StmtElemForm.BooleanExpression:
            return true;
          case StmtElemForm.ValueExpression:
            return true;
          default:
            return false;
        }
      }
    }

    public bool IsSentence
    {
      get
      {
        switch (mElemForm)
        {
          case StmtElemForm.Sentence:
            return true;
          case StmtElemForm.BracedSentence:
            return true;
          default:
            return false;
        }
      }
    }

    public StmtElem Parent
    {
      get { return mParent; }
    }

    public StmtElemForm ParentElemForm
    {
      get
      {
        if (Parent == null)
          return StmtElemForm.None;
        else
          return Parent.ElemForm;
      }
    }

    /// <summary>
    /// get the top level Stmt this StmtElem is a member of.
    /// </summary>
    public Stmt TopStmt
    {
      get { return mTopStmt; }
    }

    // ----------------------------- SubElements ----------------------------------
    public List<StmtElem> SubElements
    {
      get
      {
        if (mMembers == null)
          mMembers = new List<StmtElem>();
        return mMembers;
      }
    }

    /// <summary>
    /// the WordCursor this element was created from.
    /// </summary>
    public StmtWordListCursor StartCursor
    {
      get { return mStartCursor; }
    }


    public void AddMemberElem( StmtElem InMbrElem )
    {
      if (mMembers == null)
        mMembers = new List<StmtElem>();
      InMbrElem.mParent = this;
      mMembers.Add(InMbrElem);
    }

    public override string ToString()
    {
      int Lx = ElemEx - ElemBx + 1;
      return TopStmt.StmtText.Substring(ElemBx, Lx);
    }

  } // end class StmtElem

}
