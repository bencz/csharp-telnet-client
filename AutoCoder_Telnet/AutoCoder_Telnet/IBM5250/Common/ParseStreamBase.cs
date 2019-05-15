using AutoCoder.Ext.System;
using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Common
{
  public abstract class ParseStreamBase
  {
    public string Errmsg
    { get; set; }

    protected byte[] InputBytes
    { get; private set; }

    /// <summary>
    /// start pos of this data stream command in the InputBytes byte stream.
    /// </summary>
    protected int BytesStart
    { get; private set; }

    /// <summary>
    /// byte length of this data stream command.
    /// </summary>
    protected int BytesLength
    { get; set; }

    public string ItemName
    { get; set; }

    public ParseStreamBase( )
    {
      this.InputBytes = null;
      this.BytesStart = 0;
      this.BytesLength = 0;
    }
    public ParseStreamBase( InputByteArray InputArray, string ItemName )
    {
      this.InputBytes = InputArray.Bytes;
      this.BytesStart = InputArray.Index;
      this.BytesLength = 0;
      this.ItemName = ItemName;
    }
    public ParseStreamBase(InputByteArray InputArray)
    {
      this.InputBytes = InputArray.Bytes;
      this.BytesStart = InputArray.Index;
      this.BytesLength = 0;
      this.ItemName = null;
    }

    public virtual string ToHexString()
    {
      // get up to 5 bytes that follow the bytes of the order.
      var followingBytes = this.GetFollowingBytes(5);

      return "Lgth: " + this.BytesLength + " Bytes: " +
        this.UsedBytes?.ToHex(' ') +
        " Following bytes:" + followingBytes.ToHex(' ');
    }

    /// <summary>
    /// the bytes from the input stream from which the order was constructed.
    /// </summary>
    public byte[] UsedBytes
    {
      get
      {
        if (this.InputBytes == null)
          return null;
        else if (this.BytesLength == 0)
          return new byte[] { };
        else
        {
          // adjust length for now if it exceeds input bytes.
          int lx = this.BytesLength;
          if (this.InputBytes.Length < (this.BytesStart + lx))
            lx = this.InputBytes.Length - this.BytesStart;
          return this.InputBytes.SubArray(this.BytesStart, lx);
        }
      }
    }

    /// <summary>
    /// return byte array containing bytes that immediately follow the bytes of this
    /// order in the input byte stream.
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    private byte[] GetFollowingBytes(int Length)
    {
      int bx = this.BytesStart + this.BytesLength;
      return this.InputBytes.SubArrayLenient(bx, Length);
    }

    /// <summary>
    /// return array of text lines which list the contents of the SetBufferAddress 
    /// order.
    /// </summary>
    /// <returns></returns>
    public virtual string[] ToReportLines()
    {
      List<string> lines = new List<string>();
      lines.Add(this.ItemName);
      lines.Add(this.ToHexString());
      lines.Add(this.ToString());

      return lines.ToArray();
    }

  }
}
