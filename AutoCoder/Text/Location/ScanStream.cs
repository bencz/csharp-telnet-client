using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.Text.Location
{
  public class ScanStream 
  {

    #region properties

    /// <summary>
    /// the LineLocation returned by last call to FindLineLocation.
    /// </summary>
    private int? CurrentLineIndex
    { get; set; }

    // used to assign next lineid.
    int _HighLineId;

    /// <summary>
    /// the lines of the stream, split on NewLine.
    /// A ScanLine contains the LineId id of the line, the LineLocation location of
    /// the line in the stream and the Text contents of the line.
    /// </summary>
    public List<ScanLine> Lines
    {
      get { return _Lines; }
    }
    List<ScanLine> _Lines;

    /// <summary>
    /// Stream is a concat of the lines, with NewLine between each line.
    /// </summary>
    public string Stream
    {
      get
      {
        if (_Stream == null)
        {
          _Stream = _sb.ToString();
        }
        return _Stream;
      }
    }
    string _Stream;

    // private variable stores the stream as it is built.
    StringBuilder _sb = null;

    #endregion

    public ScanStream()
    {
      _sb = new StringBuilder();
      _Stream = null;
      _HighLineId = 0;
      _Lines = new List<ScanLine>();
      this.CurrentLineIndex = null;
    }

    public ScanStream(string[] Lines)
      :this( )
    {
      foreach (var line in Lines)
      {
        this.AddLine(line);
      }
    }

    public void AddLine(string Text)
    {
      _HighLineId += 1;
      AddLine(Text, new LineIdentifier(_HighLineId));
    }

    public void AddLine(string Text, LineIdentifier LineId)
    {
      var text = this.ScrubNewLineFromEnd(Text);

      // the start pos of the line.
      int startPos = _sb.Length;

      // add to the stream.
      _sb.Append(text + Environment.NewLine);
      _Stream = null;

      // end pos of the line.
      int endPos = _sb.Length - 1;

      // LineLocation stores the LineId of the line and its position within the stream.
      LineLocation loc = new LineLocation(LineId, startPos, endPos);

      // ScanLine stores the text of the line and its LineLocation.
      ScanLine sl = new ScanLine(text, loc);

      // list of lines.
      _Lines.Add(sl);

      // update the highest assigned lineid.
      if (LineId.IntValue.Value > _HighLineId)
      {
        _HighLineId = LineId.IntValue.Value;
      }
    }

    public ScanLine FindLine(LineIdentifier LineId)
    {
      var rv = FindLineLocation(LineId, 0);
      int ix = rv.Item2;
      ScanLine line = this.Lines[ix];
      return line;
    }

    public LineLocation FindLineLocation(LineIdentifier LineId)
    {
      LineLocation found = null;
      int foundIx = 0 ;

      // start the search from last found line index.
      int ix = 0 ;
      if (this.CurrentLineIndex != null)
        ix = this.CurrentLineIndex.Value;

      // search from starting line forward.
      {
        var rv = this.FindLineLocation(LineId, ix);
        found = rv.Item1 ;
        foundIx = rv.Item2 ;
      }

      // not found. seach from line 0 to the end.
      if (found == null)
      {
        var rv = this.FindLineLocation(LineId, 0);
        found = rv.Item1 ;
        foundIx = rv.Item2 ;
      }

      // store the last found line index.
      if (found != null)
        this.CurrentLineIndex = foundIx;

      // error if not found.
      if (found == null)
        throw new ApplicationException("Line not found");

      return found;
    }

    private Tuple<LineLocation,int> FindLineLocation(LineIdentifier LineId, int StartIx)
    {
      LineLocation found = null;
      int ix = StartIx;

      int nbrLines = this.Lines.Count;
      while (true)
      {
        if (ix >= nbrLines)
        {
          found = null;
          break;
        }

        ScanLine scanLine = this.Lines[ix] ;
        if (scanLine.LineId.Equals(LineId))
        {
          found = scanLine.Location;
          break;
        }

        // advance to the next line.
        ix += 1;
      }

      return new Tuple<LineLocation,int>(found,ix);
    }

    /// <summary>
    /// remove newline and any whitespace that follows from the text line.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    private string ScrubNewLineFromEnd(string Text)
    {
      string returnText = Text;
      int fx = returnText.IndexOf(Environment.NewLine);

      // NewLine found in the string.
      if (fx != -1)
      {

        // look for a non blank, non new line from the location of the NewLine
        // to the end of the string.
        int noneIx = Text.IndexOfNone(fx, new char[] { ' ', '\r', '\n' });

        // nothing but blanks to the end. trim it all.
        if (noneIx == -1)
        {
          returnText = returnText.Substring(0, fx);
        }
      }
      return returnText;
    }

    public string Substring(int Start)
    {
      return this.Stream.Substring(Start);
    }

    public string Substring(int Start, int Length)
    {
      return this.Stream.Substring(Start, Length);
    }

    public override string ToString()
    {
      if (this.Stream == null)
        return "ScanStream. Empty.";
      else if (this.Stream.Length > 2000)
        return this.Stream.Length.ToString( ) + " " 
          + this.Stream.Substring(0, 2000) + "...";
      else
        return this.Stream.Length.ToString( ) + " " + this.Stream;
    }

    public string[] ToStringArray()
    {
      List<string> textLines = new List<string>();
      foreach (var scanLine in this.Lines)
      {
        textLines.Add(scanLine.Text);
      }
      return textLines.ToArray();
    }
  }
}
