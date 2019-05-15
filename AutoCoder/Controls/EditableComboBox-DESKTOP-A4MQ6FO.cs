using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Controls
{

  // todo:
  // enable to delete items from the combobox dropdown list.
  // drag and drop items within the combobox dropdow list. 
  // sort the items of the dropdown list alphabetically.

  public class EditableComboBox : ComboBox
  {
    /// <summary>
    /// text changed event. ComboBox by itself does not provide this event.
    /// </summary>
    public event Action<EditableComboBox, string, string> TextValueChanged;
    public EditableComboBox()
    {
      // set property to make the combo box editable.
      this.IsEditable = true;

      this.Loaded += EditableComboBox_Loaded;
    }

    public string TextValue
    {
      get { return (string)GetValue(TextValueProperty); }
      set { SetValue(TextValueProperty, value); }
    }

    public static readonly DependencyProperty TextValueProperty =
    DependencyProperty.Register(
      "TextValue", typeof(string),
      typeof(EditableComboBox),
      new FrameworkPropertyMetadata("",
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
      OnTextValuePropertyChanged));

    private static void OnTextValuePropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = sender as EditableComboBox;
      var tv = e.NewValue as string;
      var wasTextValue = e.OldValue as string;

      // trim the text value.
      if (tv != null)
        tv = tv.TrimEndWhitespace();

      // apply the trimmed value back to TextValue.
      if (tv != thisControl.TextValue)
        thisControl.TextValue = tv;

      // apply the TextValue to the Text property. The two properties should
      // always be in sync.
      if (thisControl.Text != tv)
        thisControl.Text = tv;

      // make sure the items source of the combobox contains the 
      // textValue.
      thisControl.ApplyTextValueToItemsSource();

      // signal the textValueChanged event.
      if (thisControl.TextValueChanged != null)
      {
        thisControl.TextValueChanged(thisControl, wasTextValue, thisControl.TextValue);
      }
    }

    public CharacterCasing CharacterCasing
    {
      get { return (CharacterCasing)GetValue(CharacterCasingProperty); }
      set { SetValue(CharacterCasingProperty, value); }
    }

    public static readonly DependencyProperty CharacterCasingProperty =
    DependencyProperty.Register("CharacterCasing", typeof(CharacterCasing),
    typeof(EditableComboBox), new PropertyMetadata(CharacterCasing.Normal, OnCharacterCasePropertyChanged));

    private static void OnCharacterCasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = d as EditableComboBox;
      thisControl.ApplyCharacterCasing(e.NewValue as CharacterCasing?);
    }

    /// <summary>
    /// property used by user of control. When true, values entered in the
    /// textbox portion of the combobox are applied to the collection that is 
    /// bound to the ItemsSource property.
    /// </summary>
    public bool ApplyEntryToItemsSource
    {
      get { return (bool)GetValue(ApplyEntryToItemsSourceProperty); }
      set { SetValue(ApplyEntryToItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ApplyEntryToItemsSourceProperty =
    DependencyProperty.Register("ApplyEntryToItemsSource", typeof(bool),
    typeof(EditableComboBox), new PropertyMetadata(true));

    private void EditableComboBox_Loaded(
      object sender, System.Windows.RoutedEventArgs e)
    {
      // hook selection changed. When selection changed apply selected value to
      // the TextValue property. ( which in turns syncs with Text property. )
      this.SelectionChanged += EditableComboBox_SelectionChanged;

      this.ApplyCharacterCasing(this.CharacterCasing);

      // hook LostFocus on the textbox within the combobox. Need to keep TextValue and
      // the Text property in sync at all times.
      this.AddHandler(
        System.Windows.Controls.Primitives.TextBoxBase.LostFocusEvent,
        new RoutedEventHandler(EditableComboBox_TextLostFocus));
    }

    /// <summary>
    /// apply CharacterCasing property of the EditableComboBox to the actual 
    /// textbox within the template of the ComboBox.
    /// </summary>
    /// <param name="casing"></param>
    void ApplyCharacterCasing(CharacterCasing? casing)
    {
      if ((casing != null) && (this.Template != null))
      {
        TextBox editTextBox = this.Template.FindName("PART_EditableTextBox", this) as TextBox;
        if (editTextBox != null)
        {
          editTextBox.CharacterCasing = casing.Value;
        }
      }
    }

    void ApplyTextValueToItemsSource()
    {
      if (this.ApplyEntryToItemsSource == true)
      {
        if (this.TextValue.IsNullOrEmpty() == false)
        {
          var collection = this.ItemsSource as ObservableCollection<string>;
          if (collection != null)
          {
            if (collection.FirstOrDefault(c => c == this.TextValue) == null)
              collection.Add(this.TextValue);
          }
        }
      }
    }

    /// <summary>
    /// the textbox component of the combobox has lost focus. Apply the Text 
    /// property value to the TextValue property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditableComboBox_TextLostFocus(object sender, RoutedEventArgs e)
    {
      var s1 = this.Text.TrimEndWhitespace();
      this.TextValue = s1;
    }

    /// <summary>
    /// new value selected in combobox. Apply selected value to the TextValue 
    /// property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var s1 = this.SelectedItem as string;
      if (s1 != this.TextValue)
      {
        var wasValue = this.TextValue;
        this.TextValue = s1;
      }
    }
  }
}

