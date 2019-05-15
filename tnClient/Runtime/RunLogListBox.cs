using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoCoder.Ext.System;

namespace tnClient.Runtime
{
  /// <summary>
  /// Encapsulates a ListBox which displays RunLog type messages.
  /// </summary>
  public class RunLogListBox
  {
    delegate void delWriteLine(string InText);
    delegate void delWriteLines(string[] InLines);

    ListBox mListBox = null;
    string mActivity = null;
    bool mShowTime = false;

    public RunLogListBox(ListBox InListBox, string InActivity)
    {
      mListBox = InListBox;
      mActivity = InActivity;
    }

    public RunLogListBox(RunLogListBox InListBox, string InActivity)
    {
      mListBox = InListBox.mListBox;
      mShowTime = InListBox.mShowTime;
      mActivity = InActivity;
    }

    public string Activity
    {
      get { return mActivity; }
      set { mActivity = value; }
    }

    /// <summary>
    /// show time in the runlog message.
    /// </summary>
    public bool ShowTime
    {
      get { return mShowTime; }
      set { mShowTime = value; }
    }

    // -------------------------- Write ----------------------------------
    public void Write(string InText)
    {
      if (mListBox.Dispatcher.CheckAccess() == false)
      {
        mListBox.Dispatcher.BeginInvoke(
          new delWriteLine(Write),
          new object[] { InText });
      }

      else
      {
        string s1 = mActivity + " " + InText;
        if (mShowTime == true)
        {
          string s2 = DateTime.Now.ToString("HH:mm:ss.fff");
          s1 = s2 + " " + s1;
        }
        mListBox.Items.Add(s1);
      }
    }

    // -------------------------- Write ----------------------------------
    public void Write(string[] InLines)
    {
      if (mListBox.Dispatcher.CheckAccess() == false)
      {
        mListBox.Dispatcher.BeginInvoke(
          new delWriteLines(Write),
          new object[] { InLines });
      }

      else
      {
        foreach (string line in InLines)
        {
          if (line.IsNotBlank())
            Write(line);
        }
      }
    }

  }

}
