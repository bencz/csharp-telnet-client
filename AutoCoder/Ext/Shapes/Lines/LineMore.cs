using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Windows.Lines;

namespace AutoCoder.Ext.Shapes.LineClasses
{
  /// <summary>
  /// class stores a line and then more info about the line.
  /// </summary>
  public class LineMore
  {
    public LineMore(Line Line)
    {
      this.Line = Line;
    }

    Line _Line;
    public Line Line
    {
      get { return _Line; }
      set
      {
        _Line = value;
      }
    }


    LineCoordinates _LineCoor;
    public LineCoordinates LineCoor
    {
      get
      {
        if (_LineCoor == null)
          _LineCoor = this.Line.GetCoordinates( ) ;
        return _LineCoor;
      }
    }
  }
}
