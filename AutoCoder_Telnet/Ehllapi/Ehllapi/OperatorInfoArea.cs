using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Ehllapi
{

  public class OperatorInfoArea
  {
    FormatCode mFormat = FormatCode.None;

    bool mSystemAvailable = false;
    bool mSubsystemReady = false;
    bool mKeyboardShift = false;
    bool mCapsLock = false;
    bool mInsertMode = false;
    bool mOperatorInputError = false;
    bool mSystemWait = false;
    bool mCommunicationsError = false;
    bool mMessageWait = false;

    public OperatorInfoArea()
    {
    }

    public OperatorInfoArea( byte[] InOiaBytes)
    {
      SetIndicatorFlags(InOiaBytes);
    }

    public bool CapsLock
    {
      get { return mCapsLock; }
      set { mCapsLock = value; }
    }

    public bool CommunicationsError
    {
      get { return mCommunicationsError; }
      set { mCommunicationsError = value; }
    }

    public bool InsertMode
    {
      get { return mInsertMode; }
      set { mInsertMode = value; }
    }

    public bool KeyboardShift
    {
      get { return mKeyboardShift; }
      set { mKeyboardShift = value; }
    }

    public bool MessageWait
    {
      get { return mMessageWait; }
      set { mMessageWait = value; }
    }

    public bool OperatorInputError
    {
      get { return mOperatorInputError; }
      set { mOperatorInputError = value; }
    }

    public bool SubsystemReady
    {
      get { return mSubsystemReady; }
      set { mSubsystemReady = value; }
    }

    public bool SystemAvailable
    {
      get { return mSystemAvailable; }
      set { mSystemAvailable = value; }
    }

    public bool SystemWait
    {
      get { return mSystemWait; }
      set { mSystemWait = value; }
    }

    public void SetIndicatorFlags(byte[] InOiaBytes)
    {
      // system available. group 1, bit 3.
      if ((InOiaBytes[81] & 0x10) > 0)
        mSystemAvailable = true;

      // subsystem ready. group 1, bit 5.
      if ((InOiaBytes[81] & 0x04) > 0)
        mSubsystemReady = true;

      // keyboard shift. group 3, bit 1.
      if ((InOiaBytes[83] & 0x40) > 0)
        mKeyboardShift = true;

      // caps lock. group 3, bit 2.
      if ((InOiaBytes[83] & 0x20) > 0)
        mCapsLock = true;

      // insert mode. group 7, bit 0.
      if ((InOiaBytes[87] & 0x80) > 0)
        mInsertMode = true;

      // operator input error. group 8, byte 3, bit 5.
      if ((InOiaBytes[90] & 0x04) > 0)
        mOperatorInputError = true;

      // system wait. group 8, byte 4, bit 2.
      if ((InOiaBytes[91] & 0x20) > 0)
        mSystemWait = true;

      // communications error. group 12, bit 0.
      if ((InOiaBytes[96] & 0x80) > 0)
        mCommunicationsError = true;

      // message wait. group 12, bit 7.
      if ((InOiaBytes[96] & 0x01) > 0)
        mMessageWait = true;
    }

    public string ToShowString()
    {
      string s1 =
        "System available: " + SystemAvailable.ToString() +
        "  " + Environment.NewLine +
        "Subsystem ready: " + SubsystemReady.ToString() +
        "  " + Environment.NewLine +
        "Keyboard shift: " + KeyboardShift.ToString() +
        "  " + Environment.NewLine +
        "Caps lock: " + CapsLock.ToString() +
        "  " + Environment.NewLine +
        "Insert mode: " + InsertMode.ToString() +
        "  " + Environment.NewLine +
        "Operator input error: " + OperatorInputError.ToString() +
        "  " + Environment.NewLine +
        "System wait: " + SystemWait.ToString() +
        "  " + Environment.NewLine +
        "Communications error: " + CommunicationsError.ToString() +
        "  " + Environment.NewLine +
        "Message wait: " + MessageWait.ToString();
      return s1;
    }


  }
}
