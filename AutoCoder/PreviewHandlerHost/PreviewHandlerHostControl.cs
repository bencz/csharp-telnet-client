using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AutoCoder.COMInterop;

namespace AutoCoder.PreviewHandlerHost
{

  [ToolboxItem(true), ToolboxBitmap(typeof(PreviewHandlerHostControl))]
  public partial class PreviewHandlerHostControl : UserControl
  {
    private string mFilePath;
    private object mComInstance = null;

    public PreviewHandlerHostControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Full path to file to be previewed
    /// 
    /// Whenever a new path is set, the preview is generated
    /// </summary>
    public string FilePath
    {
      get { return mFilePath; }
      set
      {
        mFilePath = value;
        if (value != null && !IsDesignTime())
          GeneratePreview();
      }
    }

    private bool IsDesignTime()
    {
      return (this.Site != null && this.Site.DesignMode);
    }

    /// <summary>
    /// 1) Look up the preview handler associated with the file extension
    /// 2) Create an instance of the handler using its CLSID and reflection
    /// 3) Check if it is a file or stream handler
    /// 4) Initialize with File or Stream using Initialize from the appropriate interface
    /// 5) Call SetWindow passing in a handle to this control and the bounds of the control
    /// 6) Call DoPreview
    /// </summary>
    private void GeneratePreview()
    {
      lblMessage.Visible = false;
      if (mComInstance != null)
      {
        ((IPreviewHandler)mComInstance).Unload();
      }

      RECT r;
      r.top = 0;
      r.bottom = this.Height;
      r.left = 0;
      r.right = this.Width;

      RegistrationData data = RegistryAccess.LoadRegistrationInformation();
      PreviewHandlerInfo handler = null;

      foreach (ExtensionInfo ei in data.Extensions)
      {
        if (mFilePath.ToUpper().EndsWith(ei.Extension.ToUpper()))
        {
          handler = ei.Handler;
          if (handler != null)
            break;
        }
      }
      if (handler == null)
      {
        lblMessage.Visible = true;
        lblMessage.Text = "No Preview Available";
        return;
      }

      Type comType = Type.GetTypeFromCLSID(new Guid(handler.ID));
      try
      {
        // Create an instance of the preview handler
        mComInstance = Activator.CreateInstance(comType);

        // Check if it is a stream or file handler
        if (mComInstance is IInitializeWithFile)
        {
          ((IInitializeWithFile)mComInstance).Initialize(mFilePath, 0);
        }
        else if (mComInstance is IInitializeWithStream)
        {
          if ( System.IO.File.Exists(mFilePath))
          {
            StreamWrapper stream = 
              new StreamWrapper(System.IO.File.Open(mFilePath, FileMode.Open));
            ((IInitializeWithStream)mComInstance).Initialize(stream, 0);
          }
          else
          {
            throw new Exception("File not found");
          }
        }
        ((IPreviewHandler)mComInstance).SetWindow(this.Handle, ref r);
        ((IPreviewHandler)mComInstance).DoPreview();
      }
      catch (Exception ex)
      {
        mComInstance = null;
        lblMessage.Visible = true;
        lblMessage.Text = "Preview Generation Failed - " + ex.Message;
      }
    }


    private void Control_Resize(object sender, EventArgs e)
    {
      if (mComInstance != null)
      {
        RECT r;
        r.top = 0;
        r.bottom = this.Height;
        r.left = 0;
        r.right = this.Width;
        ((IPreviewHandler)mComInstance).SetRect(ref r);
      }
    }
  }
}
