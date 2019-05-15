using System;
using System.Collections.Generic;
using System.Text;

#if skip

namespace AutoCoder.Text
{
  // consider strings that are delimited, not by characters, but by patterns of 
  // characters. the "->" pointer in c++.  

  // the NonWordLookup array has a side by side, entry to entry relationship
  // with NonWordChars. Where the array index to nonWordChar is also the index
  // to the NonWordLookup array, which contains the full non word string.

  // In the event there are more than one nonword strings with the same starting
  // char, use the NextLookup member to access the other nonword string with 
  // the same starting char.
  public class ScannerNonWord
  {
    DelimClassification mDelimClass;

    // the full string value of the non word pattern.
    string mValue;
    
    char mNonWordChar;

    // the next ScannerNonWord object which has the same starting char as 
    // this object. Used in the FindInLookupChain method to return the
    // ScannerNonWord which correctly matches the non word pattern in the string
    // being scanned.
    ScannerNonWord mNextLookup;

    public ScannerNonWord()
    {
      mDelimClass = DelimClassification.None;
      mValue = null;
      mNextLookup = null;
      mNonWordChar = '\0' ;
    }

    public ScannerNonWord(DelimClassification InDelimClass, string InValue)
    {
      mDelimClass = InDelimClass;
      mValue = InValue;
      mNextLookup = null;
      mNonWordChar = '\0' ;
    }

    public ScannerNonWord(DelimClassification InDelimClass, char InNonWordChar)
    {
      mDelimClass = InDelimClass;
      mValue = null;
      mNextLookup = null;
      mNonWordChar = InNonWordChar;
    }

    /// <summary>
    /// this non word object is a single character.
    /// </summary>
    public bool IsNonWordChar
    {
      get { return mValue == null; }
    }

    /// <summary>
    /// this non word object is a char string.
    /// </summary>
    public bool IsNonWordString
    {
      get { return mValue != null; }
    }

    public bool IsOpenBraced
    {
      get
      {
        return Scanner.IsOpenBraced(mDelimClass);
      }
    }

    public char NonWordChar
    {
      get
      {
        if (mValue != null)
          throw new ApplicationException("non word value is a string");
        else
          return mNonWordChar;
      }
    }

    public string NonWordString
    {
      get
      {
        if (mValue == null)
          return mNonWordChar.ToString();
        else
          return mValue;
      }
      set { mValue = value; }
    }

    public DelimClassification DelimClass
    {
      get { return mDelimClass; }
      set { mDelimClass = value; }
    }

    public ScannerNonWord NextLookup
    {
      get { return mNextLookup; }
    }

    /// <summary>
    /// Append to end of lookup chain where the leading character is the same
    /// for all the delim strings in the chain.
    /// </summary>
    /// <param name="InNextLookup"></param>
    /// <returns></returns>
    public ScannerNonWord AppendNextLookup(ScannerNonWord InNextLookup)
    {
      ScannerNonWord addedTo = null;

      // skip the append if nonword string or char already exists in the lookup
      // chain.
      if ((InNextLookup.IsNonWordChar == true)
        && (this.IsNonWordChar == true))
      {
        addedTo = null;
      }

      else if ((InNextLookup.IsNonWordString == true)
        && (this.IsNonWordString == true)
        && (InNextLookup.NonWordString == this.NonWordString))
      {
        addedTo = null;
      }

        // this item is the end of the chain.
      else if (mNextLookup == null)
      {
        mNextLookup = InNextLookup;
        addedTo = this;
      }

        // step down the chain of nonword string/char with the same leading char.
      else
      {
        addedTo = mNextLookup.AppendNextLookup(InNextLookup);
      }

      return addedTo;
    }

    /// <summary>
    /// Compare if this non word pattern is equal to the substring starting at
    /// pos InBx.
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InBx"></param>
    /// <returns></returns>
    public bool IsEqual( Scanner.ScanBoundedString InBoundedString, int InBx )
    {
      int remLx = InBoundedString.Ex - InBx + 1 ;
      if ( remLx < mValue.Length )
        return false ;
      else
      {
        int Lx = mValue.Length ;
        return( InBoundedString.Substring( InBx, Lx ) == mValue ) ;
      }
    }

    /// <summary>
    /// Return the ScannerNonWord which fully matches the non word pattern 
    /// located at pos InIx in the string being searched. When the pattern is a 
    /// single char, then this NonWord object is the matching object. When the 
    /// pattern is a string, have to check each non word string as the pattern in
    /// the string.
    /// </summary>
    /// <param name="InStart"></param>
    /// <param name="InBoundedString"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public static ScannerNonWord FindInLookupChain(
      ScannerNonWord InStart, Scanner.ScanBoundedString InBoundedString, int InIx)
    {
      ScannerNonWord snw = InStart;
      ScannerNonWord found = null;
      ScannerNonWord foundSingleChar = null;

      // use the single linked list chaining structure of ScannerNonWord to
      // compare each nonword string to the char sequence in the scanned string.
      while (snw != null)
      {
        // this ScannerNonWord consists of a single char. there is no string of chars
        // that the char is the leading char of.
        // ( When the single char word is found have to continue looking. Rule is that
        //   matching delim strings take precedence over matching single char. ) 
        if (snw.IsNonWordChar == true)
        {
          foundSingleChar = snw;
        }

        else if (snw.IsEqual(InBoundedString, InIx) == true)
        {
          found = snw;
          break;
        }

        snw = snw.NextLookup;
      }

      if (found != null)
        return found;
      else
        return foundSingleChar;
    }

  }

}

#endif
