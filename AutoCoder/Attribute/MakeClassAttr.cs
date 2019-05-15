using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Attribute
{

#if _skip

  [AttributeUsage(AttributeTargets.All)]
  public class MakeClassAttr : System.Attribute 
  {
    bool mDataContract;
    bool mDefaultConstructor;
    bool mFullConstructor;
    bool mInterface_Enumerable;
    bool mInterface_PropChgd;
    bool mInterface_XmlSerializable;

    public bool DataContract
    {
      get { return mDataContract; }
      set { mDataContract = value; }
    }

    public bool DefaultConstructor
    {
      get { return mDefaultConstructor; }
      set { mDefaultConstructor = value; }
    }

    public bool FullConstructor
    {
      get { return mFullConstructor; }
      set { mFullConstructor = value; }
    }

    public bool Interface_Enumerable
    {
      get { return mInterface_Enumerable; }
      set { mInterface_Enumerable = value; }
    }

    public bool Interface_PropChgd
    {
      get { return mInterface_PropChgd; }
      set { mInterface_PropChgd = value; }
    }

    public bool Interface_XmlSerializable
    {
      get { return mInterface_XmlSerializable; }
      set { mInterface_XmlSerializable = value; }
    }
  }

#endif

}
