using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.WebControls.RowPrompt
{

  public delegate void
  RowPromptResultsEvent(object o, RowPromptResults args);

  /// <summary>
  /// results from prompting of the row columns.
  /// </summary>
  public class RowPromptResults
  {
    ActionCode mAction = ActionCode.None;
    AcNamedValues mPrimaryKeys = null;
    AcNamedValues mColumnValues = null;

    public RowPromptResults()
    {
    }

    public ActionCode Action
    {
      get { return mAction; }
      set { mAction = value; }
    }

    public AcNamedValues ColumnValues
    {
      get { return mColumnValues; }
      set { mColumnValues = value; }
    }

    /// <summary>
    /// the keys to the row that was acted on.
    /// If the action is Add, this property is null.
    /// If an action such as Change, this property contains the keys
    /// of the row changed in AcNamedValues dictionary form.
    /// </summary>
    public AcNamedValues PrimaryKeys
    {
      get { return mPrimaryKeys; }
      set { mPrimaryKeys = value; }
    }
  }

}
