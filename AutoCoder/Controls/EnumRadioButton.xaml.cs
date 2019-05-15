using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoCoder.Ext.System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace AutoCoder.Controls
{
  /// <summary>
  /// Interaction logic for EnumRadioButton.xaml
  /// </summary>
  [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
  public partial class EnumRadioButton : UserControl
  {
    /// <summary>
    /// type Type of the enum value. Use this property to list all the values of
    /// the enum. A radio button is created for each enum value.
    /// </summary>
    public Type CurrentEnumType
    {
      get { return _CurrentEnumType; }
      set
      {
        if ( value != _CurrentEnumType)
        {
          _CurrentEnumType = value;
          this.AddRadioButtons(this.CurrentEnumType);
        }
      }
    }
    private Type _CurrentEnumType;

    public object EnumValue
    {
      get { return (object)GetValue(EnumValueProperty); }
      set { SetValue(EnumValueProperty, value); }
    }

    private object CurrentEnumValue
    { get; set; }

    // Using a DependencyProperty as the backing store for EnumValue. 
    public static readonly DependencyProperty EnumValueProperty =
        DependencyProperty.Register("EnumValue", typeof(object), 
          typeof(EnumRadioButton), 
          new FrameworkPropertyMetadata(true, 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
            OnEnumValuePropertyChanged));

    private static void OnEnumValuePropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = sender as EnumRadioButton;
      if ( e.NewValue != null )
      {
        Enum newValue = e.NewValue as Enum;

        thisControl.CurrentEnumType = newValue.GetType();
        thisControl.ApplyValueToRadioButton(newValue);

        // signal value changed event ( if actually changed. )
        thisControl.OnEnumValueChanged(e.NewValue);
      }
    }

    public event Action<Enum,Enum,EnumRadioButton> EnumValueChanged;

    public EnumRadioButton()
    {
      InitializeComponent();
      this.Loaded += EnumRadioButton_Loaded;
    }

    private void EnumRadioButton_Loaded(object sender, RoutedEventArgs e)
    {
    }

    void OnEnumValueChanged(object NewValue)
    {
      bool actuallyChanged = false;

      // check that the enum value has actually been changed.
      if ((this.CurrentEnumValue == null) && (NewValue == null))
      {
        actuallyChanged = false;
      }
      else if ((this.CurrentEnumValue == null) || (NewValue == null))
        actuallyChanged = true;
      else
      {
        int curInt = (int)this.CurrentEnumValue;
        int newInt = (int)NewValue;
        if (curInt != newInt)
          actuallyChanged = true;
      }

      if (actuallyChanged == true)
      {
        var wasEnumValue = this.CurrentEnumValue;

        this.CurrentEnumValue = NewValue;

        if (EnumValueChanged != null)
        {
          this.EnumValueChanged(wasEnumValue as Enum, this.EnumValue as Enum, this);
        }
      }
    }

    /// <summary>
    /// the Type of the enum has been assigned. Remove all the radio buttons of the
    /// control. Then add a new set of radio buttons. One for each value of the enum.
    /// </summary>
    /// <param name="enumType"></param>
    void AddRadioButtons( Type enumType)
    {
      // first. remove all radio buttons.
      wrap1.Children.Clear();

      var enumValues = Enum.GetValues(enumType);
      foreach (var ev in enumValues)
      {
        var rb = new RadioButton();
        rb.Margin = new Thickness(3);
        rb.Content = ev.ToString();
        rb.Tag = ev;
        rb.Checked += Rb_Checked;
        wrap1.Children.Add(rb);
      }

      // use .net framework base class to create a new instance of the enum type.
      // When each radio button is checked, the value of the enum ( stored in the
      // Tag of the radiobutton ), is assigned to this.EnumValue. 
      // this.EnumValue = Activator.CreateInstance(enumType);
    }

    /// <summary>
    /// radio button that corresponds with a value of the enum has been checked. Store
    /// that value in the EnumValue property. That makes the enum value available to
    /// the parent of this control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Rb_Checked(object sender, RoutedEventArgs e)
    {
      var rb = sender as RadioButton;

      {
        int enumAsInt = (int)rb.Tag;
        int currentValue = (int)this.EnumValue;
        if (enumAsInt != currentValue )
        {
          this.EnumValue = Enum.ToObject(this.CurrentEnumType, enumAsInt);
        }
        return;
      }

      {
        // the Tag object of the radio button contains the enum value object.
        this.EnumValue = rb.Tag;

        // convert the enum value to an int.
        int enumAsInt = (int)this.EnumValue;

        // store the int value as the value of the EnumValue exposed by the control.
        enumAsInt = (int)rb.Tag;
        this.EnumValue = Enum.ToObject(this.CurrentEnumType, enumAsInt);
      }
    }

    private void ApplyValueToRadioButton( Enum enumValue )
    {
      // the int value of the value being applied.
      int enumInt = enumValue.ToInt();

      // look at each of the radio buttons. Find the button for this enum value. Once
      // found, check the button.
      foreach ( var child in wrap1.Children)
      {
        if ( child is RadioButton )
        {
          var rb = child as RadioButton;
          var rbInt = (int)rb.Tag;
          if (rbInt == enumInt)
          {
            if ( rb.IsChecked == false )
            {
              rb.IsChecked = true;
            }
            break;
          }
        }
      }
    }

    void PrintEnumValues()
    {
        var enumValues = Enum.GetValues(this.CurrentEnumType);
        foreach (var ev in enumValues)
        {
          Debug.Print(ev.ToString());
        }
    }

    private void butOk_Click(object sender, RoutedEventArgs e)
    {
      PrintEnumValues();
    }
  }
}
