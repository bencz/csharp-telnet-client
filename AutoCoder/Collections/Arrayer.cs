using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections
{
  public static class Arrayer
  {

    /// <summary>
    /// test if any of the boolean array entries are true.
    /// </summary>
    /// <param name="InArray"></param>
    /// <returns></returns>
    public static bool AnyTrue(bool[] InArray)
    {
      bool anyTrue = false;
      for (int ix = 0; ix < InArray.Length; ++ix)
      {
        if (InArray[ix] == true)
        {
          anyTrue = true;
          break;
        }
      }
      return anyTrue;
    }

    public static string[] CharArrayToStringArray(char[] InCharArray)
    {
      string[] rv = null;
      if (InCharArray == null)
        rv = null;
      else
      {
        rv = new string[InCharArray.Length];
        for (int ix = 0; ix < InCharArray.Length; ++ix)
        {
          rv[ix] = InCharArray[ix].ToString();
        }
      }
      return rv;
    }

    public static T[] Concat<T>(T InValue1, T[] InValue2)
    {
      int sx = 0;
      if (InValue1 != null)
        sx += 1;
      sx = Concat_CalcNeededSx(sx, InValue2);

      int usedSx = 0;
      T[] res = new T[sx];

      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      if ( InValue2 != null )
        usedSx = Concat_CopyTo(res, usedSx, InValue2);
      
      return res;
    }

    public static T[] Concat<T>(T[] InValue1, T InValue2)
    {
      int Sx = Concat_CalcNeededSx(1, InValue1);
      int usedSx = 0;
      T[] res = new T[Sx];
      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      usedSx = Concat_CopyTo(res, usedSx, InValue2);
      return res;
    }

    /// <summary>
    /// Concatenate one char array to another.
    /// </summary>
    /// <param name="InValue1"></param>
    /// <param name="InValue2"></param>
    /// <returns></returns>
    public static T[] Concat<T>(T[] InValue1, T[] InValue2)
    {
      T[] res = null;
      int usedSx = 0;
      int Sx = Concat_CalcNeededSx(InValue1, InValue2, null, null);
      res = new T[Sx];
      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      if (InValue2 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue2);
      return res;
    }

    /// <summary>
    /// Concatenate set of char arrays.
    /// </summary>
    /// <param name="InValue1"></param>
    /// <param name="InValue2"></param>
    /// <param name="InValue3"></param>
    /// <returns></returns>
    public static T[] Concat<T>(T[] InValue1, T[] InValue2, T[] InValue3)
    {
      T[] res = null;
      int usedSx = 0;
      int Sx = Concat_CalcNeededSx(InValue1, InValue2, InValue3, null);
      res = new T[Sx];
      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      if (InValue2 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue2);
      if (InValue3 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue3);
      return res;
    }

    public static T[] Concat<T>(
      T[] InValue1, T[] InValue2, T[] InValue3, T[] InValue4)
    {
      T[] res = null;
      int usedSx = 0;
      int Sx = Concat_CalcNeededSx(InValue1, InValue2, InValue3, InValue4);
      res = new T[Sx];
      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      if (InValue2 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue2);
      if (InValue3 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue3);
      if (InValue4 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue4);
      return res;
    }

    public static T[] Concat<T>(
      T[] InValue1, T[] InValue2, T[] InValue3, T[] InValue4, T[] InValue5)
    {
      T[] res = null;
      int usedSx = 0;
      int Sx = Concat_CalcNeededSx(InValue1, InValue2, InValue3, InValue4);
      Sx = Concat_CalcNeededSx(Sx, InValue5);
      res = new T[Sx];
      if (InValue1 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue1);
      if (InValue2 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue2);
      if (InValue3 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue3);
      if (InValue4 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue4);
      if (InValue5 != null)
        usedSx = Concat_CopyTo(res, usedSx, InValue5);
      return res;
    }

    /// <summary>
    /// use the Concat_CalcNeededSx and Concat_CopyTo when it is possible that some
    /// of the char arrays to concat can be null.
    /// </summary>
    /// <param name="InValue1"></param>
    /// <param name="InValue2"></param>
    /// <param name="InValue3"></param>
    /// <param name="InValue4"></param>
    /// <param name="InValue5"></param>
    /// <returns></returns>
    public static int Concat_CalcNeededSx<T>(
      T[] InValue1, T[] InValue2, T[] InValue3, T[] InValue4)
    {
      int Sx = 0;
      if (InValue1 != null)
        Sx += InValue1.Length;
      if (InValue2 != null)
        Sx += InValue2.Length;
      if (InValue3 != null)
        Sx += InValue3.Length;
      if (InValue4 != null)
        Sx += InValue4.Length;
      return Sx;
    }

    public static int Concat_CalcNeededSx<T>(int InNeededSx, T[] InValue)
    {
      if (InValue == null)
        return InNeededSx;
      else
        return InNeededSx + InValue.Length;
    }

    public static int Concat_CalcNeededSx<T>(int InNeededSx, T[] InValue1, T[] InValue2)
    {
      int Sx = InNeededSx;
      if (InValue1 != null)
        Sx += InValue1.Length;
      if (InValue2 != null)
        Sx += InValue2.Length;
      return Sx;
    }

    public static int Concat_CalcNeededSx<T>(
      int InNeededSx, T[] InValue1, T[] InValue2, T[] InValue3)
    {
      int Sx = InNeededSx;
      if (InValue1 != null)
        Sx += InValue1.Length;
      if (InValue2 != null)
        Sx += InValue2.Length;
      if (InValue3 != null)
        Sx += InValue3.Length;
      return Sx;
    }

    public static int Concat_CopyTo<T>(T[] InConcat, int InUsedSx, T[] InValue)
    {
      InValue.CopyTo(InConcat, InUsedSx);
      return InUsedSx + InValue.Length;
    }

    public static int Concat_CopyTo<T>(T[] InConcat, int InUsedSx, T InValue)
    {
      InConcat[InUsedSx] = InValue;
      return InUsedSx + 1;
    }

    /// <summary>
    /// concat chars of fac2 which dont exist in fac1 to end of fac1.
    /// </summary>
    /// <param name="InValue1"></param>
    /// <param name="InValue2"></param>
    /// <returns></returns>

    public static T[] ConcatDistinct<T>(T[] InFac1, T[] InFac2)
    {
      T[] res = null;

      if ((InFac1 == null) && (InFac2 == null))
        res = null;
      else if (InFac2 == null)
        res = InFac1 ;
      else if (InFac1 == null)
        res = InFac2 ;
      else
      {
        int containCx = Arrayer.ContainsCount<T>(InFac1, InFac2);
        int addCx = InFac2.Length - containCx;
        if (addCx == 0)
          res = InFac1;
        else
        {
          res = new T[InFac1.Length + addCx];
          Array.Copy(InFac1, 0, res, 0, InFac1.Length);
          int ix = InFac1.Length;
          foreach (T ch1 in InFac2)
          {
            if (Array.IndexOf<T>(InFac1, ch1) == -1)
            {
              res[ix] = ch1;
              ++ix;
            }
          }
        }
      }
      return res ;
    }

    /// <summary>
    /// test if array1 contains all the values in array2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InArray1"></param>
    /// <param name="InArray2"></param>
    /// <returns></returns>
    public static bool ContainsAll<T>(T[] InArray1, T[] InArray2)
    {
      bool containsAll = true;
      if ((InArray2 == null) || (InArray2.Length == 0))
        containsAll = false;
      else if ((InArray1 == null) || (InArray1.Length == 0))
        containsAll = false;
      else
      {
        foreach (T ch1 in InArray2)
        {
          if (Array.IndexOf<T>(InArray1, ch1) == -1)
          {
            containsAll = false;
            break;
          }
        }
      }
      return containsAll;
    }

    /// <summary>
    /// test if array1 contains any of the values in array2.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InArray1"></param>
    /// <param name="InArray2"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(T[] InArray1, T[] InArray2)
    {
      bool containsAny = false;
      if ((InArray2 == null) || (InArray2.Length == 0))
        containsAny = false;
      else if ((InArray1 == null) || (InArray1.Length == 0))
        containsAny = false;
      else
      {
        foreach (T ch1 in InArray2)
        {
          if (Array.IndexOf<T>(InArray1, ch1) >= 0)
          {
            containsAny = true;
            break;
          }
        }
      }
      return containsAny;
    }

    /// <summary>
    /// count number of items in array2 which exist in array1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InArray1"></param>
    /// <param name="InArray2"></param>
    /// <returns></returns>
    public static int ContainsCount<T>(T[] InFac1, T[] InFac2)
    {
      int cx = 0;
      if ((InFac1 == null) || (InFac2 == null))
        cx = 0;
      else
      {
        foreach (T ch1 in InFac2)
        {
          if (Array.IndexOf<T>(InFac1, ch1) != -1)
            ++cx;
        }
      }
      return cx;
    }

    /// <summary>
    /// test if array is null or zero length.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InFac1"></param>
    /// <returns></returns>
    public static bool IsEmpty<T>(T[] InFac1)
    {
      if (InFac1 == null)
        return true;
      else if (InFac1.Length == 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// return the last item in an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InArray"></param>
    /// <returns></returns>
    public static T LastItem<T>(T[] InArray) where T: class 
    {
      if (InArray.Length == 0)
        return null;
      else
      {
        int ix = InArray.Length - 1;
        return InArray[ix];
      }
    }

    /// <summary>
    /// count number of items in array2 which are missing from array1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InArray1"></param>
    /// <param name="InArray2"></param>
    /// <returns></returns>
    public static int MissingCount<T>(T[] InFac1, T[] InFac2)
    {
      int cx = 0;
      if (InFac2 == null)
        cx = 0;
      else if (InFac1 == null)
        cx = InFac2.Length;
      else
      {
        foreach (T ch1 in InFac2)
        {
          if (Array.IndexOf<T>(InFac1, ch1) == -1)
            ++cx;
        }
      }
      return cx;
    }


    // filter out the empty strings from the array of newline strings.
    // ( this is needed because the associated NewLineLeadingChars array
    //   cannot represent an empty string. )
    public static string[] PruneEmptyStrings(string[] InStrings)
    {
      string[] pruned = null;
      int cx = 0;
      foreach (string s1 in InStrings)
      {
        if ((s1 != null) && (s1.Length > 0))
          ++cx;
      }

      if (cx == InStrings.Length )
        pruned = InStrings;
      else
      {
        pruned = new string[cx];
        int ix = 0;
        foreach (string s1 in InStrings)
        {
          if (( s1 != null ) && ( s1.Length > 0 ))
          {
            pruned[ix] = s1;
            ++ix;
          }
        }
      }

      return pruned;
    }


    public static char[] StringArrayToLeadingCharArray(string[] InStringArray)
    {
      char[] leadingChar;
      if (InStringArray == null)
      {
        leadingChar = null;
      }
      else
      {
        leadingChar = new char[InStringArray.Length];
        int ix = 0;
        foreach (string s1 in InStringArray)
        {
          if ((s1 == null) || (s1.Length == 0))
            throw new ApplicationException(
              "null or zero length string cannot be represented as leading char");
          leadingChar[ix] = s1[0];
          ++ix;
        }
      }
      return leadingChar;
    }

    public static T[] TrimUnused<T>(T[] InArray, int InUsedSx)
    {
      if (InUsedSx == InArray.Length)
        return InArray;
      else
      {
        T[] trimmed = new T[InUsedSx];
        Array.Copy(InArray, trimmed, InUsedSx);
        return trimmed;
      }
    }

  }
}
