using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using AutoCoder.Text;

namespace AutoCoder.WebControls.RowPrompt
{
  [TypeConverter(typeof(RowPromptStylesConverter))]
  public class RowPromptStyles
  {
    string mPanelClass = null ;
    string mTitleClass = null ;
    string mHeadingClass = null ;
    string mFieldClass = null ;

    public RowPromptStyles()
    {
    }

    public RowPromptStyles(string InPanelClass)
    {
      mPanelClass = InPanelClass;
    }

    public RowPromptStyles(
      string InPanelClass, string InTitleClass, string InHeadingClass,
      string InFieldClass)
    {
      mPanelClass = InPanelClass;
      mTitleClass = InTitleClass;
      mHeadingClass = InHeadingClass;
      mFieldClass = InFieldClass;
    }

    public string FieldClass
    {
      get
      {
        if ( mFieldClass == null )
          return "field" ;
        else
          return mFieldClass ;
      }
      set { mFieldClass = value; }
    }

    public string HeadingClass
    {
      get
      {
        if ( mHeadingClass == null )
          return "columnHeading" ;
        else
          return mHeadingClass ;
      }
      set { mHeadingClass = value; }
    }

    public string PanelClass
    {
      get
      {
        if ( mPanelClass == null )
          return "rowPrompt" ;
        else
          return mPanelClass ;
      }
      set { mPanelClass = value; }
    }

    public string TitleClass
    {
      get 
      {
        if ( mTitleClass == null )
          return "title" ;
        else
        return mTitleClass;
      }
      set { mTitleClass = value; }
    }

    public static RowPromptStyles FromString(string InString)
    {
      RowPromptStyles rps = new RowPromptStyles( ) ;
      int ix = 0;
      string[] vlus = new string[4] ;
      CsvCursor csr = new CsvCursor(InString);
      for( ix = 0 ; ix <= 3 ; ++ix )
      {
        csr.NextValue( ) ;
        if ( csr.IsBraced == true )
          throw new ApplicationException(
            "serialized RowPromptStyles string contains invalid format" ) ;
        vlus[ix] = csr.ItemValue ;
      }

      // load the parsed values
      rps.mPanelClass = vlus[0] ;
      rps.mTitleClass = vlus[1] ;
      rps.mHeadingClass = vlus[2] ;
      rps.mFieldClass = vlus[3] ;

      return rps;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder(10000);
      sb.Append(Stringer.GetNonNull(mPanelClass));
      sb.Append(",");
      sb.Append(Stringer.GetNonNull(mTitleClass));
      sb.Append(",");
      sb.Append(Stringer.GetNonNull(mHeadingClass));
      sb.Append(",");
      sb.Append(Stringer.GetNonNull(mFieldClass));
      return sb.ToString();
    }

  }


  public class RowPromptStylesConverter : TypeConverter
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

    // convert from string to RowPromptStyles 
    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue)
    {
      if (InValue is string)
      {
        return RowPromptStyles.FromString((string)InValue);
      }
      return base.ConvertFrom(context, culture, InValue);
    }

    // Overrides the ConvertTo method of TypeConverter.
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object InValue,
      Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        RowPromptStyles msgs = (RowPromptStyles)InValue;
        return (msgs.ToString());
      }
      return base.ConvertTo(context, culture, InValue, destinationType);
    }
  }

}
