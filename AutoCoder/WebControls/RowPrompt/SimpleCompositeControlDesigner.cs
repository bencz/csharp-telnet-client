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
  public class SimpleCompositeControlDesigner : CompositeControlDesigner
  {
    // Set this property to prevent the designer from being resized.
    public override bool AllowResize
    {
      get { return false; }
    }
  }
}