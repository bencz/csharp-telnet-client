using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder ;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Collections;
using AutoCoder.Ext.System;

namespace AutoCoder.Parse
{

  public class StmtWord : ITreeTraverse<StmtWord>
  {
    string mStmtText;
    StmtWord mParent;

    // This StmtWord is a SubWord of some StmtWord in the StmtWord tree ( unless
    // is the top word ). SubWordNode stores the LinkedListNode in the SubWords 
    // LinkedList of the Parent which contains this StmtWord. This node is used
    // when traversing the StmtWord tree and the code needs to advance to the 
    // word which follows of the parent word.
    LinkedListNode<StmtWord> mSubWordNode = null;
    
    WordCursor mWordCursor;

    // the StmtWord(s) this brace enclosed word is composed of.
    StmtWordList mSubWords = null;

    // set during first pass, the first and last of the stmt are set to point
    // to each other. ( those in between are set to null. )
    // note: likely will remove these properties. Should be able to calc begin
    //       and end stmt words from SubWords.
    StmtWord mEndStmtWord = null;
    StmtWord mBeginStmtWord = null;

    WordCompositeCode mCompositeCode = WordCompositeCode.None;

    // depth within the Parent/SubWords hierarchy.
    int mDepth = 0;

    // paren level. When StmtWord is braced, the paren level of the StmtWord is
    // incremented from the paren level of its parent StmtWord.
    int mParenLevel = 0;

    // In the composite braced word, store the position of the closing brace. 
    int mCloseBracePosition = -1;
    WordCursor _CloseBraceCursor = null;

    // WordText converted to upper case. On first reference, cache the ToUpper result,
    // then store here so subsequent references run faster.
    string mUpperWordText = null;

    public StmtWord(string StmtText, StmtWord Parent, WordCursor WordCursor)
    {
      mStmtText = StmtText;
      mSubWords = null;
      mWordCursor = WordCursor;
      this.Parent = Parent;
    }

    public StmtWord(
      string StmtText,
      StmtWord Parent, WordCursor WordCursor, WordCompositeCode CompositeCode )
    {
      mStmtText = StmtText;
      mSubWords = null;
      mWordCursor = WordCursor;
      mCompositeCode = CompositeCode;
      this.Parent = Parent;
    }

    public WordCursor BeginCursor
    {
      get
      {
        if (HasSubWords == false)
          return mWordCursor;
        else if (CompositeCode == WordCompositeCode.Braced)
          return mWordCursor;
        else
          return FirstSubWord.BeginCursor;
      }
    }

    public StmtWord BeginStmtWord
    {
      get { return mBeginStmtWord; }
      set { mBeginStmtWord = value; }
    }

    int _ChildNbr;
    public int ChildNbr
    {
      get { return _ChildNbr; }
      set { _ChildNbr = value; }
    }

    int _ChildCount;
    public int ChildCount
    {
      get { return _ChildCount; }
      set { _ChildCount = value; }
    }

    public WordCursor CloseBraceCursor
    {
      get { return _CloseBraceCursor; }
      set { _CloseBraceCursor = value; }
    }

    public int CloseBracePosition
    {
      get { return mCloseBracePosition; }
      set { mCloseBracePosition = value; }
    }

    public WordCompositeCode CompositeCode
    {
      get { return mCompositeCode; }
      set 
      { 
        mCompositeCode = value;
        CalcParenLevel();
      }
    }

    public DelimClassification DelimClass
    {
      get
      {
        if (IsAtomic == true)
          return mWordCursor.DelimClass;
        else if ((IsBraced( ) == true) && (this.CloseBraceCursor != null))
          return this.CloseBraceCursor.DelimClass;
        else if (IsComposite == true)
          return EndCursor.DelimClass;
        else
          return DelimClassification.None;
      }
    }

    public int Depth
    {
      get { return mDepth; }
      set { mDepth = value; }
    }

    public WordCursor EndCursor
    {
      get
      {
        if (HasSubWords == false)
          return mWordCursor;
        else if (IsBraced() == true)
          return this.CloseBraceCursor;
        else
        {
          return LastSubWord.EndCursor;
        }
      }
    }

    public StmtWord EndStmtWord
    {
      get { return mEndStmtWord; }
      set { mEndStmtWord = value; }
    }

    public StmtWord FirstSubWord
    {
      get
      {
        if (HasSubWords == false)
          return null;
        else
          return SubWords.First.Value;
      }
    }

    public StmtWord GrandParent
    {
      get
      {
        if (Parent == null)
          return null;
        else
          return Parent.Parent;
      }
    }

    public bool HasSubWords
    {
      get
      {
        if (mSubWords == null)
          return false;
        else if (mSubWords.Count == 0)
          return false;
        else
          return true;
      }
    }

