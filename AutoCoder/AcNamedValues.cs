using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using AutoCoder.Text ;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;

namespace AutoCoder
{

  /// <summary>
  /// Dictionary object holding ValueName,Value pairs
  /// </summary>
  [TypeConverter(typeof(AcNamedValuesConverter))]
  public class AcNamedValues : Dictionary<string, string>
  {
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder(10000);
      foreach (KeyValuePair<string, string> pair in this)
      {
        string strPair = PairToString(pair);

        if (sb.Length > 0)
          sb.Append(",");
        sb.Append(strPair);
      }
      return sb.ToString();
    }

    private static string PairToString(KeyValuePair<string, string> InPair)
    {
      string strPair =
        "[" + Stringer.ToParseableString(InPair.Key) + "," +
        Stringer.ToParseableString(InPair.Value) + "]";
      return strPair;
    }

    private static KeyValuePair<string, string> ParsePair(string InStrPair)
    {
      string pairKey = null;
      string pairValue = null;
      CsvCursor csr = new CsvCursor(InStrPair);

      csr.NextValue();
      if ( csr.IsAtItemValue == true )
        pairKey = csr.ItemValue ;
      else
        throw new ApplicationException( 
          "key missing from serialized key/value pair" ) ;

      // the value part
      csr.NextValue( ) ;
      pairValue = csr.ItemValue ;

      return new KeyValuePair<string, string>(pairKey, pairValue);
    }

    public static AcNamedValues Parse(string InString)
    {
      AcNamedValues vlus = new AcNamedValues();

      TextTraits traits = new TextTraits();
      traits.OpenNamedBracedPatterns.Replace("[", Text.Enums.DelimClassification.OpenNamedBraced);
      traits.DividerPatterns.AddDistinct(",", Text.Enums.DelimClassification.DividerSymbol) ;
      WordCursor csr = Scanner.PositionBeginWord(InString, traits);
      while (true)
      {
        csr = Scanner.ScanNextWord(InString, csr);
        if (csr.IsEndOfString == true)
          break;
        else if
          ((csr.IsDelimOnly == false) &&
          (csr.Word.Class == WordClassification.ContentBraced))
        {
          KeyValuePair<string, string> pair = ParsePair(csr.Word.BracedText);
          vlus.Add(pair.Key, pair.Value);
        }
        else
          throw new ApplicationException(
            "serialized AcNamedValues string in invalid format");
      }
      return vlus;
    }
  }


  public class AcNamedValuesConverter : TypeConverter
  {

    // this TypeConverter can convert from string.
    public override bool CanConvertFrom(
      ITypeDescriptorContext context,
      Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }
      return base.CanConvertFrom(context, sourceType);
    }

    // convert from string to AcNamedValues 
    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue)
    {
      if (InValue is string)
        return AcNamedValues.Parse((string)InValue);
      else
        return base.ConvertFrom(context, culture, InValue);
    }

    // Convert from AcNamedValue to string.
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue,
      Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        AcNamedValues vlus = InValue as AcNamedValues;
        return (vlus.ToString());
      }
      else
        return base.ConvertTo(context, culture, InValue, destinationType);
    }
  }


  public class GenericTypeConverter<T> : TypeConverter where T:class, new( ) 
  {

    // this TypeConverter can convert from string.
    public override bool CanConvertFrom(
      ITypeDescriptorContext context,
      Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }
      return base.CanConvertFrom(context, sourceType);
    }

    // convert from string to T 
    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue)
    {
      if (InValue is string)
      {
        T obj = new T();
//        return obj.Parse( InValue as string ) ;
        return base.ConvertFrom(context, culture, InValue);
      }
      else
        return base.ConvertFrom(context, culture, InValue);
    }

    // Convert from T to string.
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue,
      Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        T vlus = InValue as T ;
        return (vlus.ToString());
      }
      else
        return base.ConvertTo(context, culture, InValue, destinationType);
    }
  }


}