using System;
using System.Collections.Generic ;
using System.Text ;
using AutoCoder ;
using AutoCoder.Scan;
using AutoCoder.Core;
using System.Text.RegularExpressions;
using AutoCoder.Text.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System;

namespace AutoCoder.Text
{

  // string extension methods:
  //    NextChar( ix )
  //    NextCharIsLfInCrLf( ix )
  //    Split( string Pattern )  // split on string pattern
  //    SplitLineBreaks( )      // split on any of the eol patterns
  //    TrimExcessNewLine( )    // trim all but one eol from end of string.
  //    
  //    

  // -------------------------- Stringer -----------------------------
  // class containing static methods which operation on strings.
  public static class Stringer
  {

    public static char AlphaInc( char InChar )
    {
      char[] alphabet = new char[] {
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
        'O','P','Q','R','S','T','U','V','W','X','Y','Z'} ;

      int ix = Array.IndexOf<char>( alphabet, InChar ) ;
      if ( ix == -1 )
        throw new ApplicationException( "Not a valid character to increment" ) ;
      else if ( ix == ( alphabet.Length - 1 ))
        throw new ApplicationException("incrementing past the last alpha character") ;
      return alphabet[ix+1] ;
    }

    /// <summary>
    /// Build a string from each string of the array, seperated by the
    /// specified delimeter.
    /// </summary>
    /// <param name="InArray"></param>
    /// <param name="InDelimeter"></param>
    /// <returns></returns>
    public static string ArrayToString(string[] InArray, string InDelimeter)
    {
      StringBuilder sb = new StringBuilder(1000);
      foreach (string line in InArray)
      {
        sb.Append(line);
        sb.Append(InDelimeter);
      }
      return sb.ToString();
    }

    /// <summary>
    /// concat a blank and then the value onto target string. 
    /// </summary>
    /// <param name="InConcatTo"></param>
    /// <param name="InConcatValue"></param>
    public static void BlankConcat(StringBuilder InConcatTo, string InConcatValue)
    {
      if ((InConcatValue != null) && (InConcatValue.Length > 0))
      {
        if (InConcatTo.Length > 0)
          InConcatTo.Append(' ');

        InConcatTo.Append(InConcatValue);
      }
    }

    /// <summary>
    /// Examine the string for its WordClassification content.
    /// </summary>
    /// <param name="InWord"></param>
    /// <param name="InTraits"></param>
    /// <returns></returns>
    public static CharObjectPair CalcWordClassification(
      string InWordText, TextTraits InTraits )
    {
      int Fx = 0, Ix = 0 ;
      WordClassification wc = WordClassification.None ;
      char braceChar = ' ' ;
      char ch1 = AcCommon.PullChar( InWordText, 0 ) ;
      int Ex = InWordText.Length - 1 ;

      // is quoted. the word runs to the closing quote.
      if ( Scanner.IsOpenQuoteChar( ch1 ) == true )
      {
        Ix = Scanner.ScanCloseQuote( InWordText, 0, InTraits.QuoteEncapsulation ) ;
        if ( Ix == Ex )
          wc = WordClassification.Quoted ;
        else
          wc = WordClassification.MixedText ;
      }

      // check if the string is a ContentBraced or NamedBraced word.
      if ( wc == WordClassification.None )
      {
        ScanPatterns combo = InTraits.DividerPatterns + InTraits.OpenNamedBracedPatterns;

        ScanPatternResults results =
          Scanner.ScanEqualAny(InWordText, 0, InWordText.Length, combo);

        // found a brace char
        if (( InTraits.IsOpenBraceChar( results.FoundPattern ) == true )
          && ( InTraits.IsDividerString( results.FoundPattern ) == false )) 
        {
          Ix = Scanner.ScanCloseBrace( InWordText, Fx ) ;
          if ( Ix == Ex )
          {
            braceChar = results.FoundPattern[0] ;
            if ( Fx == 0 ) // word starts with open brace char.
              wc = WordClassification.ContentBraced ;
            else
              wc = WordClassification.NamedBraced ;
          }
        }
      }

      // word is all delimeter.
      if ( wc == WordClassification.None )
      {
        Fx = Scanner.ScanNotEqualStrings(
          InWordText, 0, InWordText.Length, InTraits.DividerPatterns.StringArray).a;
        if ( Fx >= 0 )
          wc = WordClassification.Delimeter ;
      }

      // check if a numeric string.
      if ( wc == WordClassification.None )
      {
        char[] digitChars =
          new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '+', '-' } ;
        Fx = Scanner.ScanNotEqual( InWordText, 0, digitChars ).a ;
        if ( Fx == -1 )
        {
          wc = WordClassification.Numeric ;
          try
          {
            double vx = double.Parse( InWordText ) ;
          }
          catch( Exception )
          {
            wc = WordClassification.None ;
          }
        }
      }

      // any delim chars in the string.  if not, the string is a name.  otherwise, it is
      // mixed.
      if ( wc == WordClassification.None )
      {
        Fx = Scanner.ScanEqualAnyStrings(
          InWordText, 0, InWordText.Length, InTraits.DividerPatterns.StringArray).FoundPos;
        if ( Fx == -1 )
          wc = WordClassification.Identifier ;
        else
          wc = WordClassification.MixedText ;
      }

      return new CharObjectPair( braceChar, wc ) ;
    }

