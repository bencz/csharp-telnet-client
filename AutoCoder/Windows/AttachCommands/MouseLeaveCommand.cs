using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AutoCoder.Windows.AttachCommands
{
  public class MouseLeaveCommand
  {

    /// <summary>
    /// store the event handler of a dependency object. 
    /// </summary>
    public class DepObjInfo
    {
      public DependencyObject DepObj
      { get; set; }

      public void MouseLeave(object sender, MouseEventArgs e)
      {
        // get the command that handles the MouseLeave event.
        var cmd = DepObj.GetValue(MouseLeaveCommand.CommandProperty) as ICommand;
        if (cmd != null)
        {
          cmd.Execute(DepObj);
        }
      }

      /// <summary>
      /// make sure the DepObj object has a DepObjInfo object. 
      /// Store the DepObjInfo object itself as an associated property of the
      /// DependencyObject.
      /// </summary>
      /// <param name="DepObj"></param>
      /// <returns></returns>
      public static DepObjInfo AssureObjInfo(DependencyObject DepObj)
      {
        // make sure the DepObjInfo object exists. Store in ObjInfo property.
        var objInfo = (DepObjInfo)DepObj.GetValue(ObjInfoProperty);
        if (objInfo == null)
        {
          objInfo = new DepObjInfo()
          {
            DepObj = DepObj
          };
          DepObj.SetValue(ObjInfoProperty, objInfo);
        }
        return objInfo;
      }
    }

    /// <summary>
    /// Command Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty ObjInfoProperty =
        DependencyProperty.RegisterAttached(
        "ObjInfo", typeof(DepObjInfo), typeof(MouseLeaveCommand),
            new FrameworkPropertyMetadata((DepObjInfo)null,
                new PropertyChangedCallback(OnObjInfoChanged)));

    /// <summary>
    /// Gets the Command property.  
    /// </summary>
    public static DepObjInfo GetObjInfo(DependencyObject d)
    {
      return (DepObjInfo)d.GetValue(ObjInfoProperty);
    }

    /// <summary>
    /// Sets the Command property. 
    /// </summary>
    public static void SetObjInfo(DependencyObject d, DepObjInfo value)
    {
      d.SetValue(ObjInfoProperty, value);
    }

    private static void OnObjInfoChanged(
      DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    /// <summary>
    /// Command Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
        "Command", typeof(ICommand), typeof(MouseLeaveCommand),
            new FrameworkPropertyMetadata((ICommand)null,
                new PropertyChangedCallback(OnCommandChanged)));

    /// <summary>
    /// Gets the Command property.  
    /// </summary>
    public static ICommand GetCommand(DependencyObject d)
    {
      return (ICommand)d.GetValue(CommandProperty);
    }

    /// <summary>
    /// Sets the Command property. 
    /// </summary>
    public static void SetCommand(DependencyObject d, ICommand value)
    {
      d.SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Handles changes to the Command property.
    /// </summary>
    private static void OnCommandChanged(
      DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var newCmd = (ICommand)e.NewValue;
      var uiElem = d as UIElement;
      if (uiElem != null)
      {
        // make sure the DepObjInfo object exists. Store in ObjInfo property.
        var objInfo = DepObjInfo.AssureObjInfo(d);

        // hook or unhook the MouseLeave event on this UIElement. 
        // store the event handler in te DepObjInfo object that stored the DepObj itself.
        if (newCmd == null)
          uiElem.MouseLeave -= objInfo.MouseLeave;
        else
          uiElem.MouseLeave += objInfo.MouseLeave;
      }
    }

  }
}
