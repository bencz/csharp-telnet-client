using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Xml;
using AutoCoder.Ext.System;
using System.Collections;

namespace AutoCoder.Xml
{
  public class XmlStringBuilder 
  {
    StringBuilder mContent = new StringBuilder();
    Elem mDocElem = null;
    bool mHasDeclaration = false ;

    public XmlStringBuilder()
    {
    }

    #region properties
    /// <summary>
    /// debug use. returns the last 128 characters of the content being built. 
    /// </summary>
    public string ContentTail
    {
      get
      {
        if (mContent == null)
          return "";
        else
          return mContent.Tail(128);
      }
    }

    /// <summary>
    /// returns the top level element of the xml string.
    /// </summary>
    public Elem DocumentElem
    {
      get { return mDocElem; }
    }

    /// <summary>
    /// EmptyDocumentElem is used when a method accepts an XmlStringBuilder.Elem
    /// argument, but the XmlStringBuilder does not yet have any elements. When
    /// a child element is appended to an EmptyDocumentElem, the
    /// AppendDocumentElem method of the XmlStringBuilder is called instead.
    /// </summary>
    public Elem EmptyDocumentElem
    {
      get
      {
        Elem emptyElem = null;
        if (mDocElem != null)
          throw new ApplicationException("XmlString already has a document element");
        else
        {
          emptyElem = new Elem(this, true);
        }
        return emptyElem;
      }
    }
    #endregion

    #region methods
    public void AppendDeclaration( )
    {
      mContent.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
      mContent.Append(Environment.NewLine);
      mHasDeclaration = true ;
    }

    /// <summary>
    /// Append text asis to the XmlString ( the text is already encoded )
    /// </summary>
    /// <param name="InText"></param>
    void AppendEncodedText( string InText )
    {
      mContent.Append( InText ) ;
    }

    public Elem AppendDocumentElem(string InName)
    {
      if (mDocElem != null)
        throw new ApplicationException("XmlString already has a document element");
      else
      {
        mDocElem = new Elem(this, InName);
      }
      return mDocElem ;
    }

    /// <summary>
    /// Throw an exception if the xml string has a declaration element. ( if the
    /// XmlStringBuilder is used as a sub elem of another XmlStringBuilder, the
    /// declaration element must not be spcfd.
    /// </summary>
    public void ThrowHasDeclaration( )
    {
      if ( mHasDeclaration == true )
        throw new ApplicationException( "xml string has declaration element" ) ;
    }

    public override string ToString()
    {
      DocumentElem.CloseElem();
      return mContent.ToString();
    }
    #endregion

    // -------------------------------- Elem -------------------------------------------
    public class Elem : IEnumerable<Elem>
    {
      XmlStringBuilder mXmlString = null ;
      Elem mCurrentSubElem = null;
      Elem mParentElem = null ;
      string mElemName = null ;
      List<Elem> mSubElems = null ;
      string mElemValue = null ;
      bool mOpenUnitIsClosed = false;
      bool mIsClosed = false;
      bool mIsOpened = false;
      bool mIsEmptyDocElem = false;

      #region constructors

      public Elem(XmlStringBuilder InXmlString, bool InIsEmptyDocumentElem)
      {
        mXmlString = InXmlString;
        mIsEmptyDocElem = InIsEmptyDocumentElem;
      }

      public Elem( XmlStringBuilder InXmlString, string InName )
      {
        mXmlString = InXmlString ;
        mElemName = InName ;
        AppendElemOpen();
      }

      public Elem( Elem InParent, string InName)
      {
        mParentElem = InParent;
        mXmlString = mParentElem.mXmlString;
        mElemName = InName;
      }
      #endregion

      #region properties
      public Elem CurrentSubElem
      {
        get { return mCurrentSubElem; }
      }

      public bool HasSubElems
      {
        get
        {
          if ((mSubElems != null) && (mSubElems.Count > 0))
            return true;
          else
            return false;
        }
      }

      public string ElemName
      {
        get { return mElemName ; }
      }

      public string ElemValue
      {
        get { return mElemValue; }
      }

      public bool IsEmptyDocumentElem
      {
        get { return mIsEmptyDocElem ; }
        set { mIsEmptyDocElem = value ; }
      }

      public XmlStringBuilder XmlString
      {
        get { return mXmlString ; }
      }
      #endregion

      #region methods
      public Elem AppendAttribute(string InName, string InValue)
      {
        if (mOpenUnitIsClosed == true)
          throw new ApplicationException(
            "xml elem open unit is closed. No more attributes allowed.");
        mXmlString.AppendEncodedText(InName + "=");
        mXmlString.AppendEncodedText("\"" + XmlStream.EncodeXmlString(InValue) + "\" ");
        return this;
      }

