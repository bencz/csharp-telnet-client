using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using AutoCoder.Sql;

namespace AutoCoder.Sql
{
  public class RunSqlDialog : Form 
  {
    FlowLayoutPanel mFlow;
    string mSqlStmt;
    string mConnString = null;
    TextBox tbSqlStmt = null;
    Label lbl1 = null;

    public RunSqlDialog()
    {
      BuildForm();
    }

    public string ConnectionString
    {
      get { return mConnString; }
      set { mConnString = value; }
    }

    public string SqlStmt
    {
      get { return mSqlStmt; }
      set { mSqlStmt = value; }
    }

    void BuildForm()
    {
      this.Text = "Run Sql Statement";
      this.Width = Font.Height * 20;
      this.Activated += new EventHandler(RunSqlDialog_Activated);

      mFlow = new FlowLayoutPanel();
      mFlow.Parent = this;
      mFlow.Dock = DockStyle.Fill;

      MenuStrip menu = new MenuStrip();
      menu.Parent = this;
      menu.Items.Add("Run", null, menu_Run_Click);
      menu.Items.Add("Exit", null, menu_Exit_Click);

      lbl1 = new Label();
      lbl1.Parent = mFlow;
      lbl1.AutoSize = true;
      lbl1.Text = "Sql statement:";

      tbSqlStmt = new TextBox();
      tbSqlStmt.Parent = mFlow;
      tbSqlStmt.Multiline = true;
      tbSqlStmt.ScrollBars = ScrollBars.Both;
      tbSqlStmt.Width = mFlow.ClientSize.Width;
      tbSqlStmt.Height = Font.Height * 5;
      tbSqlStmt.Text = SqlStmt;
    }

    void RunSqlDialog_Activated(object sender, EventArgs e)
    {
      if (ConnectionString == null)
        throw new ApplicationException("sql connection string is missing");
      tbSqlStmt.Text = SqlStmt;
    }

    void menu_Run_Click(object InObj, EventArgs InArgs)
    {
      SqlCommand cmd = null;

      using (SqlConnection conn = new SqlConnection(ConnectionString ))
      {
        conn.Open();

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = tbSqlStmt.Text;
        int rowCx = cmd.ExecuteNonQuery();
      }
      this.DialogResult = DialogResult.OK;
    }

    void menu_Exit_Click(object InObj, EventArgs InArgs)
    {
      this.DialogResult = DialogResult.OK;
    }

  }
}
