using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;

namespace AutoCoder.Xml
{
  public class XmlUnit
  {
    XmlUnit mNxUnit;
    XmlUnitCode mCode = XmlUnitCode.None;
    int mBx = -1;
    int mLx = -1;
    WordCursor mNameWord = null;
    WordCursor mEncodedAttributeValueWord = null;
    string mUnitValue = null;
    List<XmlUnit> mSubUnits = null;

    public XmlUnit()
    {
    }

    public int Bx
    {
      get { return mBx; }
      set { mBx = value; }
    }

    public WordCursor EncodedAttributeValueWord
    {
      get { return mEncodedAttributeValueWord; }
      set
      {
        // error check. 
        if (value != null)
        {
          if (value.Word == null)
            throw new ApplicationException("xml attribute value is missing");
          else if (value.Word.IsQuoted == false)
            throw new
              ApplicationException("xml attribute value is not enclosed in quotes");
        }
        mEncodedAttributeValueWord = value;
      }
    }

    /// <summary>
    /// Unit end position
    /// </summary>
    public int Ex
    {
      get
      {
        if ((mBx == -1) || (mLx == -1))
          return -1;
        else
          return (mBx + mLx - 1);
      }
      set
      {
        if (mBx == -1)
          throw new ApplicationException("Bx is not yet set");
        else
          mLx = value - mBx + 1;
      }
    }

    public int Lx
    {
      get { return mLx; }
      set { mLx = value; }
    }

    public WordCursor NameWord
    {
      get { return mNameWord; }
      set { mNameWord = value; }
    }

    public XmlUnit NextUnit
    {
      get { return mNxUnit; }
      set { mNxUnit = value; }
    }

    public List<XmlUnit> SubUnits
    {
      get { return mSubUnits; }
    }

    public XmlUnitCode UnitCode
    {
      get { return mCode; }
      set { mCode = value; }
    }

    public string UnitName
    {
      get
      {
        if (mNameWord == null)
          return null;
        else
          return mNameWord.Word.Value;
      }
    }

    /// <summary>
    /// the value of the unit, decoded from its xml encoded form in 
    /// the document stream.
    /// </summary>
    public string UnitValue
    {
      get
      {
        if (mUnitValue != null)
          return mUnitValue;
        else if (mEncodedAttributeValueWord != null)
        {
          string dequotedValue = mEncodedAttributeValueWord.Word.DequotedValue;
          string decodedValue = XmlStream.DecodeXmlString(dequotedValue);
          return decodedValue;
        }
        else
          return null;
      }
      set { mUnitValue = value; }
    }

    public XmlUnit AddAttribute(
      WordCursor InNameWord, WordCursor InEncodedAttributeValueWord)
    {
      if (mSubUnits == null)
        mSubUnits = new List<XmlUnit>();
      XmlUnit AttributeUnit = new XmlUnit();
      mSubUnits.Add(AttributeUnit);

      AttributeUnit.Bx = InNameWord.WordBx;
      AttributeUnit.Ex = InEncodedAttributeValueWord.WordEx;
      AttributeUnit.UnitCode = XmlUnitCode.Attribute;
      AttributeUnit.NameWord = InNameWord;
      AttributeUnit.EncodedAttributeValueWord = InEncodedAttributeValueWord;
      return AttributeUnit;
    }
  }
}
