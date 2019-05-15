using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext.Shapes.LineClasses;
using System.Windows;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Triangles;

namespace AutoCoder.Ext.Shapes.Triangles
{
  public class RightTriangle
  {
    public LineCoordinates AdjSide
    { get; set; }

    public LineCoordinates OppSide
    { get; set; }

    private TriangleVertex _AdjVertex;
    /// <summary>
    /// the point at the vertex of the adjacent side and the hypotenuse
    /// </summary>
    public TriangleVertex AdjVertex
    {
      get
      {
        if (_AdjVertex == null)
        {
          if ((AdjSide != null) && (OppSide != null))
          {
            Point? pos = null;
            if (AdjSide.Start.Equals(OppSide.Start))
              pos = AdjSide.End;
            else if (AdjSide.Start.Equals(OppSide.End))
              pos = AdjSide.End;
            else if (AdjSide.End.Equals(OppSide.Start))
              pos = AdjSide.Start;
            else if (AdjSide.End.Equals(OppSide.End))
              pos = AdjSide.Start;
            else
              throw new ApplicationException("adjacent and opposite sides do not meet");

            // length of the adjacent line
            var adjLgth = this.AdjSide.Length;
            var oppLgth = this.OppSide.Length;

            // the angle of this adjacent line with the hypotenuse.
            var adjTan = Tangent.CalcTangent(adjLgth, oppLgth);
            var adjAngle = adjTan.ToAngle();

            // store the adjacent vertex info
            _AdjVertex = new TriangleVertex()
            {
              Angle = adjAngle,
              Location = pos.Value
            };
          }
        }
        return _AdjVertex;
      }
    }

    private TriangleVertex _OppVertex;
    /// <summary>
    /// the point at the vertex of the opposite side and the hypotenuse
    /// </summary>
    public TriangleVertex OppVertex
    {
      get
    {
        if (_OppVertex == null)
        {
          if ((AdjSide != null) && (OppSide != null))
          {
            Point? pos = null;
            if (OppSide.Start.Equals(AdjSide.Start))
              pos = OppSide.End;
            else if (OppSide.Start.Equals(AdjSide.End))
              pos = OppSide.End;
            else if (OppSide.End.Equals(AdjSide.Start))
              pos = OppSide.Start;
            else if (OppSide.End.Equals(AdjSide.End))
              pos = OppSide.Start;
            else
              throw new ApplicationException("adjacent and opposite sides do not meet");

            // length of the adjacent line
            var adjLgth = this.AdjSide.Length;
            var oppLgth = this.OppSide.Length;

            // the angle of this adjacent line with the hypotenuse.
            var adjTan = Tangent.CalcTangent(adjLgth, oppLgth);
            var adjAngle = adjTan.ToAngle();

            // compute the angle of the opposite line and the hypotenuse from the 
            // angle of the adjacent line.
            var oppAngle = 90 - this.AdjVertex.Angle ;

            // store the opposite vertex info
            _OppVertex = new TriangleVertex()
            {
              Angle = oppAngle,
              Location = pos.Value
            };
          }
        }
        return _OppVertex;
      }
    }
  }
}
