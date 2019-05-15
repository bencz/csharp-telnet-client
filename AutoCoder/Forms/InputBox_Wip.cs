using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AutoCoder.Forms
{
  public class InputBox_Wip : Form
  {
    private global::System.Windows.Forms.TextBox tbInput;
    Label lblPrompt;
    private global::System.ComponentModel.Container components = null;
    string mInputedText = null;
    string mPromptText = null;

    public InputBox_Wip()
    {
      InitializeComponent();
    }

    public InputBox_Wip(string InPromptText)
    {
      PromptText = InPromptText;
      InitializeComponent();
    }

    public string InputedText
    {
      get
      {
        if (mInputedText == null)
          return "";
        else
          return mInputedText;
      }
      set { mInputedText = value; }
    }

    public string PromptText
    {
      set { mPromptText = value; }
      get
      {
        if (mPromptText == null)
          return "";
        else
          return mPromptText;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();

      FlowLayoutPanel flow = new FlowLayoutPanel();
      flow.Parent = this;
      flow.Dock = DockStyle.Fill;

      // prompt label
      lblPrompt = new Label();
      lblPrompt.Parent = flow;
      lblPrompt.AutoSize = true;
      lblPrompt.Text = PromptText;
      flow.SetFlowBreak(lblPrompt, true);

      // tbInput 
      tbInput = new TextBox();
      tbInput.Parent = flow;
      tbInput.Width = Font.Height * 10;
      tbInput.KeyDown +=
        new System.Windows.Forms.KeyEventHandler(tbInput_KeyDown);

      // InputBox 
      this.Width = Font.Height * 14;
      this.ControlBox = false;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.ResumeLayout(false);
      this.Activated += new EventHandler(InputBox_Activated);
    }

    void InputBox_Activated(object sender, EventArgs e)
    {
      lblPrompt.Text = PromptText;
      tbInput.Text = InputedText;
    }

    private void tbInput_KeyDown(
      object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        mInputedText = tbInput.Text;
        this.Close();
      }
    }
  }
}
