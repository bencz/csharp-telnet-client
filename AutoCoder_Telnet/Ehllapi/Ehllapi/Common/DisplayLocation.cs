using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing ;
using AutoCoder.Forms;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Ehllapi.Common
{
  public class DisplayLocation : IComparer<DisplayLocation>
  {
    int mRow = 0;
    int mColumn = 0;

    public DisplayLocation()
    {
    }

    public DisplayLocation(int InRow, int InColumn)
    {
      mRow = InRow;
      mColumn = InColumn;
    }

    public DisplayLocation(CharPoint InCharLoc)
    {
      mRow = InCharLoc.Y + 1;
      mColumn = InCharLoc.X + 1;
    }

    public override bool Equals(object obj)
    {
      DisplayLocation fac2 = obj as DisplayLocation;
      if (fac2 == null)
        return false;
      else if (this == fac2)
        return true;
      else
        return false;
    } 

    public static bool operator ==(DisplayLocation InFac1, DisplayLocation InFac2)
    {
      bool null1 = object.ReferenceEquals( InFac1, null ) ;
      bool null2 = object.ReferenceEquals( InFac2, null ) ;

      if (( null1 == true ) && ( null2 == true ))
        return true ;
      else if (( null1 == true ) || ( null2 == true ))
        return false ;
      else if ((InFac1.Row == InFac2.Row) &&
        (InFac1.Column == InFac2.Column))
        return true;
      else
        return false;
    }

    public static bool operator !=(DisplayLocation InFac1, DisplayLocation InFac2)
    {
      object obj1 = (object)InFac1;
      object obj2 = (object)InFac2;
      if ((obj1 == null) && (obj2 == null))
        return false;
      else if (obj1 == null)
        return true;
      else if (obj2 == null)
        return true;
      else if ((InFac1.Row != InFac2.Row) ||
        (InFac1.Column != InFac2.Column))
        return true;
      else
        return false;
    }

    public static bool operator <=(DisplayLocation InFac1, DisplayLocation InFac2)
    {
      int rv = InFac1.Compare(InFac1, InFac2);
      if (rv <= 0)
        return true;
      else
        return false;
    }

    public static bool operator >=(DisplayLocation InFac1, DisplayLocation InFac2)
    {
      int rv = InFac1.Compare(InFac1, InFac2);
      if (rv >= 0)
        return true;
      else
        return false;
    }

    public int Row
    {
      get { return mRow; }
      set { mRow = value; }
    }

    public int Column
    {
      get { return mColumn; }
      set { mColumn = value; }
    }

    public int ToLinear( PresentationSpaceDim InDim)
    {
      int lx = InDim.Width * ( mRow - 1 );
      lx += mColumn;
      return lx;
    }

    public override string ToString()
    {
      return mRow.ToString() + ", " + mColumn.ToString();
    }

    public CharPoint ToCharPoint( )
    {
      return new CharPoint( Column - 1, Row - 1 ) ;
    }

    #region IComparer<DisplayLocation> Members

    public int Compare(DisplayLocation InFac1, DisplayLocation InFac2)
    {
      if (InFac1.Row < InFac2.Row)
        return -1;
      else if (InFac1.Row > InFac2.Row)
        return 1;
      else if (InFac1.Column < InFac2.Column)
        return -1;
      else if (InFac1.Column > InFac2.Column)
        return 1;
      else
        return 0;
    }

    #endregion

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }

  public static class DisplayLocationExt
  {
    public static XElement ToXElement(this DisplayLocation Loc, XName Name)
    {
      if (Loc == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("Row", Loc.Row),
            new XElement("Col", Loc.Column));
        return xe;
      }
    }

    public static DisplayLocation DisplayLocationOrDefault(
      this XElement Elem, XNamespace ns, 
      DisplayLocation Default = null)
    {
      if (Elem == null)
        return Default;
      else
      {
        int rowNx = Elem.Element(ns + "Row").IntOrDefault(0).Value;
        int colNx = Elem.Element(ns + "Col").IntOrDefault(0).Value;
        return new DisplayLocation(rowNx, colNx);
      }
    }
  }
}
