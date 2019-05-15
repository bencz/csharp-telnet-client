using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  /// <summary>
  /// a string bounded by a begin and end position
  /// </summary>
  public class BoundedString
  {
    public int Bx = -1;
    public int Ex = -1;
    public string String = null;

    public BoundedString(string InString)
    {
      String = InString;
      Bx = 0;
      Ex = String.Length - 1;
    }

    public BoundedString(string InString, int InBx)
    {
      String = InString;
      Bx = InBx;
      Ex = InString.Length - 1;
    }

    public BoundedString(string InString, int InBx, int InEx)
    {
      String = InString;
      Bx = InBx;
      Ex = InEx;
    }

    // -------------------- overload the [] operator ( an indexer ) ------------------
    public char this[int InIx]
    {
      get
      {
        if ((InIx < Bx) || (InIx > Ex))
          throw new ApplicationException("index into string is out of bounds");
        return String[InIx];
      }
    }

    public char GetChar(int InIx)
    {
      if (IsOutsideBounds(InIx) == true)
        return '\0';
      else
        return this[InIx];
    }

    public int GetRemainingLength(int InIx)
    {
      int remLx = Ex - InIx + 1;
      return remLx;
    }

    public bool IsOutsideBounds(int Ix)
    {
      if ((Ix < Bx) || (Ix > Ex))
        return true;
      else
        return false;
    }

    public bool IsOutsideBounds(int Ix, int Lx)
    {
      int ex = Ix + Lx - 1;
      if ((Ix < this.Bx) || (ex > Ex))
        return true;
      else
        return false;
    }

    public WordCursor PositionBefore(int InIx, TextTraits InTraits)
    {
      WordCursor csr = new WordCursor();
      ThrowOutsideBounds(InIx);
      csr.Position = RelativePosition.Before;
      csr.WordBx = InIx;
      csr.TextTraits = InTraits;
      return csr;
    }

    /// <summary>
    /// return substring from start pos to bounded end pos
    /// </summary>
    /// <param name="InBx"></param>
    /// <returns></returns>
    public string Substring(int InBx)
    {
      if ((InBx < Bx) || (InBx > Ex))
        throw new ApplicationException("bounded string bounds exceeded");
      int lx = Ex - InBx + 1;
      return String.Substring(InBx, lx);
    }

    /// <summary>
    /// return substring checked within this bounded string
    /// </summary>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <returns></returns>
    public string Substring(int InBx, int InLx)
    {
      if ((InBx < Bx) || (InBx > Ex))
        throw new ApplicationException("bounded string bounds exceeded");
      int remLx = Ex - InBx + 1;
      if (InLx > remLx)
        throw new ApplicationException("substring length exceeds string bounds");
      return String.Substring(InBx, InLx);
    }

    public void ThrowBeforeBounds(int InIx)
    {
      if (InIx < Bx)
        throw new ApplicationException("Position " + InIx.ToString() +
          "before range of bounded string");
    }

    public void ThrowOutsideBounds(int InIx)
    {
      if ((InIx < Bx) || (InIx > Ex))
        throw new ApplicationException("Position " + InIx.ToString() +
          "outside range of bounded string");
    }
  }
}
