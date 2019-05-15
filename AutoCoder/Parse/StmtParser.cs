using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.File;
using AutoCoder.Core;
using AutoCoder.Collections;
using AutoCoder.Text.Enums;

namespace AutoCoder.Parse
{
	using StmtWordCursor = TreeCursor<StmtWord>;

	public static class StmtParser
  {

    /// <summary>
    /// This is the central method where the cracking of statement text into a hierarchy
    /// of StmtWord(s) takes place.
    /// </summary>
    /// <param name="InStmtText"></param>
    /// <param name="InTraits"></param>
    /// <param name="InParentStart"></param>
    /// <param name="InParentWord"></param>
    /// <returns></returns>
    static WordCursor ParseParent(
      string StmtText, StmtTraits Traits, WordCursor ParentStart,
      StmtWord ParentWord)
    {
      WordCursor csr = ParentStart;
      StmtWord parentWord = ParentWord;
      StmtWord word = null;

      while (true)
      {
        word = null;

        // get next word in the stmt string.
        csr = Scanner.ScanNextWord(StmtText, csr);

        // end of string. Got nothing. 
        if (csr.IsEndOfString == true)
          break;

        // word is start of a sentence.
        if ((Traits.FormSentencesFromWhitespaceDelimWords == true)
          && (parentWord.IsSentence == false))
        {
          if ((csr.DelimIsWhitespace == true) || (csr.DelimIsOpenBrace == true))
          {
            if (csr.WordClassification != WordClassification.CommentToEnd)
						{
							word = new StmtWord(StmtText, parentWord, csr, WordCompositeCode.Sentence);
							csr.StayAtFlag = true;
							csr = ParseParent(StmtText, Traits, csr, word);
//							word.EndCursor.AssignDelimPart(csr);
						}
          }
        }

        // Word is braced. Make a composite word, then recursively call this method to
        // parse the contents. 
        bool bracedWordWasParsed = false;
        if ((word == null) && (csr.WordIsOpenBrace == true))
        {
          word = new StmtWord(StmtText, parentWord, csr, WordCompositeCode.Braced);
          csr = ParseParent(StmtText, Traits, csr, word);
          bracedWordWasParsed = true;

          // this braced word may be the start of a sentence. 
          if ((Traits.FormSentencesFromWhitespaceDelimWords == true)
            && (parentWord.IsSentence == false))
          {
            if ((csr.DelimIsWhitespace == true) || (csr.DelimIsOpenBrace == true))
            {
              StmtWord w2 = new StmtWord(
                StmtText, parentWord, word.WordCursor, WordCompositeCode.Sentence);
              word.Parent = w2;
            }
          }
        }

        // add the standalone word to the parent word 
        if (word == null)
        {
          // this word might be the whitespace after an EndStmt delim sentence and the
          // end of the braced parent.  ( ex: return _Name ; } )
          
          // todo: draw distinction between skipping the empty word before a close brace
          //       and the empty word after a comma delim sequence. ex: { a, b, c, }
          
          if (csr.IsDelimOnly == false)
          {
            word = new StmtWord(StmtText, parentWord, csr, WordCompositeCode.Atom);
          }
        }

        // is the final word in a sentence.
        // note: a semicolon or comma will end a sentence. 
        if (parentWord.CompositeCode == WordCompositeCode.Sentence)
        {
          if ((csr.DelimIsWhitespace == false)
          && (csr.DelimIsAssignmentSymbol == false))
          {
            break;
          }

            // sentence also ends when word is braced and this braced word is not the
            // first word in the sentence.  ex: get { return _Name ; }
          else if ((bracedWordWasParsed == true) && (parentWord.SubWords.Count > 1))
            break;
        }

        // final word in a Braced sequence.
        if (parentWord.CompositeCode == WordCompositeCode.Braced)
        {
          // the close brace delim is the closing brace of the parent word.
          // ex: { wd1 wd2 }  the } delim for wd2 applies to the braced word.
          if ((csr.DelimIsCloseBrace == true) 
            && (( word == null ) || ( word.OwnsCloseBracedDelim == false )))
          {
            // save the location of the closing brace. 
            parentWord.CloseBracePosition = csr.DelimBx;
						parentWord.CloseBraceCursor = csr;

            // cursor is located at the closing brace. We want the word after the closing
            // brace to always be a delim only word. In a parent where members are delimed
            // by comma this is no problem. But in a whitespace sep list, this might not
            // be the case without a little helpful adjustment.
            csr = Scanner.ScanNextWord(StmtText, csr);
            csr.StayAtFlag = true;

            if (csr.IsDelimOnly == true)
            {
              csr.StayAtFlag = false;
							parentWord.CloseBraceCursor = csr;
            }
            else if ((csr.WordClassification == WordClassification.OpenNamedBraced)
              || (csr.WordClassification == WordClassification.OpenContentBraced))
            {
              csr.SetVirtualCursor_WhitespaceOnly(csr.WordBx - 1);
              csr.StayAtFlag = false;
            }

            break;
          }
        }

        // line break. consider end of first pass processing of the stmt words
        // when the paren level is zero.
        //        if ((csr.DelimClass == DelimClassification.NewLine) && ( word.ParenLevel == 0 ))
        //          break ;
      }

      return csr;
    }


