using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder
{
  /// <summary>
  /// Build a large buffer as a sequence of chunks. The GetBuffer method is
  /// called to return a single byte[] that contains a copy of the data from
  /// all the chunks.
  /// </summary>
  public class BufferBuilder
  {
    List<byte[]> mChunks;
    List<int> mChunkLengths;

    public BufferBuilder()
    {
      mChunks = new List<byte[]>();
      mChunkLengths = new List<int>();
    }

    public int LastChunkLength
    {
      get { return mChunkLengths[mChunks.Count-1]; }
      set { mChunkLengths[mChunks.Count - 1] = value; }
    }

    public int Length
    {
      get
      {
        int lx = 0;
        foreach (int chunkLx in mChunkLengths)
        {
          lx += chunkLx;
        }
        return lx;
      }
    }

    public byte[] AppendChunk(int InSize)
    {
      byte[] buf = new byte[InSize];
      mChunks.Add(buf);
      mChunkLengths.Add(InSize);
      return buf;
    }

    /// <summary>
    /// Return a single byte[] buffer that contains the data from each of
    /// the appended chunks.
    /// </summary>
    /// <returns></returns>
    public byte[] GetBuffer()
    {
      byte[] bigbuf = new byte[this.Length];
      int ox = 0;
      for (int ix = 0; ix < mChunks.Count; ++ix)
      {
        Buffer.BlockCopy( mChunks[ix], 0, bigbuf, ox, mChunkLengths[ix]);
      }
      return bigbuf;
    }
  }
}
