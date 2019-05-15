using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;

namespace AutoCoder.WebControls.RowPrompt
{
  public class SimpleContainerControlDesigner : ContainerControlDesigner
  {
    private Style _style = null;

    // Add the caption by default. Note that the caption 
    // will only appear if the Web server control 
    // allows child controls rather than properties. 
    public override string FrameCaption
    {
      get
      {
        return "A Simple ContainerControlDesigner";
      }
    }

    public override Style FrameStyle
    {
      get
      {
        if (_style == null)
        {
          _style = new Style();
          _style.Font.Name = "Verdana";
          _style.Font.Size = new FontUnit("XSmall");
          _style.BackColor = Color.LightBlue;
          _style.ForeColor = Color.Black;
        }

        return _style;
      }
    }
  }
}