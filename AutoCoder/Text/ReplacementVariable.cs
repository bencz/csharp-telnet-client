using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Text
{

  // ------------------------------ ReplacementVariable --------------------------
  public class ReplacementVariable
  {
    string mName;
    string mValue;

    public ReplacementVariable()
    {
      mName = null;
      mValue = null;
    }
    public ReplacementVariable(string InName, string InValue)
    {
      mName = InName;
      mValue = InValue;
    }

    public string Name
    {
      get { return mName; }
      set { mName = value; }
    }
    public string Value
    {
      get { return mValue; }
      set { mValue = value; }
    }
  }

  // --------------------------- ReplacementVariables ----------------------------
  public class ReplacementVariables
  {
    List<ReplacementVariable> mVars;

    public ReplacementVariables()
    {
      mVars = new List<ReplacementVariable>();
    }

    public ReplacementVariables Add(string InName, string InValue)
    {
      ReplacementVariable var = new ReplacementVariable(InName, InValue);
      mVars.Add(var);
      return this;
    }

    public ReplacementVariable Find(string InFindName)
    {
      ReplacementVariable foundVar = null;
      foreach (ReplacementVariable var in mVars)
      {
        if (var.Name == InFindName)
        {
          foundVar = var;
          break;
        }
      }
      return foundVar;
    }
  } // end class ReplacementVariables



}
