using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext.System;
using System.Collections;

namespace AutoCoder.Xml
{
  public class XmlElem : IEnumerable<XmlElem>
  {
    XmlElem mParentElem;
    ValueForm mValueForm = ValueForm.None;
    LinkedList<XmlElem> mChildElems = null;

    // the node of this elem in the LinkedList it is a member of.
    LinkedListNode<XmlElem> mNode = null;

    string mElemName = null;
    string mElemValue = null;
    int mDepth = 0;

    public XmlElem()
    {
    }

    #region properties
    public LinkedList<XmlElem> ChildElems
    {
      get { return mChildElems; }
    }

    public int Depth
    {
      get { return mDepth; }
      set { mDepth = value; }
    }

    public string ElemName
    {
      get { return mElemName; }
      set { mElemName = value; }
    }

    public string ElemValue
    {
      get { return mElemValue; }
      set { mElemValue = value; }
    }

    public bool HasChildElems
    {
      get
      {
        if (mChildElems == null)
          return false;
        else if (mChildElems.Count == 0)
          return false;
        else
          return true;
      }
    }

    /// <summary>
    /// the node which holds this XmlElem of the LinkedList
    /// of the Parent XmlElem. ( use this node to step to the next
    /// and previous peer XmlElem of this XmlElem )
    /// </summary>
    public LinkedListNode<XmlElem> Node
    {
      get { return mNode; }
    }

    public XmlElem Parent
    {
      get { return mParentElem; }
    }

    public ValueForm ValueForm
    {
      get { return mValueForm; }
      set { mValueForm = value; }
    }
    #endregion

    #region indexers
    /// <summary>
    /// Search child elements for the named element.
    /// </summary>
    /// <param name="InKey"></param>
    /// <returns></returns>
    public XmlElem this[string InKey]
    {
      get
      {
        XmlElem elem = FindChildElem(InKey);
        return elem;
      }
    }

    public XmlElem this[string InKey1, string InKey2]
    {
      get
      {
        XmlElem elem = FindChildElem(new string[] { InKey1, InKey2 });
        return elem;
      }
    }
    #endregion

    #region methods
    public XmlElem AddChild()
    {
      XmlElem child = new XmlElem();

      child.mParentElem = this;
      child.Depth = Depth + 1;
      child.ValueForm = ValueForm.None;

      // create the ChildElem LinkedList of this parent. 
      if (mChildElems == null)
      {
        mChildElems = new LinkedList<XmlElem>();
      }

      child.mNode = ChildElems.AddLast( child ) ;

      return child;
    }

    public XmlElem
      FindChildElem(string[] InKeys)
    {
      XmlElem found = FindChildDeep(InKeys, 0);
      return found;
    }

    public XmlElem
      FindChildElem(string[] InKeys, int InStartKeyLevel)
    {
      XmlElem found = FindChildDeep(InKeys, InStartKeyLevel);
      return found;
    }

    public XmlElem
      FindChildElem(string InKey)
    {
      XmlElem found = null;
      string[] keys = new string[] { InKey };
      found = FindChildDeep(keys, 0);
      return found;
    }

    private XmlElem
      FindChildDeep(string[] InKeys, int InKeyLevel)
    {
      XmlElem foundElem = null;

      // check for ElemName match in each sub element of this element.
      LinkedListNode<XmlElem> node = mChildElems.First;
      while (node != null)
      {
        XmlElem elem = node.Value;
        if (elem.ElemName == InKeys[InKeyLevel])
        {
          // at the last item in the keys array.
          // The searched for element is found.  
          if (InKeyLevel == (InKeys.Length - 1))
            foundElem = elem;

            // drill down to the next level, searching for the next
          // key in the search array.
          else
          {
            foundElem = elem.FindChildDeep(InKeys, InKeyLevel + 1);
          }

          if (foundElem != null)
            break;
        }
        node = node.Next;
      }

      return foundElem;
    }

    public XmlElem FirstChild()
    {
      if (mChildElems == null)
        return null;
      else
      {
        LinkedListNode<XmlElem> node = mChildElems.First;
        if (node == null)
          return null;
        else
          return node.Value;
      }
    }

    /// <summary>
    /// get the child XmlElem of this parent XmlElem which follows
    /// the specified current child XmlElem.
    /// </summary>
    /// <param name="InCurrent"></param>
    /// <returns></returns>
    public XmlElem NextChild(XmlElem InCurrent)
    {
      if (InCurrent == null)
        return FirstChild();
      else if (InCurrent.Parent != this)
        throw new ApplicationException(
          "current XmlElem is not a child XmlElem of this parent XmlElem");
      else
      {
        LinkedListNode<XmlElem> curNode = InCurrent.Node;
        LinkedListNode<XmlElem> node = curNode.Next;
        if (node == null)
          return null;
        else
          return node.Value;
      }
    }

    /// <summary>
    /// The element and its child elements in xml stream string form.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      if (ValueForm == ValueForm.Attribute)
      {
        string encodedValue = XmlStream.EncodeXmlString(ElemValue);
        return ElemName + "=\"" + encodedValue + "\"";
      }

        // only the top level root element is unnamed.
      else if (ElemName == null)
        return ToString_RootElem();

      else
      {
        return ToString_NonAttributeElem();
      }
    }

    private string ToString_RootElem()
    {
      StringBuilder sb = new StringBuilder();
      foreach (XmlElem child in this)
      {
        sb.Append(child.ToString());
        sb.Append(Environment.NewLine);
      }
      return sb.ToString();
    }

    // An attribute in the xml element is the name=value string that follows the
    // xml element name. A non attribute elem does not have any such attributes.
    // ( the XmlElem class does not yet support attributes )
    private string ToString_NonAttributeElem()
    {
      StringBuilder sb = new StringBuilder();
      
      // start the xml elem.
      sb.Append("<" + ElemName + " ");

      // add the Attribute child elements to the string.
      bool nonNmvChildElems = false;
      foreach (XmlElem child in this)
      {
        if (child.ValueForm == ValueForm.Attribute)
        {
          sb.Append(child.ToString());
        }
        else
          nonNmvChildElems = true;
      }

      // no value, no child elems. Close out this end node elem.
      if ((nonNmvChildElems == false) && (ElemValue == null))
        sb.Append("/>");
      else

        // close out the open unit.
      {
        sb.Append(">");

        // followed by the element value 
        if (ElemValue != null)
          sb.Append( XmlStream.EncodeXmlString(ElemValue));

        // next, the non named value sub elements.
        if (nonNmvChildElems == true)
        {
          sb.Append(Environment.NewLine);
          foreach (XmlElem child in this)
          {
            if (child.ValueForm != ValueForm.Attribute)
            {
              sb.Append(child.ToString());
              sb.Append(Environment.NewLine);
            }
          }
          if ( sb.Tail(2) != Environment.NewLine ) 
            sb.Append(Environment.NewLine);
        }

        // the close unit.
        sb.Append("</" + ElemName + ">");
      }

      return sb.ToString();
    }
    #endregion

    #region IEnumerable Members

    IEnumerator<XmlElem> IEnumerable<XmlElem>.GetEnumerator()
    {
      if (HasChildElems == false)
        yield break;
      else
      {
        foreach (XmlElem elem in mChildElems)
        {
          yield return elem;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion


  }

}
