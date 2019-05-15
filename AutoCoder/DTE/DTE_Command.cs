using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_Command
  {

    public class ConstructProperties
    {
      int _BitmapIx = 59;
      object[] _ContextGuids = new object[] { };
      bool _IsMsoButton = true;
      int _CommandStatusSupported = (int)vsCommandStatus.vsCommandStatusSupported ;
      int _CommandStatusEnabled = (int)vsCommandStatus.vsCommandStatusEnabled ;
      int _CommandStyle = (int) vsCommandStyle.vsCommandStylePictAndText;
      EnvDTE80.vsCommandControlType _ControlType = vsCommandControlType.vsCommandControlTypeButton;
      int _Useablex = (int)vsCommandDisabledFlags.vsCommandDisabledFlagsEnabled;
      int _Useable = 16;

      public string Name { get; set; }
      public string ButtonText { get; set; }
      
      public int CommandStatusEnabled
      {
        get { return _CommandStatusEnabled; }
        set { _CommandStatusEnabled = value; }
      }

      public int CommandStatusSupported
      {
        get { return _CommandStatusSupported; }
        set { _CommandStatusSupported = value; }
      }

      public int CommandStyle
      {
        get { return _CommandStyle; }
        set { _CommandStyle = value; }
      }

      public EnvDTE80.vsCommandControlType ControlType
      {
        get { return _ControlType; }
        set { _ControlType = value; }
      }

      public int Useable
      {
        get { return _Useable; }
        set { _Useable = value; }
      }

      public string TooltipText { get; set; }
      public bool IsMsoButton
      {
        get { return _IsMsoButton; }
        set { _IsMsoButton = value; }
      }

      /// <summary>
      /// index into collection of Visual studio button bitmaps
      /// </summary>
      public int BitmapIx
      {
        get { return _BitmapIx; }
        set { _BitmapIx = value; }
      }

      public object[] ContextGuids
      {
        get { return _ContextGuids ; }
        set { _ContextGuids = value ; }
      }

    }

    DTE_Main mMain = null;
    EnvDTE.Command mCmd = null;

    public DTE_Command(DTE_Main InMain, EnvDTE.Command InCmd)
    {
      mMain = InMain;
      mCmd = InCmd;
    }

    public ButtonText ButtonText
    {
      get
      {
        throw new Exception("not supported");
      }
    }

    public string Name
    {
      get { return mCmd.Name; }
    }

    public string Guid
    {
      get { return mCmd.Guid; }
    }

    public int ID
    {
      get { return mCmd.ID; }
    }

    public EnvDTE.Command command
    {
      get { return mCmd; }
    }
  }
}