    // what first pass processing does:
    //   - cracks the stmt stream into a sequence of delim separated words
    //   - organizes the words in a bracket organized hierarchy
    //   - possibly, groups the words into stmt units based on end of stmt and 
    //     new line delimeters spcfd in StmtTraits. Also on the comment 
    //     markers contained in StmtTraits.
    // 
    public static WordCursor FirstPass(
      string InStmtText, StmtTraits InTraits, WordCursor InCsr, StmtWord InParentWord)
    {
      WordCursor csr = InCsr;
      StmtWord fsWord = null;
      StmtWord parentWord = InParentWord;
      int xx = 0;

      while (true)
      {
        xx += 1;
        csr = Scanner.ScanNextWord(InStmtText, csr);

        if (csr.IsEndOfString == true)
          break;

        StmtWord word = new StmtWord(InStmtText, parentWord, csr);

        // this word is start of stmt.
        if (fsWord == null)
          fsWord = word;

        // word is start of a sentence.
        if ((InTraits.FormSentencesFromWhitespaceDelimWords == true) &&
          ( parentWord.IsComposite == false ))
        {
          if ((csr.DelimIsWhitespace == true) || (csr.DelimIsOpenBrace == true))
          {
            word.CompositeCode = WordCompositeCode.Sentence;
            parentWord = word;
            word = new StmtWord(InStmtText, parentWord, csr);
          }
        }

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
        if ( csr.WordIsOpenBrace == true )
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

          // end of a sentence.
        else if ((parentWord.CompositeCode == WordCompositeCode.Sentence)
          && (csr.DelimIsWhitespace == false))
        {
          return csr;
        }

          // todo: have to expand this throw exception when the closing brace does
        //       not match the open brace.
        else if (csr.DelimClass == DelimClassification.CloseBraced)
        {
          break;
        }

      }
      return csr;
    }

    // build a complex that contains the lines to parse concatenated together.
    // The complex also a cross reference for converting buffer locations to
    // line positions.
    // ParseBufferComplex buf = new ParseBufferComplex(InTextLines);

    public static StmtWord ParseTextLines(
      ParseBufferComplex ParseBuf, StmtTraits Traits)
    {
      StmtWord topWord = null;

      WordCursor csr = 
        Scanner.PositionBeginWord(ParseBuf.Buffer, Traits);

      topWord = new StmtWord(ParseBuf.Buffer, null, null, WordCompositeCode.General);
      csr = ParseParent(ParseBuf.Buffer, Traits, csr, topWord);

      return topWord;
    }

  }
}
