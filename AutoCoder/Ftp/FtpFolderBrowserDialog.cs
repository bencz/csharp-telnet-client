using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AutoCoder;
using AutoCoder.Ftp;
using AutoCoder.File;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Ext.System;

namespace AutoCoder.Ftp
{
  /// <summary>
  /// Dialog form used to browse and select a folder in an FTP site.
  /// To use, new the object, set the user, password and site properties,
  /// then call the ShowDialog method. On return, get from the
  /// SelectedFolderPath property to return the selected folder.
  /// </summary>
  public class FtpFolderBrowserDialog : Form
  {
    FlowLayoutPanel mFlow = new FlowLayoutPanel();
    Label lblFtpSite = new Label();
    TreeView mFtpDirTree = new TreeView();
    string mFtpSiteDir = "";
    string mFtpSiteUrl = "" ;
    string mFtpUserName = "";
    string mFtpPassword = "";
    FullPath mFtpHomePath = new FullPath("");   

    public FtpFolderBrowserDialog()
    {
      BuildForm();
    }

    public string FtpSiteDir
    {
      get { return mFtpSiteDir; }
      set
      {
        mFtpSiteUrl = value;
        FtpDirTree_Refill();
      }
    }

    public string FtpSiteUrl
    {
      get { return mFtpSiteUrl; }
      set
      {
        mFtpSiteUrl = value;
        lblFtpSite.Text = "Ftp Site : " + mFtpSiteUrl;
        FtpDirTree_Refill(); 
      }
    }

    public FullPath FtpHomePath
    {
      get { return mFtpHomePath; }
      set
      { 
        mFtpHomePath = value;
        FtpDirTree_Refill();
      }
    }

    public string FtpPassword
    {
      get { return mFtpPassword; }
      set
      { 
        mFtpPassword = value;
        FtpDirTree_Refill();
      }
    }

    public string FtpUserName
    {
      get { return mFtpUserName; }
      set
      { 
        mFtpUserName = value;
        FtpDirTree_Refill();
      }
    }

    /// <summary>
    /// The selected FTP folder. Get this property after the dialog returns
    /// dialog result OK.
    /// </summary>
    public FullPath SelectedFolderPath
    {
      get
      {
        TreeNode node = mFtpDirTree.SelectedNode;
        if (node == null)
          return null;
        else
          return (FullPath)node.Tag;
      }
    }

    void BuildForm()
    {
      this.Width = Font.Height * 40;

      this.Text = "Browse For Ftp Folder";

      mFlow.Parent = this;
      mFlow.Dock = DockStyle.Fill;
      mFlow.AutoSize = true;
      mFlow.Resize += new EventHandler(flow_Resize);

      lblFtpSite.Parent = mFlow;
      lblFtpSite.AutoSize = true;

      mFlow.SetFlowBreak(lblFtpSite, true);

      mFtpDirTree = new TreeView();
      mFtpDirTree.Parent = mFlow;
      mFtpDirTree.Font = new Font("Lucida Console", 8);
      mFtpDirTree.Size = FtpDirTree_CalcSize();
      mFtpDirTree.LabelEdit = true;
      mFtpDirTree.BeforeExpand += 
        new TreeViewCancelEventHandler(FtpDirTree_BeforeExpand);
      mFtpDirTree.NodeMouseClick += 
        new TreeNodeMouseClickEventHandler(FtpDirTree_NodeMouseClick);
      mFtpDirTree.AfterLabelEdit += 
        new NodeLabelEditEventHandler(FtpDirTree_AfterLabelEdit);
      mFlow.SetFlowBreak(mFtpDirTree, true);

      Button butNewFolder = new Button();
      butNewFolder.Parent = mFlow;
      butNewFolder.Width = this.Font.Height * 8;
      butNewFolder.Text = "Make new folder";
      butNewFolder.Click += new EventHandler(butNewFolder_Click);

      Button butOk = new Button();
      butOk.Parent = mFlow;
      butOk.Width = this.Font.Height * 6;
      butOk.Text = "OK";
      butOk.Click += new EventHandler(butOk_Click);

      Button butCancel = new Button();
      butCancel.Parent = mFlow;
      butCancel.Width = this.Font.Height * 6;
      butCancel.Text = "Cancel";
      butCancel.Click += new EventHandler(butCancel_Click);

    }

    void flow_Resize(object sender, EventArgs e)
    {
      mFtpDirTree.Size = FtpDirTree_CalcSize( ) ;
    }

    /// <summary>
    /// Create a new ftp directory within the directory of the
    /// currently selected tree node.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void butNewFolder_Click(object sender, EventArgs e)
    {
      FullPath dirPath = SelectedFolderPath ;
      using (FtpClient fc = GetFtpClient(dirPath))
      {
        fc.MakeDirectory("NewFolder");

        FtpResponse_DirList dirList = fc.GetDirList("NewFolder*");
        foreach (FtpDirEntry de in dirList.RcvdDirList)
        {
          string s1 = de.EntryName;
          if (s1 == "NewFolder")
          {
            TreeNode node = mFtpDirTree.SelectedNode;
           
            string s2 = Path.Combine(dirPath.ToString(), de.EntryName);
            FullPath filePath = new FullPath(s2);

            TreeNode newNode = node.Nodes.Add(de.EntryName);
            newNode.Tag = filePath;
          }
        }
      }
    }

    void butOk_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
    }

    void butCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    Size FtpDirTree_CalcSize()
    {
      int wx = mFlow.ClientSize.Width;
      int hx = mFlow.ClientSize.Height - 80;
      return new Size(wx, hx);
    }