      /// <summary>
      /// append a child element to this parent element.
      /// </summary>
      /// <param name="InName"></param>
      /// <returns></returns>
      public Elem AppendSubElem(string InName)
      {
        Elem subElem = null;

        // this parent elem is a placeholder for an empty XmlStringBuilder.
        if (mIsEmptyDocElem == true)
          subElem = XmlString.AppendDocumentElem(InName);

        else
        {
          subElem = new Elem(this, InName);
          AppendSubElem_Common(subElem);
          subElem.AppendElemOpen();
        }

        return subElem;
      }

      public Elem AppendSubElem(string InName, string InValue)
      {
        Elem subElem = null ;
        
        // this parent elem is a placeholder for an empty XmlStringBuilder.
        if (mIsEmptyDocElem == true)
          subElem = XmlString.AppendDocumentElem(InName);
        
        else
        {
          subElem = new Elem(this, InName);
          AppendSubElem_Common(subElem);
          subElem.AppendElemOpen();
        }

        subElem.AppendElemValue(InValue);
        return subElem;
      }

      public Elem AppendSubElem( XmlStringBuilder InXml )
      {
        Elem subElem = AppendSubElem_FromChildElement(InXml.DocumentElem);
        return subElem;
      }

      private Elem AppendSubElem_FromChildElement(Elem InFromChild)
      {
        Elem toChild = null;
        if (InFromChild.ElemValue != null)
        {
          toChild = AppendSubElem(InFromChild.ElemName, InFromChild.ElemValue);
        }
        else
          toChild = AppendSubElem(InFromChild.ElemName);

        if (InFromChild.HasSubElems == true)
          toChild.AppendSubElem_FromChildElements(InFromChild);

        return toChild;
      }

      private void AppendSubElem_FromChildElements(Elem InFromParent)
      {
        foreach (Elem fromChild in InFromParent)
        {
          AppendSubElem_FromChildElement(fromChild);
        }
      }

      /// <summary>
      /// An Elem being appended to the string, of which this elem is the parent.
      /// </summary>
      /// <param name="InChild"></param>
      private void AppendSubElem_Common(Elem InSubElem)
      {
        if (mOpenUnitIsClosed == false)
          AppendOpenUnitClose();

        // close the current sub element of this parent elem. 
        if (mCurrentSubElem != null)
        {
          mCurrentSubElem.CloseElem();
          mCurrentSubElem = null;
        }

        // set the current subElem of this parent.
        mCurrentSubElem = InSubElem;

        // add to list of sub elements of this parent.
        if (mSubElems == null)
          mSubElems = new List<Elem>();
        mSubElems.Add(InSubElem);
      }

      private void AppendElemClose()
      {
        if ((mIsClosed == false) && ( mIsOpened == true ))
        {
          if ((mElemValue != null) || (mSubElems != null))
            mXmlString.AppendEncodedText("</" + mElemName + ">");
          else
            mXmlString.AppendEncodedText("/>");
          mIsClosed = true;
          mXmlString.AppendEncodedText(Environment.NewLine);
        }
      }

      private void AppendElemOpen()
      {
        mXmlString.AppendEncodedText("<" + mElemName + " ");
        mIsOpened = true;
      }

      public Elem AppendElemValue(string InText)
      {
        // store the element value.
        if (mElemValue != null)
          throw new ApplicationException("element already has a value");
        mElemValue = InText;
        
        // this element has a currently open sub element. Before adding
        // the value to the xml string, have to close the sub element.
        if (mCurrentSubElem != null)
        {
          mCurrentSubElem.CloseElem();
          mCurrentSubElem = null;
        }

        if (mOpenUnitIsClosed == false)
          AppendOpenUnitClose();

        string encodedValue = XmlStream.EncodeXmlString(mElemValue);
        mXmlString.AppendEncodedText(encodedValue);

        return this;
      }

      private void AppendOpenUnitClose()
      {
        if (mOpenUnitIsClosed == true)
          throw new ApplicationException("Open unit already closed");
        mXmlString.AppendEncodedText(">");
        mOpenUnitIsClosed = true;
      }

      public void CloseElem()
      {
        if (mCurrentSubElem != null)
        {
          mCurrentSubElem.CloseElem();
          mCurrentSubElem = null;
        }
        AppendElemClose();
      }

      /// <summary>
      /// An Elem being appended to the string, of which this elem is the parent.
      /// </summary>
      /// <param name="InChild"></param>
      private void SubElemAdded(Elem InSubElem)
      {
        if (mOpenUnitIsClosed == false)
          AppendOpenUnitClose();
        
        // close the current sub element of this parent elem. 
        if (mCurrentSubElem != null)
        {
          mCurrentSubElem.CloseElem();
          mCurrentSubElem = null;
        }

        // set the current subElem of this parent.
        mCurrentSubElem = InSubElem;

        // add to list of sub elements of this parent.
        if (mSubElems == null)
          mSubElems = new List<Elem>();
        mSubElems.Add(InSubElem);
      }

    #endregion 

      #region IEnumerable Members
      IEnumerator<Elem> IEnumerable<Elem>.GetEnumerator()
      {
        if ((mSubElems == null) || (mSubElems.Count == 0))
          yield break;
        else
        {
          foreach (Elem elem in mSubElems)
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
}