    /// <summary>
    /// compare substring equal pattern string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InIx"></param>
    /// <param name="InEx"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static bool CompareSubstringEqual(
      string InString, int InIx, int InEx, string InPattern)
    {
      int remLx = InEx - InIx + 1;

      if ((InPattern.Length <= remLx) 
        && (InPattern == InString.Substring(InIx, InPattern.Length)))
        return true;
      else
        return false;
    }

    /// <summary>
    /// check for the substring in a string equal any of the pattern strings.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InIx"></param>
    /// <param name="InEx"></param>
    /// <param name="InPatterns"></param>
    /// <returns></returns>
    public static int CompareSubstringEqualAny(
      string InString, int InIx, int InEx, string[] InPatterns)
    {
      int remLx = InEx - InIx + 1;
      int patIx = -1;
      while (true)
      {
        patIx += 1;
        if (patIx >= InPatterns.Length)
        {
          patIx = -1;
          break;
        }

        string pat = InPatterns[patIx];
        if ((pat.Length <= remLx) && (pat == InString.Substring(InIx, pat.Length)))
          break;
      }
      return patIx;
    }

    // --------------------------- Dequote ------------------------------
    public static string Dequote( string InText, QuoteEncapsulation InQem )
    {
      int Lx = InText.Length + 2 ;	// 2=quote chars. 
      char QuoteChar = InText[0] ;
      StringBuilder sb = new StringBuilder( Lx ) ;
      int Ix = 0 ;
      int EndIx = InText.Length - 2 ;
      while( true )
      {
        ++Ix ;
        if ( Ix > EndIx )
          break ;
        char ch1 = InText[Ix] ;

        // using the escape method to enclose quote chars. This means the escape char
        // is used to encapsulate other special characters in the quoted string.
        // todo: dequote using "QuotedStringTraits" rules.
        if (( ch1 == '\\' )
          && ( InQem == QuoteEncapsulation.Escape )
          && ( Ix < ( EndIx - 1 )))
        {
          IntCharPair rv = Dequote_MaterializeEscapeChar( InText, Ix ) ;
          sb.Append( rv.b ) ;
          Ix += ( rv.a - 1 ) ;
        }

          // quote char enquoted using the "double the char" method.
        else if (( ch1 == QuoteChar ) &&
          ( InQem == QuoteEncapsulation.Double ) &&
          ( AcCommon.PullChar( InText, Ix + 1 ) == QuoteChar ))
        {
          sb.Append( ch1 ) ;
          ++Ix ;
        }

          // any other character.  append to result string.
        else
          sb.Append( ch1 ) ;
      }
      return sb.ToString( ) ;
    }

    // ----------------------- Dequote_MaterializeEscapeChar ---------------------
    // used by the Dequote method. unpacks the standard escape sequences used in
    // quoted strings.
    // returns an int/char pair holding the length of the escape sequence and the
    // materialized character value.
    private static IntCharPair Dequote_MaterializeEscapeChar(string InString, int InIx)
    {
      char nx = AcCommon.PullCharArray(InString, InIx, 2)[1];
      if (nx == 't')
        return new IntCharPair(2, '\t');
      else if (nx == 'r')
        return new IntCharPair(2, '\r');
      else if (nx == 'n')
        return new IntCharPair(2, '\n');
      else if (nx == '\'')
        return new IntCharPair(2, '\'');
      else if (nx == '\\')
        return new IntCharPair(2, '\\');
      else if (nx == '"')
        return new IntCharPair(2, '"');
      else if (nx == '0')
        return new IntCharPair(2, '\0');
      else
        throw (new ApplicationException("Unexpected escape sequence starting at " +
          "position " + InIx + " in string: " + InString));
    }

    /// <summary>
    /// if the string is null, return an empty string. Otherwise, return the
    /// string itself.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static string EmptyIfNull( this string InValue)
    {
      if (InValue == null)
        return "";
      else
        return InValue;
    }

    // ------------------------- EndsWith ------------------------------------
    // test if string ends with sub string
    public static bool EndsWith( string InValue, string InEndsWith )
    {
      int EndsLx = InEndsWith.Length ;
      int Bx = InValue.Length - EndsLx ;
      if (( Bx >= 0 ) &&
        ( InValue.Substring( Bx, EndsLx ) == InEndsWith ))
        return true ;
      else
        return false ;
    }

    // ------------------------- EndsWith ------------------------------------
    // test if StringBuilder ends with sub string
    public static bool EndsWith( this StringBuilder Value, string EndsWith )
    {
      int EndsLx = EndsWith.Length ;
      int Bx = Value.Length - EndsLx ;
      if (( Bx >= 0 ) &&
        ( Value.ToString( Bx, EndsLx ) == EndsWith ))
        return true ;
      else
        return false ;
    }

    // ------------------------- Enquote ------------------------------
    public static string Enquote(string InValue)
    {
      return (Enquote(InValue, '"', QuoteEncapsulation.Escape));
    }

    // ------------------------- Enquote ------------------------------
    public static string Enquote(	string InValue, char InQuoteChar )
    {
      return( Enquote( InValue, InQuoteChar, QuoteEncapsulation.Escape )) ;
    }

    // ------------------------- Enquote ------------------------------
    public static string Enquote(
      string InValue, char InQuoteChar, QuoteEncapsulation InQem )
    {
      string rv = null ;
      if (InQem == QuoteEncapsulation.Double)
        rv = Enquote_Double(InValue, InQuoteChar);
      else
        rv = Enquote_Escape(InValue, InQuoteChar);

      return rv;
    }

    // ------------------------- Enquote_Double ------------------------
    // enquote a string using method where embeded quote chars are doubled.
    private static string Enquote_Double(
      string Value, char QuoteChar)
    {
      int Lx = 0;
      if ( Value != null )
        Lx = Value.Length;
      int Sx = (Lx * 2) + 2;

      // start the enquoted result string with the quote char.
      StringBuilder sb = new StringBuilder(Sx);
      sb.Append(QuoteChar);
      
      // for each char in the string being enquoted.
      for (int Ix = 0; Ix < Lx; ++Ix)
      {
        char ch1 = Value[Ix];

        // double up the quote chars contained in the string.
        if (ch1 == QuoteChar)
        {
          sb.Append(ch1);
        }

        // add char to the enquoted result.
        sb.Append(ch1);
      }

      // close out the enquoted string
      sb.Append(QuoteChar);

      return sb.ToString();
    }

    // ------------------------- Enquote_Escape ------------------------
    // enquote a string using method where embeded quote chars preceeded by
    // an escape char. 
    private static string Enquote_Escape(string Value, char QuoteChar)
    {
      int Lx = 0 ;
      if ( Value != null )
        Lx = Value.Length ;
      int Sx = (Lx * 2) + 2;
      char escapeChar = '\\' ;

      // start the enquoted result string with the quote char.
      StringBuilder sb = new StringBuilder(Sx);
      sb.Append(QuoteChar);

      // for each char in the string being enquoted.
      for (int Ix = 0; Ix < Lx; ++Ix)
      {
        char ch1 = Value[Ix];

        // preceed embedded quotes and escape chars with an escape.
        if ((ch1 == QuoteChar) || (ch1 == escapeChar))
        {
          sb.Append(escapeChar);
        }

        // add char to the enquoted result.
        sb.Append(ch1);
      }

      // close out the enquoted string
      sb.Append(QuoteChar);

      return sb.ToString();
    }

    // --------------------- GetCorrCloseBraceChars ------------------------------------
    public static char[] GetCorrCloseBraceChars(char[] InOpenBraceChars)
    {
      char[] closeBraceChars = new char[InOpenBraceChars.Length];
      for (int ix = 0; ix < InOpenBraceChars.Length; ++ix)
      {
        char ch1 = InOpenBraceChars[ix];
        if (ch1 == '<')
          closeBraceChars[ix] = '>';
        else if (ch1 == '{')
          closeBraceChars[ix] = '}';
        else if (ch1 == '(')
          closeBraceChars[ix] = ')';
        else if (ch1 == '[')
          closeBraceChars[ix] = ']';
      }
      return closeBraceChars;
    }

    // ------------------------------ GetNonNull -----------------------------
    // return the string value that is empty string if null value.
    public static string GetNonNull( string InValue )
    {
      if ( InValue == null )
        return "" ;
      else
        return InValue ;
    }

    public static string GetNonNull(string InValue, string InNullReplacement )
    {
      if (InValue == null)
        return InNullReplacement;
      else
        return InValue;
    }

    /// <summary>
    /// return the value of an object which is a string.
    /// If the object is null, or is not a string, return the InvalidReplace
    /// parm value.
    /// Usage example. You store a string value into the Tag property of a control.
    /// Use this static to get the string value of the tag property.
    /// </summary>
    /// <param name="InStringObject"></param>
    /// <param name="InInvalidReplace"></param>
    /// <returns></returns>
    public static string GetFromStringObject( object InStringObject, string InInvalidReplace )
    {
      if ( InStringObject == null )
        return InInvalidReplace ;
      if ( InStringObject.GetType( ) != typeof(string))
        return InInvalidReplace ;
      return (string) InStringObject ;
    }

    public static string GetValueOrDefault(this string Value, string Default = "")
    {
      if (Value == null)
        return Default;
      else
        return Value;
    }

    /// <summary>
    /// return the max InLx characters at the start ( head ) of the string
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InLx"></param>
    /// <returns></returns>
    public static string Head(this string Value, int InLx)
    {
      if (Value.Length <= InLx)
        return Value;
      else
        return Value.Substring(0, InLx);
    }

    /// <summary>
    /// return the max InLx characters at the start ( head ) of the string.
    /// If there is more to the string, concat the InMoreText to the end of the sample.
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InLx"></param>
    /// <param name="InMoreText"></param>
    /// <returns></returns>
    public static string Head(this string Value, int InLx, string MoreText )
    {
      if (Value.Length <= InLx)
        return Value;
      else
        return( Value.Substring(0, InLx) + MoreText );
    }

    /// <summary>
    /// scan for any of the pattern strings in the factor 1 string.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InBx"></param>
    /// <param name="InPatterns"></param>
    /// <returns></returns>
    public static int IndexOfAny( string InText, int InBx, string[] InPatterns )
    {
      int ix = -1;
      foreach( string pattern in InPatterns )
      {
        ix = InText.IndexOf( pattern, InBx ) ;
        if ( ix != -1 )
          break ;
      }
      return ix ;
    }

    /// <summary>
    /// test if all characters in the string are digits.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static bool IsDigits(this string Text)
    {
      bool isDigits = true;
      if ((Text == null ) || (Text.Length == 0))
        isDigits = false;
      else
      {
        for (int ix = 0; ix < Text.Length; ++ix)
        {
          if (Char.IsDigit(Text[ix]) == false)
          {
            isDigits = false;
            break;
          }
        }
      }
      return isDigits;
    }

    // ----------------------------- IsEmpty ------------------------------
    // string is null or zero length
    public static bool IsEmpty( string InValue )
    {
      if ( InValue == null )
        return true ;
      else if ( InValue.Length == 0 )
        return true ;
      else
        return false ;
    }

    /// <summary>
    /// Compare if factor 1 string is equal to any of the factor 2 strings
    /// </summary>
    /// <param name="InVlu1"></param>
    /// <param name="InVlu2"></param>
    /// <returns></returns>
    public static bool IsEqualAny(string InFac1, string[] InFac2)
    {
      bool rv = false;
      foreach (string vlu in InFac2)
      {
        if (vlu == InFac1)
        {
          rv = true;
          break;
        }
      }
      return rv;
    }

    // -------------------------- IsNotEmpty -----------------------------
    public static bool IsNotEmpty( string InValue )
    {
      return( !IsEmpty( InValue )) ;
    }

    public static bool IsPastEnd(this string Text, int Ix)
    {
      int ex = Text.Length - 1;
      if (Ix > ex)
        return true;
      else
        return false;
    }

    /// <summary>
    /// is index within the bounds of the string.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Ix"></param>
    /// <returns></returns>
    public static bool IsWithin(this string Text, int Ix)
    {
      if (Ix < 0)
        return false;
      else if (Ix > (Text.Length - 1))
        return false;
      else
        return true;
    }

    // --------------------------- IsQuoted --------------------------------
    // is the string enclosed in quotes.  the quoted string must be properly formed.
    // 1st char is quote, last char is closing quote, and any enclosed quotes are
    // properly encapsulated as per the QuoteEncapsulationMethod.
    public static bool IsQuoted( string InString, QuoteEncapsulation InQem )
    {
      if ( InString.Length < 2 )
        return false ;
      char ch1 = InString[0] ;
      if (( ch1 != '"' ) && ( ch1 != '\'' ))
        return false ;
      int Fx = Scanner.ScanCloseQuote( InString, 0, InQem ) ;
      if (( Fx != -1 ) && ( Fx == InString.Length - 1 ))
        return true ;
      else
        return false ;
    }

    /// <summary>
    /// check that string contains all valid variable name characters.
    /// </summary>
    /// <param name="InName"></param>
    /// <returns></returns>
    public static bool IsValidVariableName(string InName)
    {
      bool isValid = true;

      if (InName.Length == 0)
        isValid = false;
      else if (char.IsDigit(InName[0]) == true)
        isValid = false;
      
      else
      {
        foreach( char ch1 in InName )
        {
          if ( char.IsLetterOrDigit( ch1 ) == true )
          { }
          else
          {
            isValid = false ;
            break ;
          }
        }
      }

      return isValid;
    }

    /// <summary>
    /// serialize a list of strings as a CSV string.  
    /// </summary>
    /// <param name="InList"></param>
    /// <returns></returns>
    public static string xListToString(
      List<string> InList )
    {
      StringBuilder sb = new StringBuilder();
      foreach (string item in InList)
      {
        if (sb.Length > 0)
          sb.Append(", ");
        if (item != null)
          sb.Append(ToParseableString(item));
      }
      return sb.ToString();
    }

    /// <summary>
    /// return a sequence of strings as a single string. 
    /// Usually with Environment.NewLine between each line.
    /// </summary>
    /// <param name="List"></param>
    /// <param name="Delimiter"></param>
    /// <returns></returns>
    public static string ListToString(
      IEnumerable<string> List, string Delimiter)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string item in List)
      {
        if (item != null)
        {
          if (sb.Length > 0)
            sb.Append(Delimiter);
          sb.Append(item);
        }
      }
      return sb.ToString();
    }

    /// <summary>
    /// Build a long abbreviation of the input name. 
    /// return a lower case version of the word where the first char is lower case.
    /// </summary>
    /// <param name="InWord"></param>
    /// <returns></returns>
    public static string LongAbbreviateName(string InName)
    {
      string fsChar = null;
      if (InName.Length > 0)
      {
        fsChar = InName.Substring(0, 1);
      }
      else
        fsChar = "";

      string remWord = null;
      if (InName.Length > 1)
        remWord = InName.Substring(1);
      else
        remWord = "";

      return fsChar.ToLower() + remWord;
    }

    /// <summary>
    /// Decrypt a string that was encrypted using masked xor encryption.
    /// </summary>
    /// <param name="InEncrypted"></param>
    /// <param name="InMask"></param>
    /// <returns></returns>
    public static string MaskedDecrypt(string InEncrypted, string InMask)
    {
      byte[] encTextBytes = Convert.FromBase64String(InEncrypted);
      UnicodeEncoding ue = new UnicodeEncoding();
      string encText = ue.GetString(encTextBytes);
      string plainText = MaskedDecrypt_Decrypt(encText, InMask);
      return plainText;
    }

    private static string MaskedDecrypt_Decrypt(string InEncrypted, string InMask)
    {
      StringBuilder sb = new StringBuilder(InEncrypted.Length);
      string mask = InMask.Repeat(InEncrypted.Length);
      for (int ix = 0; ix < InEncrypted.Length; ++ix)
      {
        char encryptedChar = InEncrypted[ix];
        char maskChar = mask[ix];
        char plainChar = (char)(encryptedChar ^ maskChar);
        sb.Append(plainChar);
      }
      return sb.ToString();
    }

    /// <summary>
    /// Encrypt a string using masked xor encryption.
    /// </summary>
    /// <param name="InPlainText"></param>
    /// <param name="InMask"></param>
    /// <returns></returns>
    public static string MaskedEncrypt(string InPlainText, string InMask)
    {
      string encText = MaskedEncrypt_Encrypt(InPlainText, InMask);
      UnicodeEncoding ue = new UnicodeEncoding();
      byte[] encTextBytes = ue.GetBytes(encText);
      string encryptedText = Convert.ToBase64String(encTextBytes);
      return encryptedText;
    }

    private static string MaskedEncrypt_Encrypt(string InPlain, string InMask)
    {
      StringBuilder sb = new StringBuilder(InPlain.Length);
      string mask = InMask.Repeat(InPlain.Length);
      for (int ix = 0; ix < InPlain.Length; ++ix)
      {
        char plainChar = InPlain[ix];
        char maskChar = mask[ix];
        char encryptChar = (char)(plainChar ^ maskChar);
        sb.Append(encryptChar);
      }
      return sb.ToString();
    }

    public static char? NextChar(this string Text, int Ix)
    {
      int ex = Text.Length - 1;
      if (Ix >= ex)
        return null;
      else
        return Text[Ix + 1];
    }

    public static bool NextCharIsLfInCrLf(this string InText, int InIx)
    {
      char ch1 = InText[InIx];
      char? nxChar = InText.NextChar(InIx);
      if ((ch1 == '\r') && (nxChar != null) && (nxChar.Value == '\n'))
        return true;
      else
        return false;
    }

    /// <summary>
    /// Pad to the left if the length of the input string is less that the specified
    /// length.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="MinLength"></param>
    /// <param name="PadChar"></param>
    /// <returns></returns>
    public static string PadMinLeft(this string Text, int MinLength, char PadChar)
    {
      if (Text.Length >= MinLength)
        return Text;
      else
      {
        string s1 = Text.PadLeft(MinLength, PadChar);
        return s1;
      }
    }

    /// <summary>
    /// Try to parse the string representation of the boolean. If TryParse fails,
    /// return the CantParseValue argument value.
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InCantParseValue"></param>
    /// <returns></returns>
    public static bool ParseBooleanLeniently(this string InValue, bool InCantParseValue)
    {
      bool boolValue;
      bool rv = Boolean.TryParse(InValue, out boolValue);
      if (rv == false)
        return InCantParseValue;
      else
        return boolValue;
    }

    public static List<string> ParseSerializedListOfString(string InStream)
    {
      List<string> list = new List<string>();
      CsvCursor csr = new CsvCursor(InStream);

      while (true)
      {
        csr.NextValue( ) ;
        if (csr.IsEndOfString == true)
          break;
        if (csr.ItemValue == null)
          list.Add(null);
        else
          list.Add(csr.ItemValue ) ;
      }

      return list;
    }

    // ----------------------- QuotedPrintableEncode --------------------
    public static string QuotedPrintableEncode(Encoding InEncoding, char InChar)
    {
      byte[] bytes = new byte[10];
      StringBuilder sb = new StringBuilder(10);
      int ByteCx = InEncoding.GetBytes("" + InChar, 0, 1, bytes, 0);
      for (int Ix = 0; Ix < ByteCx; ++Ix)
      {
        if (bytes[Ix] < 16)
          sb.Append("=0" + Convert.ToString(bytes[Ix], 16).ToUpper());
        else
          sb.Append("=" + Convert.ToString(bytes[Ix], 16).ToUpper());
      }
      return sb.ToString();
    }

    /// <summary>
    /// Remove all instances of the array of characters from the input string.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InEqualChars"></param>
    /// <returns></returns>
    public static string RemoveAll(this string InText, char[] InEqualChars)
    {
      StringBuilder sb = new StringBuilder();

      foreach (char ch1 in InText)
      {
        // append to the output string if char is not in array of chars to remove.
        if (Array.IndexOf(InEqualChars, ch1) == -1)
          sb.Append(ch1);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Remove all instances of the array of characters from the input string.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InEqualChars"></param>
    /// <returns></returns>
    public static string RemoveAll(
      this string InText, char[] InEqualChars, int InStart, int InLength)
    {
      StringBuilder sb = new StringBuilder();

      for (int ix = 0; ix < InLength; ++ix)
      {
        char ch1 = InText[InStart + ix];

        // append to the output string if char is not in array of chars to remove.
        if (Array.IndexOf(InEqualChars, ch1) == -1)
          sb.Append(ch1);
      }

      return sb.ToString();
    }

    /// <summary>
    /// if the input string ends with the specified text, return the input text with
    /// the ending text removed.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="EndsWithText"></param>
    /// <returns></returns>
    public static Tuple<string,bool> RemoveEndsWithText(this string Text, string EndsWithText)
    {
      if (Text.EndsWith(EndsWithText))
      {
        int remLx = Text.Length - EndsWithText.Length;
        string resultText = Text.Substring(0, remLx);
        return new Tuple<string,bool>(resultText, true);
      }
      else
      {
        return new Tuple<string, bool>(Text, false);
      }
    }

    /// <summary>
    /// replace substring in string with the specified value
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx">replace begin position</param>
    /// <param name="InLx">length of substring to replace</param>
    /// <param name="InValue">replacement value</param>
    /// <returns></returns>
    public static string Replace(string InString, int InBx, int InLx, string InValue)
    {
      StringBuilder sb = new StringBuilder(InString.Length + InValue.Length - InLx);
      if ( InBx > 0 )
        sb.Append( InString.Substring( 0, InBx )) ;
      sb.Append( InValue ) ;
      int ix = InBx + InLx ;
      if ( ix < InString.Length )
        sb.Append( InString.Substring( ix )) ;
      return sb.ToString( ) ;
    }

    /// <summary>
    /// replace WordCursor located word in string with the specified value.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InCsr"></param>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static string Replace(string InString, WordCursor InCsr, string InValue)
    {
      InCsr.Throw_NotPositionAt();
      string res = Replace(InString, InCsr.WordBx, InCsr.WordLx, InValue);
      return res;
    }

    // ------------------------ ReplaceAllMarkedVariables --------------------------
    public static string ReplaceAllMarkedVariables(
      string InLine, string InMarkerText, ReplacementVariables InVars)
    {
      TextTraits traits = new TextTraits();
      string rv = ReplaceAllMarkedVariables(InLine, InMarkerText, InVars, traits);
      return rv ;
    }

    // ------------------------ ReplaceAllMarkedVariables --------------------------
    /// <summary>
    /// Scan and replace all the marked variables in the string. 
    /// Marked variables have the form: MarkerText( VariableName ).
    /// Where the MarkerText is assigned as a value not found otherwise in the string.
    /// The VariableName replacement value is found using the Find method of the
    /// ReplacementVariables parameter.
    /// </summary>
    /// <param name="InLine"></param>
    /// <param name="InMarkerText"></param>
    /// <param name="InVars"></param>
    /// <param name="InTraits"></param>
    /// <returns></returns>
    public static string ReplaceAllMarkedVariables(
      string InLine, string InMarkerText, ReplacementVariables InVars, TextTraits InTraits)
    {
      int ix = 0;
      StringBuilder sb = new StringBuilder(InLine.Length + 256);

      while (ix < InLine.Length)
      {
        IntObjectPair<WordCursor> rv =
          ReplaceMarkedVariables_FindNext(InLine, ix, InMarkerText, InTraits);

        // copy the scanned passed text to the wip line
        if (rv.a == -1)
          sb.Append( InLine.Substring( ix )) ; 
        else
        {
          int asisLx = rv.a - ix;
          if ( asisLx > 0 )
            sb.Append( InLine.Substring( ix, asisLx )) ;
        }

        // no more marker variables
        if (rv.a == -1)
          break;

        // the variable value. ( if variable is not found in the ReplacementVariables
        // object, pass along the MarkedVariable as is to the output. )
        string varVlu =
          ReplaceMarkedVariables_GetVariableValue(InLine, rv.b, InVars);
        if (varVlu == null)
        {
          sb.Append( InLine.Substring( rv.a, rv.b.WordLx )) ;
        }
        else
          sb.Append(varVlu);

        // advance the input string index.
        ix = rv.a + rv.b.WordLx;
      }
      return sb.ToString();
    }

    // ----------------------- ReplaceMarkedVariables_FindNext ---------------------------
    private static IntObjectPair<WordCursor> ReplaceMarkedVariables_FindNext(
      string InLine, int InBx, string InMarkerText, TextTraits InTraits)
    {
      WordCursor csr = null;
      string markPat = InMarkerText + "(";
      int fx = InLine.IndexOf(markPat, InBx);
      if (fx != -1)
      {
        csr = Scanner.PositionBefore(InLine, fx, InTraits);
        csr = Scanner.ScanNextWord(InLine, csr);
        if (csr.WordClassification != WordClassification.NamedBraced)
          csr = null;
      }
      return new IntObjectPair<WordCursor>(fx, csr);
    }

    // ----------------ReplaceMarkedVariables_GetVariableValue --------------------- 
    private static string ReplaceMarkedVariables_GetVariableValue(
      string InLine, WordCursor InCsr, ReplacementVariables InVars)
    {
      string varVlu = null;
      Stmt stmt = new Stmt(InLine, InCsr);
      if (stmt.Elements.Count > 0)
      {
        string sbsVarName = stmt.Elements[0].ToString();
        if (sbsVarName != null)
        {
          ReplacementVariable var = InVars.Find(sbsVarName);
          if (var != null)
          {
            varVlu = var.Value;
          }
        }
      }
      return varVlu;
    }

    /// <summary>
    /// Build a short abbreviation of the input name. Extract each upper case char,
    /// convert those chars to lowercase, then return the concat ofthose chars.
    /// </summary>
    /// <param name="InName"></param>
    /// <returns></returns>
    public static string ShortAbbreviateName(string InName)
    {
      StringBuilder sb = new StringBuilder();
      for (int ix = 0; ix < InName.Length; ++ix)
      {
        char ch1 = InName[ix];
        if (Char.IsUpper(ch1) == true)
          sb.Append(Char.ToLower(ch1));
      }

      if (sb.Length == 0)
      {
        sb.Append(InName.SubstringLenient(0, 3));
      }

      return sb.ToString();
    }

    /// <summary>
    /// Enquote a string with single quotes.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string SingleEnquote(this string Value)
    {
      string s1 = Stringer.Enquote(Value, '\'');
      return s1;
    }

    // ---------------------- Split --------------------------------
    // same as string.Split, only splits based on the occurance of string.
#if skip
    public static string[] zSplit( string InString, string InPattern )
    {
      int Bx, Fx, Tx, Lx ;

      // first, talley the number of lines which will be split out.
      int Cx = Talley( InString, InPattern ) ;
      if ( Tail( InString, InPattern.Length ) != InPattern )
        ++Cx ;

      // now we know how many lines will be needed.  alloc the string array.
      string[] results = new string[Cx] ;
      Tx = 0 ;

      // split out the strings.
      Bx = 0 ;
      while( true )
      {
        if ( Bx >= InString.Length )
          break ;

        // remaining string to split is shorter than the split pattern. Add this 
        // remainder to the results array and leave. 
        if ( Bx > ( InString.Length - InPattern.Length ))
        {
          results[Tx] = InString.Substring( Bx ) ;
          break ;
        }

        // no more pattern. from here to the end is the final string.
        Fx = InString.IndexOf( InPattern, Bx ) ;
        if ( Fx == -1 )
        {
          results[Tx] = InString.Substring( Bx ) ;
          break ;
        }

        // split out the string up to the split pattern.
        Lx = Fx - Bx ;
        if ( Lx == 0 )
          results[Tx] = "" ;
        else
          results[Tx] = InString.Substring( Bx, Lx ) ;
        ++Tx ;

        // advance to the next split start position.
        Bx = Fx + InPattern.Length ;
      }

      return results ;
    }
#endif

    /// <summary>
    /// Split string on string pattern.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static string[] Split(this string InText, string InPattern)
    {
      string[] xx = InText.Split(new string[] { InPattern }, StringSplitOptions.None);
      return xx;
    }

    public static string[] SplitLineBreaks(this string InText)
    {
      int fx = InText.IndexOfAny(new char[] { '\r', '\n' });
      if (fx == -1)
        return new string[1] { InText };
      else
      {
        List<string> lines = new List<string>();
        string[] xx = InText.Split(
          new string[] { Environment.NewLine, "\r", "\n" },
          StringSplitOptions.None);
        for (int ix = 0; ix < xx.Length; ++ix)
        {
          string s1 = xx[ix];
          if ((ix == xx.Length - 1) && (s1.Length == 0))
          {
          }
          else
          {
            lines.Add(s1);
          }
        }

        return lines.ToArray();
      }
    }

    public static string SplitCharEncode(
      this string Text, char SplitChar, string SubstitutePair = "&+")
    {
      StringBuilder sb = new StringBuilder();
      char sp1 = SubstitutePair[0];
      char sp2 = SubstitutePair[1];

      int ix = -1;
      while(true)
      {
        ix += 1;
        if (ix >= Text.Length)
          break;

        char ch1 = Text[ix] ;

        if (ch1 == SplitChar)
        {
          sb.Append(SubstitutePair);
        }

        else if (ch1 == sp1)
        {
          sb.Append(sp1);
          sb.Append(sp1);
        }

        else
        {
          sb.Append(ch1);
        }
      }

      return sb.ToString();
    }

    public static string SplitCharDecode(
      this string Text, char SplitChar, string SubstitutePair = "&+")
    {
      StringBuilder sb = new StringBuilder();
      char sp1 = SubstitutePair[0] ;
      char sp2 = SubstitutePair[1] ;

      int ix = -1;
      while(true)
      {
        ix += 1;
        if (ix >= Text.Length)
          break;

        char ch1 = Text[ix];

        if (ch1 != sp1)
        {
          sb.Append(ch1);
        }

        else 
        {
          char? nxChar = Text.NextChar(ix);
          if (nxChar == null)
            throw new ApplicationException("invalid substitution sequence");

          // the current two chars are the replacement for the split char.
          else if (nxChar.Value == sp2)
          {
            sb.Append(SplitChar);
            ix += 1;
          }

          else if (nxChar.Value == sp1)
          {
            sb.Append(sp1);
            ix += 1;
          }

          else
            throw new ApplicationException("invalid substitution char");
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// find the bounds of the word at Pos in Text. Return 3 segments. The text before
    /// the word, the word itself and the text after the word.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Pos"></param>
    /// <param name="NonWordChars"></param>
    /// <returns></returns>
    public static Tuple<string, string, string> SplitOnWordInText(
      string Text, int Pos, char[] NonWordChars)
    {
      string textBefore = null;
      string textAfter = null;
      string textMiddle = null;
      int middleStart = Pos - 5;
      int middleLength = 10;

      {
        var rv = Scanner.ScanReverseEqual(Text, Pos, NonWordChars);

        middleStart = rv.ResultPos;
        if (middleStart == -1)
          middleStart = 0;
        else
          middleStart += 1;

        rv = Scanner.ScanEqualAny(Text, Pos, NonWordChars);
        int ex = rv.ResultPos;
        if (ex == -1)
          ex = Text.Length - 1;
        else
          ex -= 1;

        middleLength = ex - middleStart + 1;
      }

      if (middleStart < 0)
        middleStart = 0;

      // adjust middle length to not exceed end of text.
      if ((middleStart + middleLength) > Text.Length)
        middleLength = Text.Length - middleStart;

      if (middleStart > 0)
        textBefore = Text.Substring(0, middleStart);
      textMiddle = Text.Substring(middleStart, middleLength);
      if ((middleStart + middleLength) < Text.Length)
        textAfter = Text.Substring(middleStart + middleLength);

      return new Tuple<string, string, string>(textBefore, textMiddle, textAfter);
    }

    /// <summary>
    /// Return a substring that is the length spcfd. Pad with blanks if the length 
    /// exceeds the remaining length of the string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <returns></returns>
    public static string SubstringPadded(this string InString, int InBx, int InLx)
    {
      int remLx = InString.Length - InBx ;
      if (InLx <= remLx)
        return (InString.Substring(InBx, InLx));
      else
        return (InString.Substring(InBx).PadRight(InLx));
    }

    /// <summary>
    /// Convert string representation of boolean value ("true", "off" ) to
    /// actual boolean value.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static bool ToBoolean(string InValue, string InDefaultValue )
    {
      string s1 = InValue;
      if ((s1 == null) || (s1.Length == 0))
        s1 = InDefaultValue;
      return ToBoolean(s1);
    }

    /// <summary>
    /// Convert string representation of boolean value ("true", "off" ) to
    /// actual boolean value.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static bool ToBoolean( string InValue )
    {
      string s1 = InValue.ToLower( ) ;
      if (( s1 == "true" ) || ( s1 == "on" ))
        return true ;
      else if (( s1 == "false" ) || ( s1 == "off" ))
        return false ;
      else
        throw new ApplicationException( 
          "string value " + InValue + " does not contain representation " +
          "of boolean value" ) ;
    }

    /// <summary>
    /// Copy chars from string into a char[]. If the length to copy exceeds the bounds of
    /// the string, fill the returned char array with hex 00.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InStart"></param>
    /// <param name="InLength"></param>
    /// <returns></returns>
    public static char[] ToCharArrayLenient(this string InText, int InStart, int InLength)
    {
      char[] rv = new char[InLength];
      int remLx = InText.Length - InStart;

      for (int ix = 0; ix < InLength; ++ix)
      {
        if (ix < remLx)
          rv[ix] = InText[ix + InStart];
        else
          rv[ix] = '\0';
      }
      return rv;
    }

    // ----------------------- ToHexExternalForm --------------------
    public static string ToHexExternalForm(string InText)
    {
      StringBuilder sb = new StringBuilder();
      foreach (char ch1 in InText)
      {
        int xx = (int)ch1;
        string chHex = xx.ToString("X").PadLeft(2, '0');

        string chRep = null;
        if (ch1 == '\r')
          chRep = "CR";
        else if (ch1 == '\t')
          chRep = "TB";
        else if (ch1 == '\n')
          chRep = "LF";
        else if (ch1 == ' ')
          chRep = "SP";
        else
          chRep = ch1.ToString();

        string s1 = chRep + "(" + chHex + ")";

        if (sb.Length > 0)
          sb.Append(" ");
        sb.Append(s1);
        if (s1.Length == 5)
          sb.Append(" ");
      }
      return sb.ToString();
    }

    public static IEnumerable<string> ToHexExternalFormLines(string InText)
    {
      int ix = 0;
      int lx = 0;
      while (ix < InText.Length)
      {
        lx = InText.Length - ix;
        lx = Math.Min(lx, 10);
        string seg = InText.Substring(ix, lx);
        string hexEf = ToHexExternalForm(seg);
        yield return hexEf;

        ix += lx;
      }
    }

    /// <summary>
    /// convert string to integer. String value of blanks returns zero.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static Int32 ToInt32(this string InValue)
    {
      if (StringExt.IsBlank(InValue) == true)
        return 0;
      else
        return Int32.Parse(InValue);
    }

    /// <summary>
    /// Encode the string so that it can always be parsed back to its
    /// original form. ( if a simple name, leave as is. If contains blanks,
    /// commas, etc, enclose in quotes.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static string ToParseableString(string InValue)
    {
      int ix = InValue.IndexOfAny(new char[] { ' ', ',', '"' });
      if (ix == -1)
        return InValue;
      else
      {
        return Stringer.Enquote(InValue);
      }
    }

    /// <summary>
    /// Trim each instance of the trim string from the end of the subject string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InTrim"></param>
    /// <returns></returns>
    public static string TrimEnd(string InString, string InTrim)
    {
      string wip = InString;
      int trimLx = InTrim.Length;
      while (true)
      {
        int bx = wip.Length - trimLx;
        if (bx < 0)
          break;
        string s1 = wip.Substring(bx, trimLx);
        if (s1 != InTrim)
          break;
        wip = wip.Substring(0, bx );
      }
      return wip;
    }

    /// <summary>
    /// trim spaces from the end of the input string.
    /// </summary>
    /// <param name="InText"></param>
    /// <returns></returns>
    public static string TrimEndingBlanks(this string Text)
    {
      char[] blank = new char[] { ' ' };
      string s1 = Text;
      if (s1 == null)
        s1 = "";
      s1 = s1.TrimEnd(blank);
      return s1;
    }

    /// <summary>
    /// Trim all but one NewLine sequence from the end of the line.
    /// </summary>
    /// <param name="InText"></param>
    /// <returns></returns>
    public static Tuple<string, bool> TrimExcessNewLine(this string InText)
    {
      string s1 = InText;

      // remove all the NewLines from end of the string.
      int rmvCx = 0;
      while (true)
      {
        if (s1.EndsWith(Environment.NewLine))
        {
          s1 = s1.Remove(s1.Length - 2);
          rmvCx += 1;
        }

        else if ((s1.EndsWith("\r")) || (s1.EndsWith("\n")))
        {
          s1 = s1.Remove(s1.Length - 1);
          rmvCx += 1;
        }

        else
          break;
      }

      // add one of the ending NewLines back.
      if (rmvCx > 0)
        s1 = s1 + Environment.NewLine;

      // some new lines were removed.
      bool someRemoved = false;
      if (rmvCx > 1)
        someRemoved = true;

      return new Tuple<string, bool>(s1, someRemoved);
    }

    // ----------------------- TrimStartAndEnd -----------------------
    public static string TrimStartAndEnd(
      string InString,
      char[] InTrimStartChars,
      char[] InTrimEndChars )
    {
      string OutString = InString.TrimStart( InTrimStartChars ) ;
      if ( OutString == null )
        return "" ;
      OutString = OutString.TrimEnd( InTrimEndChars ) ;
      if ( OutString == null )
        return "" ;
      else
        return OutString ;
    }


    public static string UrlEncode(string InText)
    {
      string s1 = UrlEncode(InText, HowUrlEncodeSpaceChar.Hex);
      return s1;
    }

    public static string UrlEncode(string InText, HowUrlEncodeSpaceChar InSpaceEncode )
    {
      // notes: the following chars are ok: * _ - . ' ( ) 
      StringBuilder sb = new StringBuilder();
      foreach (char ch1 in InText)
      {
        if (ch1 == ' ')
        {
          if (InSpaceEncode == HowUrlEncodeSpaceChar.Hex)
            sb.Append("%20");
          else
            sb.Append("+");
        }

        else
        {
          if (ch1 == '"')
            sb.Append("%22");
          else if (ch1 == '#')
            sb.Append("%23");
          else if (ch1 == '$')
            sb.Append("%24");
          else if (ch1 == '%')
            sb.Append("%25");
          else if (ch1 == '&')
            sb.Append("%26");
          else if (ch1 == '+')
            sb.Append("%2B");
          else if (ch1 == ',')
            sb.Append("%2C");
          else if (ch1 == '/')
            sb.Append("%2F");
          else if (ch1 == ':')
            sb.Append("%3A");
          else if (ch1 == ';')
            sb.Append("%3B");
          else if (ch1 == '<')
            sb.Append("%3C");
          else if (ch1 == '=')
            sb.Append("%3D");
          else if (ch1 == '>')
            sb.Append("%3E");
          else if (ch1 == '?')
            sb.Append("%3F");
          else if (ch1 == '@')
            sb.Append("%40");
          else if (ch1 == '[')
            sb.Append("%5B");
          else if (ch1 == '\\')
            sb.Append("%5C");
          else if (ch1 == ']')
            sb.Append("%5D");
          else if (ch1 == '^')
            sb.Append("%5E");
          else if (ch1 == '`')
            sb.Append("%60");
          else if (ch1 == '{')
            sb.Append("%7B");
          else if (ch1 == '|')
            sb.Append("%7C");
          else if (ch1 == '}')
            sb.Append("%7D");
          else if (ch1 == '~')
            sb.Append("%7E");
          else
            sb.Append(ch1);
        }
      }
      return sb.ToString();
    }

    /// <summary>
    /// Return the value of the string argument. Return "0" if the string is 
    /// null, empty or spaces.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static string ZeroIfBlank(string InValue)
    {
      if (InValue.IsBlank( ))
        return "0";
      else
        return InValue;
    }
    
  } // end class Stringer
} // end namespace AutoCoder.Text
