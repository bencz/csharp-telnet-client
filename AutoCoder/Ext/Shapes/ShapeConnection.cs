using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Windows.Shapes.Connection;
using System.Windows.Controls;
using AutoCoder.Ext.Controls;

namespace AutoCoder.Ext.Shapes
{

  /// <summary>
  /// the connected to shape and the line that connects to it.
  /// </summary>
  public class ShapeConnection
  {
    public ShapeConnection()
    {
    }

    /// <summary>
    /// the line that connects to the ToShape
    /// </summary>
    public Line ConnectLine
    { get; set; }


    /// <summary>
    /// the connection route that connects to the ToShape.
    /// </summary>
    public ConnectionRoute ConnectRoute
    { get; set; }

    /// <summary>
    /// the connected to shape
    /// </summary>
    public Shape ToShape
    { get; set; }


    public void RemoveConnectionLines( Canvas InCanvas)
    {
      // remove the connection line.
      if (this.ConnectLine != null)
      {
        InCanvas.Children.Remove(this.ConnectLine);
      }

      if (this.ConnectRoute != null)
      {
        InCanvas.RemoveLinesOfRoute(this.ConnectRoute);
      }
    }
  }
}
