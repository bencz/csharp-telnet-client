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

  [Designer(typeof(SimpleCompositeControlDesigner))]
  public class SimpleCompositeControl : CompositeControl
  {

    private String _prompt = "Please enter your date of birth: ";
    public virtual String Prompt
    {
      get
      {
        object o = ViewState["Prompt"];
        return (o == null) ? _prompt : (string)o;
      }
      set
      {
        ViewState["Prompt"] = value;
      }
    }

    public virtual DateTime DOB
    {
      get
      {
        object o = ViewState["DOB"];
        return (o == null) ? DateTime.Now : (DateTime)o;
      }
      set
      {
        ViewState["DOB"] = value;
      }
    }

    protected override void CreateChildControls()
    {
      Label lab = new Label();

      lab.Text = Prompt;
      lab.ForeColor = System.Drawing.Color.Red;
      this.Controls.Add(lab);

      Literal lit = new Literal();
      lit.Text = "<br />";
      this.Controls.Add(lit);

      TextBox tb = new TextBox();
      tb.ID = "tb1";
      tb.Text = DOB.ToString();
      this.Controls.Add(tb);

      base.CreateChildControls();
    }

  }
}
