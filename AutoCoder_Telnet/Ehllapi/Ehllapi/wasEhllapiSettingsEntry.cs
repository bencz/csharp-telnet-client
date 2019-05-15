
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Serialization;
using AutoCoder.Text;
using AutoCoder.Core;

namespace AutoCoder.Ehllapi
{

#if skip

  public class EhllapiSettingsEntry : Form
  {
    TextBox tbSystemName;
    TextBox tbUserName;
    TextBox tbPassword;
    TextBox tbSessId;
    TextBox tbPath_WrkstnProfile;
    Button butPath_WrkstnProfile;
    TextBox tbPath_pcsws;
    Button butPath_pcsws;
    TextBox tbPath_cwblogon;
    Button butPath_cwblogon;

    string mFormText = "Display Station Settings Entry";
    Button butOk;
    Button butCancel;
    EhllapiSettings mBeforeRow = null ;
    EhllapiSettings mAfterRow = null ;
    ActionCode mAction;
    bool wasActivated = false;

    public EhllapiSettingsEntry()
    {
      BuildForm();
    }

    public AutoCoder.Core.ActionCode ActionCode
    {
      get { return mAction; }
      set { mAction = value; }
    }

    public EhllapiSettings AfterActionRow
    {
      get { return mAfterRow; }
      set { mAfterRow = value; }
    }

    public EhllapiSettings BeforeActionRow
    {
      get { return mBeforeRow; }
      set { mBeforeRow = value; }
    }

    void BuildForm()
    {
      Label lbl = null;
      this.Text = mFormText;
      this.Activated += new EventHandler(SettingsEntryForm_Activated);
      this.Height = Font.Height * 25;
      this.Width = Font.Height * 45;

      FlowLayoutPanel flow = new FlowLayoutPanel();
      flow.Parent = this;
      flow.Dock = DockStyle.Fill;

      MenuStrip menu = new MenuStrip();
      menu.Parent = this;
      menu.Items.Add("Exit", null, menu_Exit_Click);

      // system name
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "System name";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbSystemName = new TextBox();
      tbSystemName.Parent = flow;
      tbSystemName.AutoSize = true;
      tbSystemName.Enter += new EventHandler(tbSystemName_Enter);
      flow.SetFlowBreak(tbSystemName, true);

      // UserName
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "User name";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbUserName = new TextBox();
      tbUserName.Parent = flow;
      tbUserName.AutoSize = true;
      flow.SetFlowBreak(tbUserName, true);

      // password
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "Password";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbPassword = new TextBox();
      tbPassword.Parent = flow;
      tbPassword.AutoSize = true;
      tbPassword.PasswordChar = '*';
      flow.SetFlowBreak(tbPassword, true);

      // sessId
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "Session ID";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbSessId = new TextBox();
      tbSessId.Parent = flow;
      tbSessId.AutoSize = true;
      flow.SetFlowBreak(tbSessId, true);

      // workstation profile file path
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "Wrkstn profile file path";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbPath_WrkstnProfile = new TextBox();
      tbPath_WrkstnProfile.Parent = flow;
      tbPath_WrkstnProfile.Width = Font.Height * 30;

      butPath_WrkstnProfile = new Button();
      butPath_WrkstnProfile.Parent = flow;
      butPath_WrkstnProfile.Width = Font.Height * 3;
      butPath_WrkstnProfile.Text = "...";
      butPath_WrkstnProfile.Click += new EventHandler(butPath_WrkstnProfile_Click);
      flow.SetFlowBreak(butPath_WrkstnProfile, true);

      // pcsws file path
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "PCSWS file path";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbPath_pcsws = new TextBox();
      tbPath_pcsws.Parent = flow;
      tbPath_pcsws.Width = Font.Height * 25;

      butPath_pcsws = new Button();
      butPath_pcsws.Parent = flow;
      butPath_pcsws.Width = Font.Height * 3;
      butPath_pcsws.Text = "...";
      butPath_pcsws.Click += new EventHandler(butPath_pcsws_Click);
      flow.SetFlowBreak(butPath_pcsws, true);

      // cwblogon file path
      lbl = new Label();
      lbl.Parent = flow;
      lbl.Text = "CWBLOGON file path";
      lbl.AutoSize = true;
      lbl.Anchor = AnchorStyles.Left;

      tbPath_cwblogon = new TextBox();
      tbPath_cwblogon.Parent = flow;
      tbPath_cwblogon.Width = Font.Height * 25;

      butPath_cwblogon = new Button();
      butPath_cwblogon.Parent = flow;
      butPath_cwblogon.Width = Font.Height * 3;
      butPath_cwblogon.Text = "...";
      butPath_cwblogon.Click += new EventHandler(butPath_cwblogon_Click);
      flow.SetFlowBreak(butPath_cwblogon, true);

      // ok button
      butOk = new Button();
      butOk.Parent = flow;
      butOk.Text = "OK";
      butOk.Click += butOk_Click;

      // cancel button
      butCancel = new Button();
      butCancel.Parent = flow;
      butCancel.Text = "Cancel";
      butCancel.Click += butCancel_Click;
      flow.SetFlowBreak(butCancel, true);
    }