    /// <summary>
    /// Word has CompositeCode Atom
    /// </summary>
    public bool IsAtomic
    {
      get
      {
        if (mCompositeCode == WordCompositeCode.Atom)
          return true;
        else
          return false;
      }
    }

    public bool IsBraced( )
    {
      return (mCompositeCode == WordCompositeCode.Braced); 
    }

    public bool IsBraced(string InOpenBracePattern)
    {
      bool isBraced = false;

      if (mCompositeCode == WordCompositeCode.Braced)
      {
        if (BeginCursor.Word.Value.Contains(InOpenBracePattern))
          isBraced = true;
      }

      return isBraced;
    }

    public bool IsComposite
    {
      get
      {
        if (mCompositeCode == WordCompositeCode.Sentence)
          return true;
        else if (mCompositeCode == WordCompositeCode.Braced)
          return true;
        else if (mCompositeCode == WordCompositeCode.General)
          return true;
        else
          return false;
      }
    }

    public bool IsSentence
    {
      get
      {
        if (mCompositeCode == WordCompositeCode.Sentence)
          return true;
        else
          return false;
      }
    }

    public bool IsTopWord
    {
      get
      {
        if (mParent == null)
          return true;
        else
          return false;
      }
    }

    public StmtWord LastSubWord
    {
      get
      {
        if (HasSubWords == false)
          return null;
        else
          return SubWords.Last.Value;
      }
    }

    public StmtWord Next( )
    {
        if (SubWordNodeIsAssigned == false)
          return null;
        else if (SubWordNode.Next != null)
          return SubWordNode.Next.Value;
        else
          return null;
    }

    public StmtWord NextDeep(bool InitialSkipDeep = false)
    {
      if ((InitialSkipDeep == false) && (this.HasSubWords == true))
      {
        var nx = this.SubWords.First.Value;
        return nx;
      }
      else if (NextSibling( ) != null)
        return NextSibling( );
      else if (this.Parent == null)
        return null;
      else
        return this.Parent.NextDeep(InitialSkipDeep: true);
    }

    /// <summary>
    /// Does the close braced delim implicitly or explicitly end this word. 
    /// If the word is braced and this close brace corresponds with the open brace of
    /// the word, then this brace is owned by the word.
    /// </summary>
    public bool? OwnsCloseBracedDelim
    {
      get
      {
        if (this.DelimClass == DelimClassification.CloseBraced)
        {
          if (this.CompositeCode == WordCompositeCode.Braced)
            return true;
          else
            return false;
        }
        else
          return null;
      }
    }

    public StmtWord Parent
    {
      get { return mParent; }
      set
      {
        // this StmtWord already has a parent. Remove this StmtWord from the list
        // of SubWords of the parent.
        if (mParent != null)
        {
          mParent.SubWords.Remove(this);
          mSubWordNode = null;
        }
         
        mParent = value;

        if (mParent != null)
        {
          mSubWordNode = mParent.SubWords.AddLast(this);
          Depth = mParent.Depth + 1;

          // assign the child number within parent.
          mParent.ChildCount += 1;
          this.ChildNbr = mParent.ChildCount;
        }
        CalcParenLevel();
      }
    }

    public int ParenLevel
    {
      get { return mParenLevel; }
      set { mParenLevel = value; }
    }

    public string ShowWordText(int Length)
    {
      string s1 = this.WordText.SubstringLenient(0, Length);
      string s2 = s1.Replace(Environment.NewLine, "<CRLF>");
      if (this.WordText.Length > Length)
        s2 = s2 + " ...";
      return s2;
    }

    public string StmtText
    {
      get { return mStmtText; }
    }

    public string[] StmtTextAsLines
    {
      get
      {
        string[] lines = this.StmtText.Split(
          new string[] { Environment.NewLine }, StringSplitOptions.None);
        return lines;
      }
    }

    public LinkedListNode<StmtWord> SubWordNode
    {
      get
      {
        if (mSubWordNode == null)
          throw new ApplicationException("SubWordNode is not assigned");
        return mSubWordNode; 
      }
      set { mSubWordNode = value; }
    }

    public bool SubWordNodeIsAssigned
    {
      get
      {
        if ( mSubWordNode == null )
          return false ;
        else
          return true ;
      }
    }

    public StmtWordList SubWords
    {

      // need to implicity create SubWords list on a get reference inorder for the 
      // following code to work:
      //      InParentWord.SubWords.AddLast(word);
      get
      {
        if (mSubWords == null)
          mSubWords = new StmtWordList();
        return mSubWords;
      }
    }

    public override string ToString()
    {
      return this.CompositeCode.ToString( ) + " " + this.WordText;
    }