    void FtpDirTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      TreeNode node = e.Node;
      if (e.CancelEdit == false)
      {
        FullPath dirPath = (FullPath)node.Tag;
        FullPath parentDirPath = new FullPath(dirPath.DirectoryName);
        using (FtpClient fc = GetFtpClient(parentDirPath))
        {
          fc.Rename(dirPath.FileName.ToString( ), e.Label);
        }
      }
    }

    void FtpDirTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      TreeNode node = e.Node;
      Cursor wasCursor = null;

      // the tag of the node contains the full path of the ftp directory.
      FullPath filePath = (FullPath)node.Tag;

      // calc if the node contains a single, empty node.
      if ((node.Nodes.Count == 1)
        && (Stringer.GetFromStringObject(node.Nodes[0].Tag, "") == "empty"))
      {
        node.Nodes.Clear();

        try
        {
          wasCursor = Cursor.Current ;
          Cursor.Current = Cursors.WaitCursor;
          mFtpDirTree.BeginUpdate();
          FtpDirTree_FillDirNode(filePath, node);
        }
        finally
        {
          mFtpDirTree.EndUpdate();
          Cursor.Current = wasCursor;
        }
      }
    }

    // ----------------------- FtpDirTree_FillDirNode ------------------------
    void FtpDirTree_FillDirNode(
      FullPath InDirPath, TreeNode InDirNode)
    {
      using (FtpClient fc = GetFtpClient( InDirPath ))
      {

        FtpResponse_DirList dirList = fc.GetDirList();
        foreach (FtpDirEntry de in dirList.RcvdDirList)
        {
          if (de.EntryType == AcFileType.Folder)
          {
            string s1 = Path.Combine(InDirPath.ToString(), de.EntryName);
            FullPath filePath = new FullPath(s1);

            TreeNode node = InDirNode.Nodes.Add(de.EntryName);
            node.Tag = filePath;

            // add a dummy child node. This cause a plus sign to be displayed
            // next to the folder node.
            TreeNode subNode = node.Nodes.Add("empty");
            subNode.Tag = "empty";
          }
        }
      }
    }

    // ----------------------- FtpDirTree_FillDirNode ------------------------
    void FtpDirTree_FillDirNode(
      FtpClient InFtpClient, string InFolderName, TreeNode InDirNode)
    {
      FtpClient fc = InFtpClient;

      fc.ChangeDirectory(InFolderName);

      FtpResponse_DirList dirList = fc.GetDirList();
      foreach (FtpDirEntry de in dirList.RcvdDirList)
      {
        if (de.EntryType == AcFileType.Folder)
        {
          TreeNode node = InDirNode.Nodes.Add(de.EntryName);
        }
      }

      fc.ChangeDirectory("..");
    }

    /// <summary>
    /// node mouse click handler. 
    /// Right click handler pops up a context menu containing "rename" and
    /// "delete" options.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void FtpDirTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      MenuItem mi = null;

      MouseButtons button = e.Button;
      if (button == MouseButtons.Right)
      {
        TreeNode node = e.Node;

        ContextMenu cm = new ContextMenu();

        mi = new MenuItem("Delete");
        mi.Click += new EventHandler(FtpDirTree_ContextMenu_Click);
        mi.Tag = node;
        cm.MenuItems.Add(mi);

        mi = new MenuItem("Rename");
        mi.Click += new EventHandler(FtpDirTree_ContextMenu_Click);
        mi.Tag = node;
        cm.MenuItems.Add(mi);

        cm.Show(mFtpDirTree, e.Location);
      }
    }

    /// <summary>
    /// popup ContextMenu option click handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void FtpDirTree_ContextMenu_Click(object sender, EventArgs e)
    {
      MenuItem mi = (MenuItem) sender ;
      string s1 = mi.Text;

      // delete the ftp directory 
      if (mi.Text == "Delete")
      {
        TreeNode node = (TreeNode)mi.Tag;
        FullPath dirPath = (FullPath)node.Tag;
        FullPath parentDirPath = new FullPath(dirPath.DirectoryName);
        using (FtpClient fc = GetFtpClient(parentDirPath ))
        {
          FtpResponse resp = fc.RemoveDirectory(dirPath.FileName.ToString( ));
          node.Remove();
        }
      }

        // rename the ftp directory. Begin the node label edit process. Actual 
        // renaming is performed by the FtpDirTree_AfterLabelEdit event handler. 
      else if (s1 == "Rename")
      {
        TreeNode node = (TreeNode)mi.Tag;
        if (node.IsEditing == false)
        {
          node.BeginEdit();
        }
      }
    }

    // ------------------------- FtpDirTree_Refill --------------------------
    void FtpDirTree_Refill()
    {
      if (StringExt.IsBlank(mFtpSiteUrl) == true)
        return;
      if (StringExt.IsBlank(mFtpUserName) == true)
        return;
      if (StringExt.IsBlank(mFtpPassword) == true)
        return;

      mFtpDirTree.Nodes.Clear();

      try
      {
        mFtpDirTree.BeginUpdate();

        // base node 
        TreeNode baseNode = mFtpDirTree.Nodes.Add(mFtpSiteDir);
        FullPath filePath = new FullPath(mFtpSiteDir);
        baseNode.Tag = filePath;

        FtpDirTree_FillDirNode(filePath, baseNode);

        // expand the base, ftp site, directory node.
        baseNode.Expand();
      }
      finally
      {
        mFtpDirTree.EndUpdate();
      }
    }

    FtpClient GetFtpClient(FullPath InDirPath)
    {
      FtpClient fc = new FtpClient();
      fc.UserName = mFtpUserName;
      fc.Password = mFtpPassword;
      fc.Url = mFtpSiteUrl;
      fc.ChangeDirectory(InDirPath.ToString());
      return fc;
    }

  }
}
