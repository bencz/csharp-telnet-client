using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{

  public delegate void
  ValueChangedByControlEventHandler(object o, EventArgs args);

  public class AcComboBox : ComboBox
  {
    string mWasValue = null;
    bool mDoSignalEvent_ValueChangedByControl = true;

    // event that distinguishes between value of control being changed by the
    // user vs value being changed by code. This event fires only when the
    // text of the control is changed by the user.
    public EventHandler ValueChangedByControl = null;

    public AcComboBox()
    {
      SelectedValueChanged += new EventHandler(AcComboBox_SelectedValueChanged);
    }

    public bool IsChanged
    {
      get
      {
        if (mWasValue == null)
          return false;
        else if (mWasValue == this.Value)
          return false;
        else
          return true;
      }
    }

    public string Value
    {
      get
      {
        return this.Text;
      }
      set
      {
        mDoSignalEvent_ValueChangedByControl = false;
        string s1 = this.Text;
        base.Text = value;
        string s2 = this.Text;
        mDoSignalEvent_ValueChangedByControl = true;

        if (mWasValue == null)
          BaseLine();
      }
    }

    public string WasValue
    {
      get { return mWasValue; }
      set { mWasValue = value; }
    }

    public void BaseLine()
    {
      WasValue = Value;
    }

    void AcComboBox_SelectedValueChanged(object sender, EventArgs e)
    {
      if (mDoSignalEvent_ValueChangedByControl == true)
      {
        if (ValueChangedByControl != null)
        {
          ValueChangedByControl(sender, e);
        }
      }
    }

  }
}
