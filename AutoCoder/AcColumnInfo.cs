using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using AutoCoder.Text;

namespace AutoCoder
{
  /// <summary>
  /// Column information. Used when prompting the column.
  /// </summary>
  public class AcColumnInfo
  {
    string mColumnName;
    string mHeadingText ;
    List<string> mAllowedValues;
    string mErrorText;

    public AcColumnInfo()
    {
    }

    public List<string> AllowedValues
    {
      get
      {
        if (mAllowedValues == null)
          mAllowedValues = new List<string>();
        return mAllowedValues;
      }
      set { mAllowedValues = value; }
    }

    public string ColumnName
    {
      get { return mColumnName; }
      set { mColumnName = value; }
    }

    public string ErrorText
    {
      get { return mErrorText; }
      set { mErrorText = value; }
    }

    public bool HasAllowedValues
    {
      get
      {
        if ((mAllowedValues == null) ||
          (mAllowedValues.Count == 0))
          return false;
        else
          return true;
      }
    }

    public string HeadingText
    {
      get { return mHeadingText; }
      set { mHeadingText = value; }
    }

    public AcColumnInfo AddAllowedValue(string[] InValues)
    {
      AllowedValues.AddRange(InValues);
      return this;
    }

    public static AcColumnInfo Parse(string InString)
    {
      AcColumnInfo info = new AcColumnInfo();

      CsvCursor csv = new CsvCursor(InString);

      // ColumnName
      csv.NextValue();
        info.ColumnName = csv.ItemValue ;

      // HeadingText
      csv.NextValue();
      info.HeadingText = csv.ItemValue;

      // AllowedValues
      csv.NextValue();
      if ( csv.ItemValue != null )
        info.AllowedValues =
          Stringer.ParseSerializedListOfString( csv.ItemValue );

      // ErrorText
      csv.NextValue();
      info.ErrorText = csv.ItemValue;

      return info;
    }

    /// <summary>
    /// return the ColumnInfo in CSV form.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder(10000);
      sb.Append(Stringer.ToParseableString(ColumnName));
      sb.Append(", ");
      if ( HeadingText != null )
        sb.Append(Stringer.ToParseableString(HeadingText));
      sb.Append(", ");
      if ( mAllowedValues != null )
        sb.Append( "[" + Stringer.xListToString(AllowedValues) + "]") ;
      sb.Append(", ");
      if (ErrorText != null)
        sb.Append(Stringer.ToParseableString(ErrorText));

      return sb.ToString();
    }
  }

  /// <summary>
  /// Dictionary object holding AcColumnInfo of a set of columns.
  /// </summary>
  [TypeConverter(typeof(AcColumnInfoDictionaryConverter))]
  public class AcColumnInfoDictionary : Dictionary<string, AcColumnInfo>
  {

    public AcColumnInfo AddColumn(string InColumnName)
    {
      AcColumnInfo info = new AcColumnInfo();
      info.ColumnName = InColumnName;
      base.Add(info.ColumnName, info);
      return info;
    }

    private static string PairToString(KeyValuePair<string, AcColumnInfo> InPair)
    {
      string strPair =
        Stringer.ToParseableString(InPair.Key) + ", [" +
        InPair.Value.ToString() + "]" ;
      return strPair;
    }

    public static AcColumnInfoDictionary Parse(string InString)
    {
      AcColumnInfoDictionary vlus = new AcColumnInfoDictionary();
      CsvCursor csr = new CsvCursor(InString);

      while (true)
      {
        csr.NextValue();
        if (csr.IsEndOfString == true)
          break;
        else if ( csr.ItemCursor.Word.IsBraced == false )
          throw new ApplicationException(
            "serialized AcColumnInfoDictionary string in invalid format");

          // each comma sep item is itself a CSV string consisting of the 
          // dictionary key and the AcColumnInfo ( itself in CSV form )
        else
        {
          string strKeyValuePair = csr.ItemValue ;
          KeyValuePair<string,AcColumnInfo> pair = ParsePair( strKeyValuePair ) ;
          vlus.Add( pair.Key, pair.Value ) ;
        }
      }
      return vlus;
    }

    /// <summary>
    /// crack the csvString that holds two items. The dictionary key and then the
    /// AcColumnInfo dictionary value ( itself in CsvString form ).
    /// </summary>
    /// <param name="InStrPair"></param>
    /// <returns></returns>
    private static KeyValuePair<string, AcColumnInfo> ParsePair(string InStrPair)
    {
      string pairKey = null;
      AcColumnInfo pairValue = null;

      CsvCursor csr = new CsvCursor(InStrPair);

      csr.NextValue();
      if (csr.IsEndOfString == false)
        pairKey = csr.ItemValue;

      csr.NextValue();
      if (csr.IsEndOfString == false)
        pairValue = AcColumnInfo.Parse(csr.ItemValue);

      return new KeyValuePair<string, AcColumnInfo>(pairKey, pairValue);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder(10000);
      foreach (KeyValuePair<string, AcColumnInfo> pair in this)
      {
        string strPair = PairToString(pair);

        if (sb.Length > 0)
          sb.Append(", ");
        sb.Append("[" + strPair + "]");
      }
      return sb.ToString();
    }
  }


  public class AcColumnInfoDictionaryConverter : TypeConverter
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

    // convert from string to AcColumnInfoDictionary 
    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue)
    {
      if (InValue is string)
        return AcColumnInfoDictionary.Parse((string)InValue);
      else
        return base.ConvertFrom(context, culture, InValue);
    }

    // Convert from AcColumnInfoDictionary to string.
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue,
      Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        AcColumnInfoDictionary vlus = InValue as AcColumnInfoDictionary;
        return (vlus.ToString());
      }
      else
        return base.ConvertTo(context, culture, InValue, destinationType);
    }
  }



}
