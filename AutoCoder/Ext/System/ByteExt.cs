using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class ByteExt
  {
    /// <summary>
    /// add the byte value to the byte[] array which stores a big endian stored value.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <returns></returns>
    public static byte[] AddValue(this byte[] Bytes, byte Value)
    {
      byte carryValue = Value;
      byte[] res = new byte[Bytes.Length];

      // Iterate through the elements of the array from right to left.
      for (int ix = Bytes.Length - 1; ix >= 0; ix--)
      {
        byte bv = Bytes[ix];

        int wk1 = bv + carryValue;
        var wkBytes = BitConverter.GetBytes(wk1);
        bv = wkBytes[0];
        carryValue = wkBytes[1];

        res[ix] = bv;
      }

      return res;
    }

    /// <summary>
    /// return new byte array which contains factor 2 byte array appended to the end
    /// of factor 1 byte array.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <returns></returns>
    public static byte[] Append(this byte[] Fac1, byte[] Fac2)
    {
      byte[] res = new byte[Fac1.Length + Fac2.Length];
      Array.Copy(Fac1, 0, res, 0, Fac1.Length);
      Array.Copy(Fac2, 0, res, Fac1.Length, Fac2.Length);
      return res;
    }

    /// <summary>
    /// return the byte at index location in byte array. If index exceeds bounds
    /// of the array, return 0x00.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Index"></param>
    /// <returns></returns>
    public static byte ByteAt(this byte[] Bytes, int Index)
    {
      if (Index < 0)
        return 0x00;
      else if (Index >= Bytes.Length)
        return 0x00;
      else
        return Bytes[Index];
    }

    /// <summary>
    /// return the bytes of the array as a sequence of chunks. That is, fixed
    /// length sized byte arrays.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="ChunkSx"></param>
    /// <returns></returns>
    public static IEnumerable<byte[]> Chunks(this byte[] Bytes, int ChunkSx)
    {
      int ix = 0;
      while (true)
      {
        int remLx = Bytes.Length - ix;
        int lx = remLx;
        if (lx > ChunkSx)
          lx = ChunkSx;
        if (lx == 0)
          yield break;
        byte[] rv = new byte[lx];
        Array.Copy(Bytes, ix, rv, 0, lx);
        yield return Bytes.SubArray(ix,lx) ;
        ix += lx;
      }
    }

    /// <summary>
    /// test that the contents of fac1 array is equal to contents of fac2 array.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <returns></returns>
    public static bool CompareEqual(this byte[] Fac1, byte[] Fac2)
    {
      if (Fac1.Length != Fac2.Length)
        return false;

      for (int ix = 0; ix < Fac1.Length; ix++)
      {
        if (Fac1[ix] != Fac2[ix])
          return false;
      }
      return true;
    }

    /// <summary>
    /// test that the contents of fac1 array match fac2 array for the length 
    /// specified.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <param name="Fac1Lgth"></param>
    /// <returns></returns>
    public static bool CompareEqual(this byte[] Fac1, byte[] Fac2, int CompareLgth )
    {
      if (Fac1.Length < CompareLgth)
        return false;

      if (Fac2.Length < CompareLgth)
        return false;

      for (int ix = 0; ix < CompareLgth; ix++)
      {
        if (Fac1[ix] != Fac2[ix])
          return false;
      }
      return true;
    }

    /// <summary>
    /// concatenate the two input arrays together.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <returns></returns>
    public static byte[] Concat(this byte[] Fac1, byte[] Fac2)
    {
      int lx = 0;
      int tx = 0;
      if (Fac1 != null)
        lx += Fac1.Length;
      if (Fac2 != null)
        lx += Fac2.Length;

      byte[] res = new byte[lx];
      if ( Fac1 != null )
      {
        Array.Copy(Fac1, 0, res, tx, Fac1.Length);
        tx += Fac1.Length;
      }
      if ( Fac2 != null)
      {
        Array.Copy(Fac2, 0, res, tx, Fac2.Length);
        tx += Fac2.Length;
      }
      return res;
    }


    /// <summary>
    /// copy the input array to the return array. For every byte that matches an item
    /// in the Match array, copy the corr item in the Replace array.
    /// </summary>
    /// <param name="Input"></param>
    /// <param name="Match"></param>
    /// <param name="Replace"></param>
    /// <returns></returns>
    public static byte[] CopyReplace( this byte[] Input, byte[] Match, byte[] Replace)
    {
      var buf = Input.SubArray(0);
      for( int ix = 0; ix < buf.Length; ++ix )
      {
        var b1 = buf[ix];
        var fx = Array.IndexOf(Match, b1);
        if (fx >= 0)
          buf[ix] = Replace[fx];
      }
      return buf;
    }

    /// <summary>
    /// Use DES encryption to decrypt the input Data bytes with the input Key bytes.
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Data"></param>
    /// <param name="Mode"></param>
    /// <returns></returns>
    public static byte[] DecryptDES(
      this byte[] Data, byte[] Key, CipherMode Mode = CipherMode.ECB, byte[] IV = null)
    {
      var tdes = new DESCryptoServiceProvider();
      tdes.Key = Key;

      // init vector.
      if (IV == null)
        tdes.IV = Key;
      else
        tdes.IV = IV;

      // mode of operation. there are 4 modes.
      tdes.Mode = Mode;

      //padding mode(if any extra byte added)
      tdes.Padding = PaddingMode.PKCS7;
      tdes.Padding = PaddingMode.None;

      ICryptoTransform cTransform = tdes.CreateDecryptor();

      // do the actual decrypt of the input data bytes.
      byte[] res = cTransform.TransformFinalBlock(Data, 0, Data.Length);

      // release resources
      tdes.Clear();

      return res;
    }

    /// <summary>
    /// Use DES encryption to encrypt the input Data bytes with the input Key bytes.
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Data"></param>
    /// <param name="Mode"></param>
    /// <returns></returns>
    public static byte[] EncryptDES(
      this byte[] Data, byte[] Key, CipherMode Mode = CipherMode.ECB, byte[] IV = null)
    {
      var tdes = new DESCryptoServiceProvider();
      tdes.Key = Key;

      // init vector.
      if (IV == null)
        tdes.IV = Key;
      else
        tdes.IV = IV;

      // mode of operation. there are 4 modes.
      tdes.Mode = Mode;
      
      // padding mode(if any extra byte added)
      tdes.Padding = PaddingMode.None;

      ICryptoTransform cTransform = tdes.CreateEncryptor();

      // do the actual encrypt of the input data bytes.
      byte[] res = cTransform.TransformFinalBlock(Data, 0, Data.Length);

      // release resources
      tdes.Clear();

      return res;
    }

    /// <summary>
    /// exclusive or the bytes of Fac1 with the Fac2 byte.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <returns></returns>
    public static byte[] ExclusiveOr(this byte[] Fac1, byte Fac2)
    {
      byte[] res = new byte[Fac1.Length];
      {
        for (int ix = 0; ix < Fac1.Length; ++ix)
        {
          res[ix] = (byte)(Fac1[ix] ^ 0x55);
        }
      }
      return res;
    }

    /// <summary>
    /// exclusive or the bytes of Fac1 with the corresponding bytes of Fac2.
    /// </summary>
    /// <param name="Fac1"></param>
    /// <param name="Fac2"></param>
    /// <returns></returns>
    public static byte[] ExclusiveOr(this byte[] Fac1, byte[] Fac2)
    {
      byte[] res = new byte[Fac1.Length];
      {
        for (int ix = 0; ix < Fac1.Length; ++ix)
        {
          if (ix < Fac2.Length)
            res[ix] = (byte)(Fac1[ix] ^ Fac2[ix]);
          else
            res[ix] = Fac2[ix];
        }
      }
      return res;
    }

    /// <summary>
    /// convert the ascii encoded input bytes to a unicode string.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <returns></returns>
    public static string FromAscii(this byte[] Bytes)
    {
      char[] chars = global::System.Text.Encoding.ASCII.GetChars(Bytes);
      return new string(chars);
    }

    public static string FromAscii(this byte[] Bytes, int Start, int Length)
    {
      char[] chars = global::System.Text.Encoding.ASCII.GetChars(Bytes, Start, Length);
      return new string(chars);
    }

    public static string FromAscii(this byte[] Bytes, int Start, int Length, string SepText)
    {
      StringBuilder sb = new StringBuilder();
      for (int ix = 0; ix < Length; ++ix)
      {
        char ch1 = global::System.Text.Encoding.ASCII.GetChars(Bytes, ix + Start, 1)[0];
        sb.Append(ch1 + SepText);
      }
      return sb.ToString();
    }

    /// <summary>
    /// convert the ebcdic encoded input bytes to a unicode string.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <returns></returns>
    public static string FromEbcdic(this byte[] Bytes)
    {
      global::System.Text.Encoding encoding =
        global::System.Text.Encoding.GetEncoding(37); // 37 = ebcdic
      var chars = encoding.GetChars(Bytes);
      return new string(chars);
    }

    /// <summary>
    /// get byte from offset in array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte GetByte(this byte[] Bytes, int Start)
    {
      if (Start >= Bytes.Length)
        throw new Exception("past end of byte array");
      return Bytes[Start];
    }

    /// <summary>
    /// get bytes from offset in array for length specified.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this byte[] Bytes, int Start, int Length)
    {
      byte[] rv = new Byte[Length];
      if (Start + Length > Bytes.Length)
        throw new Exception("past end of byte array");
      Array.Copy(Bytes, Start, rv, 0, Length);
      return rv;
    }

    /// <summary>
    /// get 3 byte big endian int from byte array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static int GetBigEndianInt3(this byte[] Bytes, int Offset)
    {
      if (Offset + 2 >= Bytes.Length)
        throw new Exception("past end of byte array");
      var ip = IntParts.LoadBigEndianInt3(Bytes, Offset);
      return ip.IntValue;
    }

    /// <summary>
    /// get 2 byte big endian int from byte array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static short GetBigEndianShort(this byte[] Bytes, int Offset)
    {
      if (Offset + 1 >= Bytes.Length)
        throw new Exception("past end of byte array");
      var ip = IntParts.LoadBigEndianShort(Bytes, Offset);
      return ip.ShortValue;
    }

    /// <summary>
    /// get 2 byte big endian unsigned int from byte array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ushort GetBigEndianUShort(this byte[] Bytes, int Offset)
    {
      if (Offset + 1 >= Bytes.Length)
        throw new Exception("past end of byte array");
      var ip = IntParts.LoadBigEndianShort(Bytes, Offset);
      return ip.UnsignedShortValue;
    }

    /// <summary>
    /// return the specified number of bytes from the start of the byte array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] Head(this byte[] Bytes, int Length)
    {
      int lx = Length;
      if (lx > Bytes.Length)
        lx = Bytes.Length;
      return Bytes.SubArray(0, lx);
    }

    public static int IndexOf(this byte[] Bytes, byte Pattern)
    {
      var fx = Array.IndexOf(Bytes, Pattern);
      return fx;
    }

    /// <summary>
    /// search for the byte pattern within a stream of bytes.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Pattern"></param>
    /// <returns></returns>
    public static int IndexOf(this byte[] Bytes, int Start, byte[] Pattern)
    {
      int fx = -1;
      int ix = Start;
      byte patFirstByte = Pattern[0];
      int ex = Bytes.Length - Pattern.Length;
      while (ix <= ex )
      {
        if (Bytes[ix] == patFirstByte)
        {
          bool found = true;

          int jx = ix;
          foreach (byte b1 in Pattern)
          {
            if (b1 != Bytes[jx])
            {
              found = false;
              break;
            }
            jx += 1;
          }

          if (found == true)
          {
            fx = ix;
            break;
          }
        }

        ix += 1;
      }

      return fx;
    }

    /// <summary>
    /// return the index of the first byte that is not equal any of the search 
    /// bytes.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="SearchBytes"></param>
    /// <returns></returns>
    public static int IndexOfNotEqual(
      this byte[] Bytes, int Start, byte[] SearchBytes )
    {
      int fx = -1;
      int ix = Start;
      while(ix < Bytes.Length)
      {
        var rx = Array.IndexOf(SearchBytes, Bytes[ix]);
        if ( rx == -1)
        {
          fx = ix;
          break;
        }
        ix += 1;
      }
      return fx;
    }

    /// <summary>
    /// starting from the last byte in the Bytes array, look backwards until a 
    /// byte is found which is not equal all bytes in the SearchBytes array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="SearchBytes"></param>
    /// <returns></returns>
    public static int IndexOfLastNotEqual(
      this byte[] Bytes, byte[] SearchBytes)
    {
      int fx = -1;
      int ix = Bytes.Length - 1;
      while(ix >= 0)
      {
        var rx = Array.IndexOf(SearchBytes, Bytes[ix]);
        if ( rx == -1 )
        {
          fx = ix;
          break;
        }
        ix -= 1;
      }
      return fx;
    }

    /// <summary>
    /// Insert the bytes of the Insert array into the Source array immed after 
    /// the length of the first part of the source array.
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="FirstPartLength"></param>
    /// <param name="Insert"></param>
    /// <returns></returns>
    public static byte[] Insert(
      this byte[] Source, int FirstPartLength, byte[] Insert)
    {
      // resulting array length
      int resultLength = Source.Length + Insert.Length;
      byte[] res = new byte[resultLength];
      int tx = 0;

      if (FirstPartLength > 0)
      {
        Array.Copy(Source, res, FirstPartLength);
        tx += FirstPartLength;
      }

      if (Insert.Length > 0)
      {
        Array.Copy(Insert, 0, res, tx, Insert.Length);
        tx += Insert.Length;
      }

      // remaining length of source array.
      int remLx = Source.Length - FirstPartLength;
      if (remLx > 0)
      {
        Array.Copy(Source, FirstPartLength, res, tx, remLx);
      }

      return res;
    }

    /// <summary>
    /// create and return a new array that is
    /// </summary>
    /// <param name="Input"></param>
    /// <param name="Length"></param>
    /// <param name="PadValue"></param>
    /// <returns></returns>
    public static byte[] PadLeft( this byte[] Input, int Length, byte PadValue )
    {
      // do not pad. Input array is equal or exceeds the pad to length.
      if (Input.Length >= Length)
        return Input;

      byte[] res = new byte[Length];
      var tx = Length - Input.Length;
      Array.Copy(Input, 0, res, tx, Input.Length);

      // apply the pad byte.
      var padLx = Length - Input.Length;
      for( int ix = 0; ix < padLx; ++ix )
      {
        res[ix] = PadValue;
      }

      return res;
    }

    public static byte[] PadRight( this byte[] Input, int Length, byte PadValue )
    {
      // do not pad. Input array is equal or exceeds the pad to length.
      if (Input.Length >= Length)
        return Input;

      byte[] res = new byte[Length];
      Array.Copy(Input, 0, res, 0, Input.Length);

      // apply the pad byte.
      var padLx = Length - Input.Length;
      for (int ix = Input.Length; ix < Length; ++ix)
      {
        res[ix] = PadValue;
      }

      return res;
    }

    /// <summary>
    /// return byte array containing the input byte repeated for the specified length.
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] Repeat( this byte Value, int Length )
    {
      var buf = new byte[Length];
      for( int ix = 0; ix < Length; ++ix )
      {
        buf[ix] = Value;
      }
      return buf;
    }

    /// <summary>
    /// search for all the Find byte[] patterns and replace with Replace byte[]
    /// contents.
    /// </summary>
    /// <param name="Input"></param>
    /// <param name="Find"></param>
    /// <param name="Replace"></param>
    /// <returns></returns>
    public static byte[] ReplaceAll( this byte[] Input, byte[] Find, byte[] Replace)
    {
      int bx = 0;
      int inputLength = Input.Length;
      ByteArrayBuilder ba = new ByteArrayBuilder(inputLength + 100);

      // loop until all bytes of the Input array
      while (bx < inputLength)
      {
        var fx = Input.IndexOf(bx, Find);
        if (fx >= 0)
        {
          var lx = fx - bx;  // length of bytes before the found pattern.
          ba.Append(Input, bx, lx);

          // copy the Replace pattern to the result array.
          ba.Append(Replace);

          // advance in the input array.
          bx = fx + Find.Length;
        }

        // find pattern not found. Copy remaining bytes to result array.
        else
        {
          int remLx = inputLength - bx;
          ba.Append(Input, bx, remLx);
          bx += remLx;
        }
      }

      // return the 
      return ba.ToByteArray();
    }

    /// <summary>
    /// SubArray, as in Substring. Return the array of bytes that are contained within
    /// a large array of bytes.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] SubArray(this byte[] Bytes, int Start, int Length)
    {
      byte[] rv = new Byte[Length];
      Array.Copy(Bytes, Start, rv, 0, Length);
      return rv;
    }

    /// <summary>
    /// return the sub array from start position in the input array to the end of the
    /// array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <returns></returns>
    public static byte[] SubArray(this byte[] Bytes, int Start)
    {
      int lx = Bytes.Length - Start;
      byte[] rv = new Byte[lx];
      Array.Copy(Bytes, Start, rv, 0, lx);
      return rv;
    }
    /// <summary>
    /// return array of bytes from input bytes, starting at Start for length of
    /// Length parm. If start pos is lt 0 return an empty array.
    /// If bounds of the sub array exceed the input array, adjust length 
    /// so as not to exceed end pos of input array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] SubArrayLenient(this byte[] Bytes, int Start, int Length)
    {
      int lx = Length;
      if (Bytes == null)
        lx = 0;
      else
      {
        if (Start < 0)
          lx = 0;

        if (Start > Bytes.Length)
        {
          lx = 0;
        }
      }

      // adjust sub array length in case exceeds length of array.
      if (lx > 0)
      {
        if ((Start + lx) > Bytes.Length)
        {
          lx = Bytes.Length - Start;
        }
      }

      byte[] rv;
      if (lx == 0)
        rv = new byte[0];
      else
      {
        rv = new Byte[Length];
        Array.Copy(Bytes, Start, rv, 0, lx);
      }

      return rv;
    }
    public static byte[] SubArrayLenient(this byte[] Bytes, int Start)
    {
      int lx = 0;
      if (Bytes == null)
        lx = 0;
      else
        lx = Bytes.Length - Start;

      var subArray = Bytes.SubArrayLenient(Start, lx);
      return subArray;
    }

    /// <summary>
    /// copy bytes for length Length from position Start of input Bytes array. Return an
    /// array of size RtnLgth. If RtnLgth exceeds Length pad the bytes to the right of
    /// Length with value Pad.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <param name="RtnLgth"></param>
    /// <param name="Pad"></param>
    /// <returns></returns>
    public static byte[] SubArrayPad(
      this byte[] Bytes, int Start, int Length, int RtnLgth, byte Pad)
    {
      // alloc the array to return
      byte[] rv = new Byte[RtnLgth];

      // copy from input array to return array.
      Array.Copy(Bytes, Start, rv, 0, Length);

      // if lengths dont match, pad the end of return array with Pad bytes.
      int padLx = RtnLgth - Length;
      if (padLx > 0)
      {
        for (int px = 0; px < padLx; ++px)
        {
          rv[Length + px] = Pad;
        }
      }
      return rv;
    }

    public static char GetAsciiChar(this byte Byte)
    {
      char[] chars = global::System.Text.Encoding.ASCII.GetChars(new byte[] {Byte});
      return chars[0];
    }

    /// <summary>
    /// Shifts the bits in an array of bytes to the left.
    /// </summary>
    /// <param name="bytes">The byte array to shift.</param>
    public static byte[] ShiftLeft(this byte[] Bytes)
    {
      bool nextCarry = false;
      byte[] res = new byte[Bytes.Length];

      // Iterate through the elements of the array from right to leftt.
      for (int ix = Bytes.Length - 1; ix >= 0; ix--)
      {
        // If the leftmost bit of the current byte is 1 then we have a carry when
        // shifting the next byte.
        bool thisCarry = nextCarry;
        nextCarry = (Bytes[ix] & 0x80) > 0;
        res[ix] = (byte)(Bytes[ix] << 1);

        // set the right most bit to the carry bit from byte to the right.
        if (thisCarry == true)
        {
          res[ix] |= 0x01;
        }
      }

      return res;
    }

    /// <summary>
    /// return the specified length of bytes at the end of the array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] Tail(this byte[] Bytes, int Length)
    {
      int lx = Length;
      if (lx > Bytes.Length)
        lx = Bytes.Length;
      int ix = Bytes.Length - lx;
      var tailBytes = Bytes.SubArray(ix, lx);
      return tailBytes;
    }

    public static string ToAscii(this byte[] Bytes)
    {
      char[] chars = global::System.Text.Encoding.ASCII.GetChars(Bytes) ;
      return new string(chars);
    }

    public static string ToAscii(this byte[] Bytes, int Start, int Length)
    {
      char[] chars = global::System.Text.Encoding.ASCII.GetChars(Bytes,Start, Length);
      return new string(chars);
    }

    public static string ToAscii(this byte[] Bytes, int Start, int Length, string SepText )
    {
      StringBuilder sb = new StringBuilder();
      for (int ix = 0; ix < Length; ++ix)
      {
        char ch1 = global::System.Text.Encoding.ASCII.GetChars(Bytes,ix + Start,1)[0] ;
        sb.Append(ch1 + SepText);
      }
      return sb.ToString();
    }

    /// <summary>
    /// convert the bytes of the array to decimal values.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <param name="SepChar"></param>
    /// <returns></returns>
    public static string ToDecimal(
      this byte[] Bytes, int Start, int Length,
      char? SepChar = null)
    {
      var sb = new StringBuilder();
      for (int ix = 0; ix < Length; ++ix)
      {
        // the seperator character.
        if ((sb.Length > 0) && (SepChar != null))
          sb.Append(SepChar.Value);

        // add the external form of the byte to the result string.
        int v1 = Bytes[Start + ix];
        sb.Append(v1.ToString( ));
      }

      return sb.ToString();
    }

    /// <summary>
    /// convert each byte in byte array to decimal form.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="SepChar"></param>
    /// <returns></returns>
    public static string ToDecimal(
      this byte[] Bytes, char? SepChar = null)
    {
      int bx = 0;
      int lx = Bytes.Length;
      var s1 = ToDecimal(Bytes, bx, lx, SepChar);
      return s1;
    }

    public static string EbcdicBytesToString(this byte[] Bytes)
    {
      global::System.Text.Encoding encoding =
        global::System.Text.Encoding.GetEncoding(37);
      var chars = encoding.GetChars(Bytes);
      return new string(chars);
    }

    /// <summary>
    /// convert the bytes from ebcdic to ascii. in the case of an unprintable ascii
    /// char, insert the hex value of the ebcdic byte in the output text.
    /// Method is intended for viewing the text value of a byte stream.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <returns></returns>
    public static string EbcdicBytesToPrintableAscii(this byte[] Bytes )
    {
      var sb = new StringBuilder();

      global::System.Text.Encoding encoding =
        global::System.Text.Encoding.GetEncoding(37);
      var text = encoding.GetString(Bytes);

      for( int ix = 0; ix < text.Length; ++ix )
      {
        var ch1 = text[ix];
        if (ch1.IsPrintable())
          sb.Append(ch1);
        else
        {
          var b1 = Bytes[ix];
          var s1 = '/' + b1.ToHex() + ' ';
          sb.SpaceSeparatorAppend(s1);
        }
      }

      return sb.ToString();
    }

    public static byte[] HexTextToBytes(this string HexText)
    {
      var ba = new ByteArrayBuilder();

      // make sure the line is padded on right with at least 1 blank.
      var curLine = HexText.Trim() + "    ";

      // process 3 char chunks on  the current line.
      int ix = 0;
      while (true)
      {
        var chunk = curLine.Substring(ix, 3);
        if (chunk == "   ")
          break;

        var rv = chunk.HexTextToByte();
        var errmsg = rv.Item2;
        if (errmsg != null)
          break;
        ba.Append(rv.Item1);

        ix += 3;
      }

      return ba.ToByteArray();
    }

    /// <summary>
    /// Convert a byte value to 2 char hex external form.
    /// </summary>
    /// <param name="InByte"></param>
    /// <returns></returns>
    public static string ToHex(this byte Byte)
    {
      string s1 = Byte.ToString("X");
      s1 = s1.PadLeft(2, '0');
      return s1;
    }

    /// <summary>
    /// convert an array of bytes to hex external form.
    /// </summary>
    /// <param name="InBytes"></param>
    /// <param name="InStart"></param>
    /// <param name="InLength"></param>
    /// <returns></returns>
    public static string ToHex(
      this byte[] Bytes, int Start, int Length,
      char? SepChar = null)
    {
      var sb = new StringBuilder();
      for (int ix = 0; ix < Length; ++ix)
      {
        // the seperator character.
        if ((sb.Length > 0) && ( SepChar != null ))
          sb.Append(SepChar.Value);

        // add the external form of the byte to the result string.
        string s1 = Bytes[Start + ix].ToHex( );
        sb.Append(s1);
      }

      return sb.ToString();
    }

    /// <summary>
    /// convert entire byte array to hex external form.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="SepChar"></param>
    /// <returns></returns>
    public static string ToHex(
      this byte[] Bytes, char? SepChar = null)
    {
      int bx = 0;
      int lx = Bytes.Length;
      var s1 = ToHex(Bytes, bx, lx, SepChar);
      return s1;
    }

    /// <summary>
    /// report the contents of the byte array as lines of hex text.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="BytesPerLine"></param>
    /// <returns></returns>
    public static IEnumerable<string> ToHexReport( this byte[] Bytes, int BytesPerLine = 16)
    {
      List<string> report = new List<string>();
      foreach( var chunk in Bytes.Chunks(BytesPerLine))
      {
        var reportLine = chunk.ToHex(' ');
        report.Add(reportLine);
      }
      return report;
    }

    public static MemoryStream ToMemoryStream(this byte[] Bytes)
    {
      var ms = new MemoryStream(Bytes);
      return ms;
    }

    public static string ToUtf8(this byte[] Bytes)
    {
      char[] chars = global::System.Text.Encoding.UTF8.GetChars(Bytes);
      return new string(chars);
    }

    /// <summary>
    /// check that an ascii eol pattern is starting at the Start position.
    /// Return the length of the eol pattern.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <returns></returns>
    private static int CheckAsciiEndOfLine(this byte[] Bytes, int Start)
    {
      int fx = -1;
      int remLx = Bytes.Length - Start;
      if ((remLx >= 2) && (Bytes[Start] == 0x0d) && (Bytes[Start + 1] == 0x0a))
        fx = 2;
      else if ( remLx >= 1 )
      {
        if (( Bytes[Start] == 0x0a ) || ( Bytes[Start] == 0x0c ) 
          || ( Bytes[Start] == 0x0d ))
          fx = 1 ;
        else
          fx = -1 ;
      }
      else
        fx = -1 ;

      return fx ;
    }

    /// <summary>
    /// Scan bytes until ascii end of line pattern. 
    /// Return the bytes including the EOL pattern.
    /// If EOL pattern is not found, return the entire input byte array.
    /// </summary>
    /// <param name="Bytes"></param>
    /// <param name="Start"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static Tuple<byte[], int> ScanAsciiTextLine(
      this byte[] Bytes, int Start, int Length)
    {
      int tx = -1;
      int eolLx = -1;
      int ix = Start;
      int ex = Start + Length - 1;
      while (ix <= ex)
      {
        var lx = Bytes.CheckAsciiEndOfLine(ix);
        if (lx != -1)
        {
          tx = ix + lx - 1;
          eolLx = lx;
          break;
        }
        ix += 1;
      }

      if (tx == -1)
      {
        tx = Start + Length - 1;
        eolLx = -1;
      }

      var lineBytes = Bytes.SubArray(Start, tx - Start + 1);
      return new Tuple<byte[], int>(lineBytes, eolLx);
    }

    public static Tuple<string, string> ToHexUpperLower(
      this byte[] Bytes, int Start, int Length)
    {
      var upper = new StringBuilder();
      var lower = new StringBuilder();

      for (int ix = 0; ix < Length; ++ix)
      {
        string s1 = Bytes[Start + ix].ToHex();
        upper.Append(s1[0]);
        lower.Append(s1[1]);
      }

      return new Tuple<string, string>(upper.ToString(), lower.ToString());
    }
  }
}