    public string UpperWordText
    {
      get
      {
        if (mUpperWordText == null)
          mUpperWordText = WordText.ToUpper();
        return mUpperWordText;
      }
    }

    public WordClassification WordClassification
    {
      get
      {
        if ( IsAtomic == true )
          return mWordCursor.WordClassification ;
        else if ( IsSentence == true )
          return WordClassification.Sentence ;
        else
        {
          WordClassification wc = BeginCursor.WordClassification ;
          switch( wc )
          {
            case WordClassification.CloseBraced:
            case WordClassification.OpenNamedBraced:
            case WordClassification.OpenContentBraced:
              return WordClassification.Braced ;
          }
        }
        return WordClassification.None ;
      }
    }

    public WordCursor WordCursor
    {
      get
      {
        if (IsAtomic == true)
          return mWordCursor;
        else
          return null;
      }
    }

    public string WordText
    {
      get
      {
        if (HasSubWords == false)
        {
          if (mWordCursor.IsDelimOnly == true)
            return "";
          else if (mWordCursor.Word == null)
            return "";
          else
            return mWordCursor.Word.Value;
        }
        else
        {
          int bx = BeginCursor.WordBx;
          int ex = WordTextEx;
          if (ex == -1)
            return mStmtText.SubstringLenient(bx, 10) + " ...";
          else
          {
            int lx = ex - bx + 1;
            return mStmtText.Substring(bx, lx);
          }
        }
      }
    }

    public int WordTextEx
    {
      get
      {
        if (HasSubWords == false)
          return EndCursor.WordEx;
        else if (CompositeCode == WordCompositeCode.Braced)
          return CloseBracePosition;
        else if (CompositeCode == WordCompositeCode.Sentence)
          return LastSubWord.WordTextEx;
        else
          return LastSubWord.WordTextEx;
      }
    }

    StmtWord _TopNode ;
    public StmtWord TopNode
    {
      get
      {
        if (( _TopNode != null ) && ( _TopNode.Parent != null ))
          _TopNode = null ;
        if ( _TopNode == null )
        {
          if ( mParent == null )
            _TopNode = this ;
          else
          {
            _TopNode = mParent.TopNode ;
          }
        }
        return _TopNode ;
      }
    }

    int? _TopNodeCount;
    public int? TopNodeCount
    {
      get
      {
        return _TopNodeCount ;
      }
      set { _TopNodeCount = value ; }
    }

    public int IncTopNodeCount()
    {
      StmtWord tn = this.TopNode;
      if (tn.TopNodeCount == null)
        tn.TopNodeCount = 1;
      tn.TopNodeCount = tn.TopNodeCount.Value + 1;
      return tn.TopNodeCount.Value;
    }

    int? _NodeNumber;
    public int? NodeNumber
    {
      get
      {
        if ( _NodeNumber == null )
        {
          _NodeNumber = IncTopNodeCount();
        }
        return _NodeNumber ;
      }
    }

    private void CalcParenLevel()
    {
      if (mParent != null)
      {
        if (mCompositeCode == WordCompositeCode.Braced)
          mParenLevel = mParent.ParenLevel + 1;
        else
          mParenLevel = mParent.ParenLevel;
      }
      else
      {
        mParenLevel = 0;
      }
    }

    public bool IsDeepChildOf(StmtWord InWord)
    {
      bool rv = false;

      StmtWord w1 = this;
      while (true)
      {
        if (w1.Parent == null)
          break;

        if (w1.Parent == InWord)
        {
          rv = true;
          break;
        }

        // this node has same parent as arg node. is not a child of.
        if (w1.Parent == InWord.Parent)
          break;

        w1 = w1.Parent;
      }

      return rv;
    }

    /// <summary>
    /// this word is a direct child of the argument word.
    /// </summary>
    /// <param name="InWord"></param>
    /// <returns></returns>
    public bool IsChildOf(StmtWord InWord)
    {
      if (this.Parent == InWord)
        return true;
      else
        return false;
    }

    /// <summary>
    /// This word and the argument word have the same parent word.
    /// </summary>
    /// <param name="InWord"></param>
    /// <returns></returns>
    public bool IsSiblingOf(StmtWord InWord)
    {
      if (this.Parent == InWord.Parent)
        return true;
      else
        return false;
    }

    #region ITreeTraverse interface

    public LinkedList<StmtWord> Children
    {
      get { return mSubWords; } 
    }

    public StmtWord NextSibling()
    {
      if (SubWordNodeIsAssigned == false)
        return null;
      else if (SubWordNode.Next != null)
        return SubWordNode.Next.Value;
      else
        return null;
    }

    public StmtWord FirstChild
    {
      get
      {
        return this.FirstSubWord;
      }
    }

    #endregion
  }
}
