using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using System.Security;
using System.IO;
using AutoCoder.Core.Enums;
using System.Globalization;
using System.Data;
using System.Xml;
using AutoCoder.Ext.System.Xml;
using AutoCoder.Html.HtmlParser;

namespace AutoCoder.Ext.System
{
  public static partial class StringExt
  {
    /// <summary>
    /// apply ApplyText to the ToText input string. return with the new value.
    /// </summary>
    /// <param name="ToText"></param>
    /// <param name="ApplyText"></param>
    /// <param name="Bx"></param>
    /// <param name="Lx"></param>
    /// <returns></returns>
    public static string ApplyText( this string ToText, string ApplyText, int Bx, int Lx = 0 )
    {
      var sb = new StringBuilder();

      // building a new string. Get any text before the pos to be applied to.
      if ( Bx > 0 )
      {
        sb.Append(ToText.Substring(0, Bx));
      }

      // apply the new text.
      int applyLx = Lx;
      {
        if (applyLx == 0)
          applyLx = ApplyText.Length;
        sb.Append(ApplyText.Substring(0, applyLx));
      }

      // apply any text that follows the applied text.
      {
        int bx = Bx + applyLx;
        int remLx = ToText.Length - bx;
        if (remLx > 0)
          sb.Append(ToText.Substring(bx, remLx));
      }

      return sb.ToString();
    }

    /// <summary>
    /// Return single char represented by the ascii encoded hex value.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InStart"></param>
    /// <returns></returns>
    public static char AsciiHexPairToChar(this string Text, int Start)
    {
      string hexPair = Text.Substring(Start, 2);
      byte b1 = global::System.Convert.ToByte(hexPair, 16);
      char[] ch1 = global::System.Text.Encoding.ASCII.GetChars(new byte[] { b1 });
      return ch1[0];
    }

    /// <summary>
    /// examine the input Text and return a char value which does not exist in 
    /// the text. This method can be used on its own. It is also used by the Mark and
    /// Unmark methods.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static char CalcMarkerChar(this String Text)
    {
      char ch1 = Char.MaxValue;
      while (true)
      {
        int fx = Text.IndexOf(ch1);
        if (fx == -1)
          break;
        int vv = (int)ch1;
        vv -= 1;
        ch1 = (char)vv;
      }
      return ch1;
    }

    /// <summary>
    /// return the input string, with the first char capitalized.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static string CapitalizeFirstChar(this string Text)
    {
      string s1 = Head(Text, 1).ToUpper();
      if (Text.Length < 2)
        return s1;
      else
        return s1 + Text.Substring(1);
    }

    /// <summary>
    /// Capitalize the first char of the string, the remainder are lower case.
    /// </summary>
    /// <param name="InWord"></param>
    /// <returns></returns>
    public static string CapitalizeWord(this string InWord)
    {
      if ((InWord == null) || (InWord.Length == 0))
        return null;
      else if (InWord.Length == 1)
        return 
          global::System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToUpper(InWord);
      else
      {
        string lcWord =
        global::System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToLower(InWord);
        char ucChar = 
          global::System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToUpper(
          InWord[0]);
        return ucChar + lcWord.Substring(1);
      }
    }

    /// <summary>
    /// return -1 if Text CompareTo compares less than From.
    /// return 1 if Text CompareTo compares greater than To.
    /// Otherwise, return 0. Text is within range of From and To.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="From"></param>
    /// <param name="To"></param>
    /// <returns></returns>
    public static int CompareToRange(this string Text, string From, string To)
    {
      int rc1 = Text.CompareTo(From);
      int rc2 = Text.CompareTo(To);
      if (rc1 < 0)
        return rc1;
      else if (rc2 > 0)
        return rc2;
      else
        return 0;
    }

