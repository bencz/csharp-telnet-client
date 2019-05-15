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
  [Designer(typeof(SimpleContainerControlDesigner))]
  [ParseChildren(false)]
  public class SimpleContainerControl : WebControl, INamingContainer
  {
  }
}
