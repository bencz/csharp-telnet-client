using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Ftp
{
  public class TransmitBufferItem
  {
    byte[] mBuffer;
    int mUsedSx;
    DateTime mTimestamp;
    CommDirection mDirection;

    public TransmitBufferItem( CommDirection InDirection, int InBufferSx )
    {
      mBuffer = new byte[InBufferSx];
      mUsedSx = 0;
      mTimestamp = DateTime.Now;
      mDirection = InDirection;
    }

    /// <summary>
    /// The space used in the buffer.
    /// </summary>
    public int UsedSx
    {
      get { return mUsedSx; }
      set { mUsedSx = value; }
    }

    /// <summary>
    /// The actual byte[] array of the buffer item.
    /// </summary>
    public byte[] Bytes
    {
      get { return mBuffer; }
    }

    /// <summary>
    /// return the buffer contents as ascii text translated to unicode string.
    /// </summary>
    /// <returns></returns>
    public string ToAsciiText( )
    {
      Encoding ASCII = Encoding.ASCII;
      return ASCII.GetString( mBuffer, 0, mUsedSx ) ;
    }
  }
}
