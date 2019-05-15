using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Text.Location
{
  /// <summary>
  /// value that identifies a text line in a text stream.
  /// </summary>
  public class LineIdentifier
  {
    public int? IntValue
    {
      get;
      set;
    }

    public Guid? GuidValue
    { get; set;}

    public LineIdentifier( )
    {
      this.GuidValue = null;
      this.IntValue = null;
    }

    public LineIdentifier(int IntValue)
    {
      this.GuidValue = null;
      this.IntValue = IntValue;
    }

    public LineIdentifier(Guid GuidValue)
    {
      this.IntValue = null;
      this.GuidValue = GuidValue;
    }

    public override bool Equals(object obj)
    {
      if (obj is LineIdentifier)
      {
        var other = obj as LineIdentifier;
        if (this.IntValue != null)
        {
          if (other.IntValue == null)
            return false;
          else
            return (this.IntValue.Value == other.IntValue.Value);
        }
        else if (this.GuidValue != null)
        {
          if (other.GuidValue == null)
            return false;
          else
            return (this.GuidValue.Value == other.GuidValue.Value);
        }
        else
        {
          if (other.IsNull)
            return true;
          else
            return false;
        }
      }
      else
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public bool IsNull
    {
      get
      {
        if (this.IntValue != null)
          return false;
        else if (this.GuidValue != null)
          return false;
        else
          return true;
      }
    }

    public override string ToString()
    {
      if (this.IntValue != null)
        return this.IntValue.Value.ToString();
      else if (this.GuidValue != null)
        return this.GuidValue.Value.ToString();
      else
        return "";
    }
  }

  public static class LineIdentifierExt
  {
    public static bool IsEqual(this LineIdentifier LineId, LineIdentifier OtherLineId)
    {
      if ((LineId == null) && (OtherLineId == null))
        return true;
      else if (LineId == null)
        return false;
      else if (OtherLineId == null)
        return false;
      else
        return LineId.Equals(OtherLineId);
    }

    public static LineIdentifier LineIdentifierOrDefault(
      this XElement Elem, XNamespace Namespace, LineIdentifier Default = null)
    {
      if (Elem == null)
        return Default;
      else if (Elem.Value.Trim().Length == 0)
        return Default;
      else
      {
        Guid? guidValue = Elem.Element(Namespace + "GuidValue").GuidOrDefault(null);
        int? intValue = Elem.Element(Namespace + "IntValue").IntOrDefault(null);
        LineIdentifier lineId = new LineIdentifier()
        {
          GuidValue = guidValue,
          IntValue = intValue
        };

        return lineId;
      }
    }

    public static XElement ToXElement(this LineIdentifier LineId, XName ElemName)
    {
      XElement xelem = null;

      if (LineId == null)
        xelem = new XElement(ElemName, null);

      else if (LineId.IntValue != null)
      {
        xelem = new XElement(ElemName,
          new XElement("IntValue", LineId.IntValue.Value.ToString()));
      }
      else if (LineId.GuidValue != null)
      {
        xelem = new XElement(ElemName,
          new XElement("GuidValue", LineId.GuidValue.Value.ToString()));
      }
      else
        xelem = new XElement(ElemName, null);

      return xelem;
    }
  }
}
