using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Ext.Shapes;
using System.Windows.Controls;

namespace AutoCoder.Ext.Controls
{
  /// <summary>
  /// more info related to the drawing of a UIElement.
  /// </summary>
  public class DrawMore
  {
    public DrawMore( Shape ThisShape)
    {
      this.ThisShape = ThisShape;
    }

    private List<ShapeConnection> _ConnectedToShapes;

    /// <summary>
    /// list of shapes that this shape is connected to.
    /// </summary>
    public List<ShapeConnection> ConnectedToShapes
    {
      get
      {
        if (_ConnectedToShapes == null)
          _ConnectedToShapes = new List<ShapeConnection>();
        return _ConnectedToShapes;
      }
      set { _ConnectedToShapes = value; }
    }

    public void RedrawConnections(Canvas Canvas)
    {
      // save the list of connections. The create a new list as the connections are to
      // be redrawn.
      var connectedToShapes = this.ConnectedToShapes;
      this.ConnectedToShapes = new List<ShapeConnection>();

      foreach (var shapeConn in connectedToShapes)
      {
        // remove the connection line(s).
        shapeConn.RemoveConnectionLines(Canvas);

        // remove the connect from the ToShape back to this shape.
        shapeConn.ToShape.RemoveConnection(this.ThisShape);

        Canvas.DrawRouteBetweenShapes(this.ThisShape, shapeConn.ToShape);
      }
    }

    public void RemoveConnection(Shape ToShape)
    {
      // find the ShapeConnection that stores the connection to the ToShape.
      ShapeConnection findConn = null;
      foreach (var shapeConn in this.ConnectedToShapes)
      {
        if (shapeConn.ToShape == ToShape)
        {
          findConn = shapeConn;
          break;
        }
      }

      if (findConn != null)
      {
        this.ConnectedToShapes.Remove(findConn);
      }
    }

    public Shape ThisShape
    { get; set; }

  }
}