    void butPath_cwblogon_Click(object sender, EventArgs e)
    {
      const string strFilter = "cwblogon.exe file|" +
                               "cwblogon.exe";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;
      DialogResult rv = dlg.ShowDialog();
      if (rv == DialogResult.OK)
        tbPath_pcsws.Text = dlg.FileName;
    }

    void butPath_WrkstnProfile_Click(object sender, EventArgs e)
    {
      const string strFilter = "Wrkstn profile files (*.ws)|" +
                               "*.WS";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;
      DialogResult rv = dlg.ShowDialog( ) ;
      if ( rv == DialogResult.OK )
        tbPath_WrkstnProfile.Text = dlg.FileName ;
    }

    void butPath_pcsws_Click(object sender, EventArgs e)
    {
      const string strFilter = "pcsws.exe file|" +
                               "pcsws.exe";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;
      DialogResult rv = dlg.ShowDialog();
      if (rv == DialogResult.OK)
        tbPath_pcsws.Text = dlg.FileName;
    }

    void tbSystemName_Enter(object sender, EventArgs e)
    {
    }

    void BuildForm_ApplyInitialValues(EhllapiSettings InRow)
    {
      tbSystemName.Text = InRow.SystemName;
      tbUserName.Text = InRow.UserName;
      tbPassword.Text = InRow.Password;
      tbSessId.Text = InRow.SessId;
      tbPath_WrkstnProfile.Text = InRow.Path_WrkstnProfile;
      tbPath_pcsws.Text = InRow.Path_pcsws;
      tbPath_cwblogon.Text = InRow.Path_cwblogon;
    }

    void SettingsEntryForm_Activated(object sender, EventArgs e)
    {
      if (wasActivated == false)
      {
        wasActivated = true;
        this.Text = mFormText;
        butOk.Text = "OK";
        if (BeforeActionRow != null)
          BuildForm_ApplyInitialValues(BeforeActionRow);
      }
    }

    void butOk_Click(object InSource, EventArgs InArgs)
    {
      EhllapiSettings row = new EhllapiSettings();
      row.SystemName = tbSystemName.Text;
      row.UserName = tbUserName.Text;
      row.Password = tbPassword.Text;
      row.SessId = tbSessId.Text;
      row.Path_WrkstnProfile = tbPath_WrkstnProfile.Text;
      row.Path_pcsws = tbPath_pcsws.Text;
      row.Path_cwblogon = tbPath_cwblogon.Text;

      // after action row.
      AfterActionRow = row;

      // exit the dialog.
        this.DialogResult = DialogResult.OK;
    }

    void butCancel_Click(object InSource, EventArgs InArgs)
    {
      this.Close();
    }

    void menu_Exit_Click(object InObj, EventArgs InArgs)
    {
      this.Close();
    }

  }

#endif

}

