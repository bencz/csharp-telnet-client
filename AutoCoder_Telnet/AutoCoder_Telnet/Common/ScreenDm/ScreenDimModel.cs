using AutoCoder.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common.ScreenDm
{
  public class ScreenDimModel : ModelBase, IScreenDim
  {

    public ScreenDimModel()
    {
      this.Height = 0;
      this.Width = 0;
    }
    public ScreenDimModel( IScreenDim Dim)
    {
      this.Height = Dim.Height;
      this.Width = Dim.Width;
    }

    public int  Height
    {
      get { return _Height; }
      set
      {
        if (_Height != value)
        {
          _Height = value;
          RaisePropertyChanged("Height");
        }
      }
    }
    private int _Height;

    public int Width
    {
      get { return _Width; }
      set
      {
        if (_Width != value)
        {
          _Width = value;
          RaisePropertyChanged("Width");
        }
      }
    }
    private int _Width;
  }
}
