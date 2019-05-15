using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Scan;

namespace AutoCoder.Text
{
  public class CsvBuilder
  {
    StringBuilder mWip = null;
    TextTraits mTraits;
    bool mEncapsulateValues = false;
    char cQuoteChar = '"';

    public CsvBuilder()
    {
      mTraits = new TextTraits();
      mTraits.DividerPatterns.AddDistinct(",", Enums.DelimClassification.DividerSymbol );
      mTraits.OpenNamedBracedPatterns.Replace("(", Enums.DelimClassification.OpenNamedBraced) ;
      mTraits.QuoteEncapsulation = QuoteEncapsulation.Double;
    }

    public CsvBuilder(TextTraits InTextTraits, bool InEncapsulateValues)
    {
      mTraits = InTextTraits;
      mEncapsulateValues = InEncapsulateValues;
    }

    public bool EncapsulateValues
    {
      get { return mEncapsulateValues; }
      set { mEncapsulateValues = value; }
    }

    public TextTraits TextTraits
    {
      get { return mTraits; }
    }

    /// <summary>
    /// add a value to the end of the string.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public void Append(string InValue)
    {
      // prepare the value to be added to the CSV string.
      string prepValue;
      if (InValue == null)
        InValue = "";
      if ((ShouldQuoteValue(InValue) == true) ||
        (InValue.Length == 0))
      {
        prepValue = Stringer.Enquote(InValue, cQuoteChar, mTraits.QuoteEncapsulation);
        if (EncapsulateValues == true)
        {
          prepValue = "_qv(" + prepValue + ")";
        }
      }
      else
        prepValue = InValue;

      // add the value to the string.
      AppendAsIs(prepValue);
    }

    /// <summary>
    /// Add an integer value to the comma sep value string.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public void Append(int InValue)
    {
      AppendAsIs(InValue.ToString());
    }

    /// <summary>
    /// Add a boolean value to the comma sep value string.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public void Append(bool InValue)
    {
      AppendAsIs(InValue.ToString());
    }

    /// <summary>
    /// Add an array of strings as a value to the comma sep value string. ( the array
    /// is its converted to CsvString form and enclosed in parenthesis before being added
    /// to the string.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public void Append(string[] InValue)
    {
      if (InValue == null)
      {
        string Value = null;
        Append(Value);
      }
      else
      {
        CsvBuilder cb = new CsvBuilder(this.TextTraits, this.EncapsulateValues);
        foreach (string vlu in InValue)
        {
          cb.Append(vlu);
        }
        AppendAsIs("_sa( " + cb.ToString() + " )");
      }
    }

    /// <summary>
    /// Append a string, as is, to the CsvString. 
    /// </summary>
    /// <param name="InValue"></param>
    public void AppendAsIs(string InValue)
    {
      if (mWip == null)
      {
        mWip = new StringBuilder();
      }

      // add the value to the string.
      if (mWip.Length > 0)
        mWip.Append(", ");
      mWip.Append(InValue);
    }

    bool ShouldQuoteValue(string InValue)
    {
      int Fx = InValue.IndexOfAny(new char[] { ' ', '\t', '\"', ',', '(' });
      if (Fx >= 0)
        return true;
      else
        return false;
    }

    public override string ToString()
    {
      if ( mWip == null )
        return "" ;
      else
        return mWip.ToString( ) ;
    }
  }
}
