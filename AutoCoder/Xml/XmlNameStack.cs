using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AutoCoder.Xml
{
  /// <summary>
  /// class used in conjunction with XmlReader. Maintains a NameStack that is
  /// used to determine the parent node of the current reader node.
  /// </summary>
  public class XmlNameStack
  {
    string[] mNameStack ;
    int mDepth;

    public XmlNameStack( )
    {
      mNameStack = new string[99] ;
    }

    /// <summary>
    /// Depth of the name stack. ( Can also use the Depth in the associated XmlReader. )
    /// </summary>
    public int Depth
    {
      get { return mDepth; }
    }

    /// <summary>
    /// indexer returns name stack entries prior to the current name stack.
    /// </summary>
    /// <param name="InRelativeUpStackDepth"></param>
    /// <returns></returns>
    public string this[int InRelativeUpStackDepth]
    {
      get
      {
        int Ix = mDepth - InRelativeUpStackDepth;
        if (Ix < 0)
          return null;
        else
          return mNameStack[Ix];
      }
    }

    public string[] NameStack
    {
      get { return mNameStack ; }
      set { mNameStack = value ; }
    }

    /// <summary>
    /// Run this method immed after reader.Read. Updates namestack with affects of
    /// the current xmlreader node.
    /// </summary>
    /// <param name="InReader"></param>
    public void UpdateCurrent(XmlReader InReader)
    {
      mDepth = InReader.Depth;
      if (InReader.NodeType == XmlNodeType.Element)
      {
        mNameStack[mDepth] = InReader.Name;
      }
    }

  }
}
