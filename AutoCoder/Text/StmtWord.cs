using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder ;

namespace AutoCoder.Text
{

  public class StmtWord
  {
    string mStmtText;
    StmtWord mParent;
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

    public StmtWord(string InStmtText, StmtWord InParent, WordCursor InWordCursor)
    {
      mStmtText = InStmtText;
      mSubWords = null;
      mWordCursor = InWordCursor;
      Parent = InParent;
    }

    public StmtWord(
      string InStmtText,
      StmtWord InParent, WordCursor InWordCursor, WordCompositeCode InCompositeCode )
    {
      mStmtText = InStmtText;
      mSubWords = null;
      mWordCursor = InWordCursor;
      mCompositeCode = InCompositeCode;
      Parent = InParent;
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
        }
         
        mParent = value;

        if (mParent != null)
        {
          mParent.SubWords.AddLast(this);
          Depth = mParent.Depth + 1;
        }
        CalcParenLevel();
      }
    }

    public int ParenLevel
    {
      get { return mParenLevel; }
      set { mParenLevel = value; }
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

    public WordCursor WordCursor
    {
      get { return mWordCursor; }
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

    public string ToWordText( )
    {
      if (HasSubWords == false)
      {
        if (WordCursor.IsDelimOnly == true)
          return "";
        else
          return WordCursor.Word.Value;
      }
      else
      {
        int bx = BeginCursor.WordBx;
        int ex = WordTextEx;
        int lx = ex - bx + 1;
        return mStmtText.Substring(bx, lx);
      }
    }

  }
}
