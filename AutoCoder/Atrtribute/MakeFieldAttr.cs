using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Atrtribute
{

  // stores the property level attributes used when making the code for a 
  // data class. For example, a class which stores all the values prompted
  // on a property sheet will have a get/set property for each value. Each
  // one of those properties has a data type and has prompt text displayed
  // next to it in the property sheet window. 


  [AttributeUsage(AttributeTargets.All)]
  public class MakeFieldAttr : System.Attribute 
  {
    string mPromptText;
    int mKeyNbr;
    bool mDataMember;

    public bool DataMember
    {
      get { return mDataMember; }
      set { mDataMember = value; }
    }

    public int KeyNbr
    {
      get { return mKeyNbr; }
      set { mKeyNbr = value; }
    }

    public string PromptText
    {
      get { return mPromptText; }
      set { mPromptText = value; }
    }
  }
}