    /// <summary>
    /// return true if the string contains any of the pattern strings.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Patterns"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string Text, string[] Patterns)
    {
      foreach (string pat in Patterns)
      {
        if (Text.Contains(pat))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Trim whitespace, replace with default value if empty.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Default"></param>
    /// <returns></returns>
    public static string DefaultTrim(this string Text, string Default = "")
    {
      if (Text == null)
        return Default;
      else
      {
        string text = Text.TrimWhitespace();
        if (text.Length == 0)
          return Default;
        else
          return text;
      }
    }

    /// <summary>
    /// Trim whitespace, replace empty with default value, clip if exceeds length.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Default"></param>
    /// <param name="MaxLength"></param>
    /// <returns></returns>
    public static string DefaultTrimClip(this string Text, string Default, int MaxLength)
    {
      string rv;
      if (Text == null)
        rv = Default;
      else
      {
        rv = Text.TrimWhitespace();
        if (rv.Length == 0)
          rv = Default;
      }

      // clip the length.
      if (rv.Length > MaxLength)
        rv = rv.Substring(0, MaxLength);

      return rv;
    }

    /// <summary>
    /// Enquote a string with double quotes.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string DoubleEnquote(this string Value)
    {
      string s1 = Stringer.Enquote(Value, '\"');
      return s1;
    }

    public static string DoubleEnquote(this string Value, QuoteEncapsulation QuoteEncapsulation)
    {
      string s1 = Stringer.Enquote(Value, '\"', QuoteEncapsulation);
      return s1;
    }
    /// <summary>
    /// return the value of the input string. If the valueis null, return an empty
    /// string.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string EmptyIfNull(this string Value )
    {
      if (Value == null)
        return String.Empty;
      else
        return Value;
    }

    /// <summary>
    /// Test if string ends with any pattern within the array of patterns. 
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InAnyPatterns"></param>
    /// <returns></returns>
    public static bool EndsWithAny(this string InString, string[] InAnyPatterns)
    {
      foreach (string s1 in InAnyPatterns)
      {
        if (InString.EndsWith(s1) == true)
          return true;
      }
      return false;
    }

    /// <summary>
    /// return the first nonword delimited string in the text.
    /// Advance past the initial NonWord chars. Then return the chars that run
    /// until to the first NonWord char.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="NonWordChars"></param>
    /// <returns></returns>
    public static string xFirstWord(this string Text, char[] NonWordChars )
    {
      int bx = Text.IndexOfNone(0, NonWordChars);
      if (bx == -1)
        return null;
      else
      {
        int fx = Text.IndexOfAny(NonWordChars, bx);
        if (fx == -1)
          return Text.Substring(bx);
        else
          return Text.Substring(bx, fx - bx);
      }
    }

    /// <summary>
    /// return the first word in the string. Return the word and its location in the
    /// string.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="NonWordChars"></param>
    /// <returns></returns>
    public static Tuple<string, int, int> firstWord(this string Text,
      char[] Whitespace = null, char[] NonWordChars = null)
    {
      int lx = -1;
      int bx = 0;
      if (Whitespace == null)
        Whitespace = new char[] { ' ' };
      if (NonWordChars == null)
        NonWordChars = Whitespace;

      // advance past whitespace.
      bx = Text.IndexOfNone(0, Whitespace);

      // all whitespace to end of string. 
      if (bx == -1)
      {
      }

      // start of word found. scan to delim that ends the word.
      else
      {
        int fx = Text.IndexOfAny(NonWordChars, bx);
        if (fx == -1)
        {
          lx = Text.Length - bx;
        }
        else
        {
          lx = fx - bx;
        }
      }

      // isolate the word.
      string wd = null;
      if (lx > 0)
        wd = Text.Substring(bx, lx);
      else
        wd = String.Empty;

      return new Tuple<string, int, int>(wd, bx, lx);
    }
    /// <summary>
    /// split the string into two parts. The first word and then the remaining text of
    /// the string.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Whitespace"></param>
    /// <param name="NonWordChars"></param>
    /// <returns></returns>
    public static Tuple<string, string> FirstWordSplit(this string Text,
      char[] Whitespace = null, char[] NonWordChars = null)
    {
      string part1 = String.Empty;
      string part2 = String.Empty;

      var rv = Text.firstWord(Whitespace, NonWordChars);
      part1 = rv.Item1;
      var bx = rv.Item2;
      var lx = rv.Item3;

      if (bx >= 0)
      {
        int ix = bx + lx;
        if (ix < Text.Length)
          part2 = Text.Substring(ix);
      }

      return new Tuple<string, string>(part1, part2);
    }

    /// <summary>
    /// return a string containing the first xxx number of characters in the string.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static string Head(this string Text, int Length)
    {
      if (Text == null)
        return "";
      else if (Text.Length < Length)
        return Text;
      else
        return Text.Substring(0, Length);
    }

    /// <summary>
    /// convert the text consisting of two hex characters to a byte value. If invalid
    /// text return an error message.
    /// </summary>
    /// <param name="HexText"></param>
    /// <returns></returns>
    public static Tuple<byte, string> HexTextToByte(this string HexText)
    {
      byte bv = 0x00;
      string errmsg = null;

      var text = HexText.ToUpper();

      // chunk is 2 chars and then a space.
      text = text.TrimEnd(new char[] { ' ' });
      if (text.Length != 2)
      {
        errmsg = "hex text must be length 2.";
      }

      // both chars of the chunk must be 0 thru 9, A thru F.
      if (errmsg == null)
      {
        char[] charSet = new char[] { '0', '1', '2', '3', '4', '5', '6',
          '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        var fx = text.IndexOfNone(0, charSet);
        if (fx >= 0)
          errmsg = "hex text contains invalid character";
      }

      if (errmsg == null)
      {
        bv = global::System.Convert.ToByte(text, 16);
      }

      return new Tuple<byte, string>(bv, errmsg);
    }

    public static DataTable HtmlTextToDataTable(this string htmlText)
    {
      var table = new DataTable();

      XmlElement htmlElement = HtmlParser.ParseHtml(htmlText);
      var topName = htmlElement.LocalName.ToLower();

      if (topName == "html")
      {
        var node = htmlElement.GetElementsByTagName("table");
        if (node != null)
          htmlElement = node[0] as XmlElement;
      }

      XmlElement tableElem = htmlElement;

      foreach (var trElem in tableElem.GetXmlElementNodes("tr"))
      {
        // tr elem contains th children. add heading to data table.
        if (trElem.ContainsNodes("th"))
        {
          foreach (var thElem in trElem.GetXmlElementNodes("th"))
          {
            var column = new DataColumn(thElem.InnerText);
            table.Columns.Add(column);
          }
        }

        else if (trElem.ContainsNodes("td"))
        {
          var tdTextArray =
            trElem.GetXmlElementNodes("td").Select(c => c.InnerText).ToArray();
          table.Rows.Add(tdTextArray);
        }
      }

      return table;
    }


    /// <summary>
    /// Scan string for char not equal any of array of chars.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InStart"></param>
    /// <param name="InAnyChars"></param>
    /// <returns></returns>
    public static int IndexOfNone(this string InText, int InStart, char[] InNoneChars)
    {
      int ix = InStart;
      int foundPos = -1;
      while (ix < InText.Length)
      {
        int fx = Array.IndexOf<char>(InNoneChars, InText[ix]);
        if (fx == -1)
        {
          foundPos = ix;
          break;
        }
        ix += 1;
      }
      return foundPos;
    }

    /// <summary>
    /// Test if all the characters of the string are digits.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static bool IsAllDigit(this string Text )
    {
      bool allDigits = true;
      if ((Text == null) || (Text.Length == 0))
        allDigits = false;
      else
      {
        foreach (char ch1 in Text)
        {
          if (Char.IsDigit(ch1) == false)
          {
            allDigits = false;
            break;
          }
        }
      }
      return allDigits;
    }

    /// <summary>
    /// String is null, empty or all blanks.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public static bool IsBlank(this string Value)
    {
      if (Value == null)
        return true;
      else if (Value.Length == 0)
        return true;
      else if ((Value[0] == ' ') && (Value.TrimEnd(new char[] { ' ' }).Length == 0))
        return true;
      else
        return false;
    }

    /// <summary>
    /// text contains a numeric value in exponential notation.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsExcelExponential(this string text)
    {
      bool isExponential = false;
      var lx = text.Length;
      if (text.Length > 2)
      {
        var part1 = text.Substring(0, text.Length - 2);
        var part2 = text.Tail(2);
        var part2a = part2.Substring(0, 1);
        var part2b = part2.Substring(1, 1);
        if ((part2a == "E") && (part2b.IsAllDigit() == true))
          isExponential = true;
      }
      return isExponential;
    }

    public static bool IsExcelNumeric(this string text)
    {
      if (text.IsAllDigit() == true)
        return true;
      else if (text.IsExcelExponential() == true)
        return true;
      else
        return false;
    }

    // --------------------------- IsNotBlank ---------------------------------
    public static bool IsNotBlank(this string InValue)
    {
      return !InValue.IsBlank();
    }

    /// <summary>
    /// test if string is null or zero length.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string Text)
    {
      if (Text == null)
        return true;
      else if (Text.Length == 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// split the input string on the pattern. Return the last item in the array of
    /// split results.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="SplitPattern"></param>
    /// <returns></returns>
    public static string LastSplit( this String Text, string SplitPattern)
    {
      var items = Text.Split(SplitPattern);
      if (items.Length == 0)
        return "";
      else
      {
        var ix = items.Length - 1;
        return items[ix];
      }
    }

    /// <summary>
    /// return a string that is a copy of the input string, with the substring
    /// location in the input string replaced with a marker pattern which does not
    /// occur elsewhere in the text.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="Lgth"></param>
    /// <returns></returns>
    public static Tuple<string, string, string> Mark(
      this String Text, int Start, int Lgth)
    {
      // assign a marker char which does not exist in the string.
      char markerChar = Text.CalcMarkerChar();
      var marker = markerChar.ToString().Repeat(Lgth);
      var rv = Text.SubstringSplit(Start, Lgth);
      var origSubstr = rv.Item2;
      var markedText = rv.Item1 + marker + rv.Item3;
      return new Tuple<string, string, string>(markedText, marker, origSubstr);
    }

    /// <summary>
    /// pad the string with equal amount of blanks on the left and the right. The
    /// result is to center the text within the specified width.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Width"></param>
    /// <returns></returns>
    public static string PadCenter(this string Text, int Width )
    {
      var text = Text;
      var padLx = Width - Text.Length;
      var leftLx = padLx / 2;
      var rightLx = padLx - leftLx;

      if (leftLx > 0)
        text = text.PadLeft(text.Length + leftLx);
      if (rightLx > 0)
        text = text.PadRight(text.Length + rightLx);

      return text;
    }

    public static string RemoveLastChar(this string Text, bool AllowEmptyString = false)
    {
      if (Text.Length == 0)
      {
        if (AllowEmptyString == false)
          throw new ApplicationException("no characters to remove");
        else
          return Text;
      }
      else
        return Text.Substring(0, Text.Length - 1);
    }

    public static string RemoveFirstChar(this string Text, bool AllowEmptyString = false)
    {
      if (Text.Length == 0)
      {
        if (AllowEmptyString == false)
          throw new ApplicationException("no characters to remove");
        else
          return Text;
      }
      else
        return Text.Substring(1, Text.Length - 1);
    }

    /// <summary>
    /// Repeat the pattern until the resulting length is achieved
    /// </summary>
    /// <param name="InPattern"></param>
    /// <param name="InResultLength"></param>
    /// <returns></returns>
    public static string Repeat(this string Text, int Lgth)
    {
      StringBuilder sb = new StringBuilder(Lgth);
      while (true)
      {
        if (sb.Length >= Lgth)
          break;
        sb.Append(Text);
      }

      // clip results in case length exceeded.
      if (sb.Length > Lgth)
        sb.Length = Lgth;

      return sb.ToString();
    }

    /// <summary>
    /// repeat the text string the specified number of times.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Count"></param>
    /// <returns></returns>
    public static string RepeatCount(this string Text, int Count)
    {
      if (Count == 1)
        return Text;
      else
      {
        var sb = new StringBuilder();
        for(int cx = 0; cx < Count; ++cx)
        {
          sb.Append(Text);
        }
        return sb.ToString();
      }
    }

    /// <summary>
    /// replace all occurances of Pattern char with the Replace char.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InPattern"></param>
    /// <param name="InReplace"></param>
    /// <returns></returns>
    public static string ReplaceAll(this string Text, char Pattern, char Replace)
    {
      var sb = new StringBuilder(Text.Length);
      for (int ix = 0; ix < Text.Length; ++ix)
      {
        char ch1 = Text[ix];
        if (ch1 == Pattern)
          sb.Append(Replace);
        else
          sb.Append(ch1);
      }
      return sb.ToString();
    }

    /// <summary>
    /// replace all instances of Find string in Text with Replace string.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Find"></param>
    /// <param name="Replace"></param>
    /// <returns></returns>
    public static string ReplaceAll(this string Text, string Find, string Replace)
    {
      var sb = new StringBuilder();
      var ix = 0;
      while (ix < Text.Length)
      {
        var fx = Text.IndexOf(Find, ix);

        // text passed over unti the found text.
        int lx;
        if (fx == -1)
          lx = Text.Length - ix;
        else
          lx = fx - ix;

        // append the passed over text to result string.
        if (lx > 0)
          sb.Append(Text.Substring(ix, lx));

        // append the replacement text in result string.
        if (fx != -1)
          sb.Append(Replace);

        // advance past found text to next scan start pos.
        if (fx == -1)
          ix = Text.Length;
        else
          ix = fx + Find.Length;
      }
      return sb.ToString();
    }

    public static string RightJustify(this string Text, int Width )
    {
      var s1 = Text.PadLeft(Width);
      return s1;
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

    /// <summary>
    /// is the first character of the string equal to the character.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Character"></param>
    /// <returns></returns>
    public static bool StartsWith(this string Text, char Character)
    {
      if (Text == null)
        return false;
      else if (Text.Length == 0)
        return false;
      else if (Text[0] == Character)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Split the string at the substring location.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static Tuple<string,string,string> SubstringSplit(
      this string Text, int Start, int Length )
    {
      string beforeText = null;
      if ((Start > 0) && (Text.Length > 0))
        beforeText = Text.Substring(0, Start);
      else
        beforeText = "";

      string substr = Text.SubstringLenient(Start, Length);

      string afterText = null;
      int bx = Start + Length;
      if (bx >= Text.Length)
        afterText = "";
      else
        afterText = Text.Substring(bx);

      return new Tuple<string, string, string>(beforeText, substr, afterText);
    }

    /// <summary>
    /// return the substring that starts at Start and ends at End.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public static string SubstringStartEnd(this string Text, int Start, int End)
    {
      int lx = End - Start + 1;
      return Text.Substring(Start, lx);
    }

    /// <summary>
    /// split the srcdta text on the "insert before pos" position.
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public static Tuple<string, string> SplitOnInsertBeforePos(
      this string Text, int Pos)
    {
      string beforeText = null;
      string afterText = null;

      // adjust pos so it does not exceed length.
      int pos = Pos;
      if (pos > Text.Length)
        pos = Text.Length;

      if (pos > 0)
      {
        int lx = Pos;
        beforeText = Text.Substring(0, lx);
      }
      else
        beforeText = "";

      if (pos < Text.Length)
        afterText = Text.Substring(pos);
      else
        afterText = "";

      return new Tuple<string, string>(beforeText, afterText);
    }

    /// <summary>
    /// Return a sub string that is lenient in terms of the begin pos and length
    /// exceeding the end of the string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <returns></returns>
    public static string SubstringLenient(this string InString, int InBx, int InLx)
    {
      int remLx = InString.Length - InBx;
      if (InBx >= InString.Length)
        return "";
      else if (InLx <= remLx)
        return (InString.Substring(InBx, InLx));
      else
        return (InString.Substring(InBx));
    }

    /// <summary>
    /// Return a sub string that is lenient in terms of the begin pos
    /// exceeding the end of the string.
    /// </summary>
    public static string SubstringLenient(this string InString, int InBx)
    {
      int remLx = InString.Length - InBx;
      if (InBx >= InString.Length)
        return "";
      else
        return InString.Substring(InBx);
    }


    // ------------------------ Tail -----------------------------
    // return the ending xx number of chars from the string.
    public static string Tail(this string Text, int Length)
    {
      int Lx = Length;
      if (Text.Length < Length)
        Lx = Text.Length;
      if (Lx == 0)
        return "";
      else
      {
        int Bx = Text.Length - Lx;
        return (Text.Substring(Bx, Lx));
      }
    }

    // ---------------------------- Talley -----------------------------
    // count the number of occurances of pattern in string.
    // ( there is an issue of how much to advance the cursor after each find of the
    //   pattern. Either advance by 1 char, or advance by the length of the pattern.
    //   This method advances by length of the pattern. Use TalleyOverlap to advance
    //   by one char after each find. )
    public static int Talley(this string InString, string InPattern)
    {
      int talleyCx = 0;
      int Bx = 0;
      while (true)
      {
        if (Bx > (InString.Length - InPattern.Length))
          break;
        int Fx = InString.IndexOf(InPattern, Bx);
        if (Fx == -1)
          break;
        ++talleyCx;
        Bx = Fx + InPattern.Length;
      }
      return talleyCx;
    }
    // ---------------------------- Talley -----------------------------
    // count the number of occurances of pattern in string.
    public static int Talley(this string InString, char[] InPattern)
    {
      int talleyCx = 0;
      for (int Ix = 0; Ix < InString.Length; ++Ix)
      {
        char ch1 = InString[Ix];
        for (int Jx = 0; Jx < InPattern.Length; ++Jx)
        {
          if (InPattern[Jx] == ch1)
          {
            ++talleyCx;
            break;
          }
        }
      }
      return talleyCx;
    }
    public static byte[] ToEbcdicBytes(this string InString)
    {
      var encoding = global::System.Text.Encoding.GetEncoding(37);
      byte[] bytes = encoding.GetBytes(InString);
      return bytes;
    }

    /// <summary>
    /// enquote the text so that it is shown as text. Not interpreted as numeric
    /// or date or some other type of value.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToExcelTextFormated(this string text)
    {
      var s1 = "=(" + text.DoubleEnquote( ) + ")";
      return s1;
    }

    public static List<string> ToListString( this string input)
    {
      var ls = new List<string>();
      ls.Add(input);
      return ls;
    }

    /// <summary>
    /// convert the array of strings to a single text line, with NewLine at the
    /// end of each line.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static string ToAllText(this string[] lines)
    {
      if (lines == null)
        return null;
      else
      {
        var sb = new StringBuilder();
        foreach (var line in lines)
        {
          sb.Append(line + Environment.NewLine);
        }
        return sb.ToString();
      }
    }

    public static SecureString ToSecureString(this string Text)
    {
      SecureString secstr = new SecureString();
      foreach (var ch1 in Text)
      {
        secstr.AppendChar(ch1);
      }
      return secstr;
    }

    /// <summary>
    /// return a MemoryStream representation of the input text.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static MemoryStream ToMemoryStream(this string Text)
    {
      UnicodeEncoding uniEncoding = new UnicodeEncoding( ) ;
      byte[] inputBytes = uniEncoding.GetBytes(Text);
      MemoryStream ms = new MemoryStream(inputBytes);
      ms.Seek(0, SeekOrigin.Begin);
      return ms;
    }

    /// <summary>
    /// trim whitespace from front and end of string.
    /// If string is null, return empty string.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string TrimWhitespace(this string Text)
    {
      if (Text == null)
        return "";
      else
      {
        char[] wsChars = { ' ', '\t', '\n', '\r' };
        return Text.Trim(wsChars);
      }
    }

    public static string TrimEndWhitespace(this string Text)
    {
      if (Text == null)
        return "";
      else
      {
        char[] wsChars = { ' ', '\t', '\n', '\r' };
        return Text.TrimEnd(wsChars);
      }
    }

    public static DateTime? TryParseDateTime(this string InText)
    {
      DateTime vlu;
      bool rv = DateTime.TryParse(InText, out vlu);
      if (rv == false)
        return null;
      else
        return vlu;
    }

    public static DateTime? TryParseDateTimeExact(this string Text, string Format)
    {
      DateTime dtm;
      var rc = DateTime.TryParseExact(
        Text, Format, null, DateTimeStyles.None, out dtm);
      if (rc == true)
        return dtm;
      else
        return null;
    }

    public static decimal? TryParseDecimal(string InText)
    {
      decimal vlu;
      bool rv = Decimal.TryParse(InText, out vlu);
      if (rv == false)
        return null;
      else
        return vlu;
    }

    public static int? TryParseInt32( this string Text, int? FailParseValue = null)
    {
      int iv;
      var rc = Int32.TryParse(Text, out iv);
      if (rc == false)
        return FailParseValue;
      else
        return iv;
    }

    /// <summary>
    /// get rid of. Caller should call substring and then TryParseInt32.
    /// this way TryParseInt32 can accept a 2nd parm of FailParseValue.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InStart"></param>
    /// <param name="InLength"></param>
    /// <returns></returns>
    public static int? xTryParseInt32(this string InText, int InStart, int InLength)
    {
      int intValue = 0;
      bool rc = Int32.TryParse(
          InText.Substring(InStart, InLength), out intValue);
      if (rc == false)
        return null;
      else
        return intValue;
    }

    public static DateTime? TryParseIsoDate(this string InText)
    {
      DateTime vlu;
      string dateText = InText;

      // 8 digits. assume yyyymmdd.
      if (dateText.Length == 8)
      {
        dateText = dateText.Substring(0, 4) + '-' + dateText.Substring(4, 2) +
          '-' + dateText.Substring(6, 2);
      }

      bool rv = DateTime.TryParse(dateText, out vlu);
      if (rv == false)
        return null;
      else
        return vlu;
    }

    /// <summary>
    /// Reconstitute a string that has been processed by the Mark method and had a 
    /// substring within it replaced with a marker pattern. 
    /// Find the marker pattern and replace it with the original substring.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Marker"></param>
    /// <param name="Orig"></param>
    /// <returns></returns>
    public static string Unmark(this String Text, string Marker, string OrigSubstr)
    {
      int fx = Text.IndexOf(Marker);
      if (fx == -1)
        throw new ApplicationException("Marker not found in text");
      var rv = Text.SubstringSplit(fx, Marker.Length);
      var origText = rv.Item1 + OrigSubstr + rv.Item3;
      return origText;
    }

    /// <summary>
    /// Split the text string into an array of words.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string[] WordSplit(this string Text)
    {
      List<string> words = new List<string>();
      char[] wsChars = new char[] { ' ', '\t' };
      int ix = 0;
      while (ix < Text.Length)
      {
        // find the start of the next word.
        int fx = Text.IndexOfNone(ix, wsChars);
        if (fx == -1)
          break;
        int bx = fx;

        // find the char that follows the end of the word.
        fx = Text.IndexOfAny(wsChars, bx);
        int lx = 0;
        if (fx == -1)
          lx = Text.Length - bx;
        else
          lx = fx - bx;

        words.Add(Text.Substring(bx, lx));

        ix = bx + lx;
      }

      return words.ToArray();
    }

    /// <summary>
    /// use XOR to encrypt/decrypt a text string.
    /// Call with plain text and a key to encrypt.
    /// Call a 2nd time with the encrypted text and the same key to decrypt.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string XORText(this string text, int key)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < text.Length; i++)
      {
        int charValue = Convert.ToInt32(text[i]);    // get the ASCII value of the char
        charValue ^= key;                            // xor the value
        sb.Append(char.ConvertFromUtf32(charValue)); // convert back to string
      }
      return sb.ToString();
    }
  }
}
