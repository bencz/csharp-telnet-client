using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Parse
{
  /// <summary>
  /// a line within a parse buffer.  A parse buffer is a list of strings 
  /// concatenated together with a newline pattern between each line.
  /// </summary>
  public class ParseBufferLine
  {

    public ParseBufferLine(string LineText, int LineNbr, int BufferPos)
    {
      _LineText = LineText;
      _LineNbr = LineNbr;
      _BufferPos = BufferPos;
    }

    string _LineText;
    public string LineText
    {
      get { return _LineText; }
    }

    int _LineNbr;
    public int LineNbr
    {
      get { return _LineNbr; }
    }

    int _BufferPos;
    public int BufferPos
    {
      get { return _BufferPos; }
    }

    public int BufferEndPos
    {
      get
      {
        int ex = _BufferPos + _LineText.Length - 1;
        return ex;
      }
    }
  }

}
