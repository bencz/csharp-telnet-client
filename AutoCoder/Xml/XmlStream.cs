using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using System.Collections;

namespace AutoCoder.Xml
{
  // Infrastructure : unit at the start of the document enclosed in <? .... ?>
  public enum XmlUnitCode
  { None, Open, Value, Close, Single, Attribute, InfraStructure } ;
  
  // how is the value of the element represented in the document. Either 
  // <Addr City="abc" />  or <City>abc</City>
  public enum ValueForm { Attribute, OpenCloseBraced, None } ;

  public enum ElemForm { Attribute, Single, OpenCloseBraced, None } ;

  public class XmlStream : IEnumerable<XmlElem>
  {
    string mStream;
    TextTraits mTraits;
    LinkedList<XmlUnit> mUnits = null;
    XmlElem mDocElem = null;

    public XmlStream()
    {
      ConstructCommon();
    }

    public XmlStream(string InStream)
    {
      ConstructCommon();
      DocumentStream = InStream;
    }

    /// <summary>
    /// the placeholder element that holds all the top level elements of the
    /// document.
    /// </summary>
    public XmlElem DocumentElem
    {
      get
      {
        if (mDocElem == null)
          ThrowIncorrectlyFormedXmlException("Document is missing a root element", null);
        return mDocElem;
      }
    }

    public string DocumentStream
    {
      get { return mStream; }
      set
      {
        mStream = value;
        mUnits = new LinkedList<XmlUnit>();
        CrackUnits();
        BuildDocumentElemTree();
      }
    }

    public XmlElem AddElem(string InElemName)
    {
      XmlElem elem = new XmlElem();
      return elem;
    }

    /// <summary>
    /// create the document root element. 
    /// </summary>
    /// <returns></returns>
    private XmlElem AddDocumentElem()
    {
      mDocElem = new XmlElem();
      mDocElem.Depth = 0;
      mDocElem.ValueForm = ValueForm.None;
      return mDocElem;
    }

    /// <summary>
    /// Build the Document element tree from the LinkedList of cracked
    /// document units.
    /// </summary>
    private void BuildDocumentElemTree()
    {
      LinkedListNode<XmlUnit> node = mUnits.First;
      while (node != null)
      {
        XmlUnit unit = node.Value;
        if (unit.UnitCode == XmlUnitCode.InfraStructure)
        { }
        else if (unit.UnitCode == XmlUnitCode.Single)
        {
          if (mDocElem != null)
            ThrowIncorrectlyFormedXmlException(
              "document has more than one root element", unit );
          else 
            node = BuildElemTree_SingleUnit( null, node);
        }
        else if (unit.UnitCode == XmlUnitCode.Open)
        {
          if (mDocElem != null)
            ThrowIncorrectlyFormedXmlException(
              "document has more than one root element", unit);
          else
            node = BuildElemTree_OpenUnit(null, node);
        }
        else
          ThrowIncorrectlyFormedXmlException("Incorrectly placed xml unit", unit);

        node = node.Next;
      }
    }

    void BuildElemTree_Attributes(XmlElem InParent, XmlUnit InParentUnit)
    {
      foreach (XmlUnit nmvUnit in InParentUnit.SubUnits)
      {
        XmlElem nmvElem = InParent.AddChild();
        nmvElem.ElemName = nmvUnit.UnitName;
        nmvElem.ElemValue = nmvUnit.UnitValue;
        nmvElem.ValueForm = ValueForm.Attribute;
      }
    }

    LinkedListNode<XmlUnit> BuildElemTree_SingleUnit(
      XmlElem InParent, LinkedListNode<XmlUnit> InSingleNode)
    {
      XmlUnit singleUnit = InSingleNode.Value;
      XmlElem singleElem = null;

      // create the XmlElem. Add to the parent.
      if (InParent == null)
        singleElem = AddDocumentElem();
      else
        singleElem = InParent.AddChild();
      singleElem.ElemName = singleUnit.UnitName;

      // the unit has named value properties. Store the named values as
      // child elements.
      if (singleUnit.SubUnits != null)
        BuildElemTree_Attributes(singleElem, singleUnit);

      return InSingleNode;
    }

    /// <summary>
    /// Build a branch in the XmlElem tree from an open XmlUnit.
    /// </summary>
    /// <param name="InParent"></param>
    /// <param name="InOpenNode"></param>
    /// <returns></returns>
    LinkedListNode<XmlUnit> BuildElemTree_OpenUnit(
      XmlElem InParent, LinkedListNode<XmlUnit> InOpenNode)
    {
      XmlElem openElem = null ;
      XmlUnit openUnit = InOpenNode.Value;
      LinkedListNode<XmlUnit> closeUnitNode = null;

      // create the XmlElem. Add to the parent.
      if (InParent == null)
        openElem = AddDocumentElem();
      else
        openElem = InParent.AddChild();
      openElem.ElemName = openUnit.UnitName;

      // the unit has named value properties. Store the named values as
      // child elements.
      if (openUnit.SubUnits != null)
        BuildElemTree_Attributes(openElem, openUnit);

      // step from unit to unit until the close unit.
      LinkedListNode<XmlUnit> curNode = InOpenNode;
      while (true)
      {
        curNode = curNode.Next;
        if (curNode == null)
          ThrowIncorrectlyFormedXmlException("Close unit of open unit not found", openUnit);

        // process the unit according to its UnitCode.
        XmlUnit unit = curNode.Value;

        // the value of the open unit.
        if (unit.UnitCode == XmlUnitCode.Value)
        {
          if (openElem.ElemValue != null)
            ThrowIncorrectlyFormedXmlException("xml element already has a value", openUnit);
          openElem.ElemValue = unit.UnitValue;
          openElem.ValueForm = ValueForm.OpenCloseBraced;
        }

        // the close unit.
        else if (unit.UnitCode == XmlUnitCode.Close)
        {
          if ((unit.UnitName != null) && (unit.UnitName != openElem.ElemName))
            ThrowIncorrectlyFormedXmlException("Close unit name does not match open unit", openUnit);
          else
          {
            closeUnitNode = curNode;
            break;
          }
        }

        // a single unit. process as a sub element of this open unit element.
        else if (unit.UnitCode == XmlUnitCode.Single)
        {
          curNode = BuildElemTree_SingleUnit(openElem, curNode);
        }

        // an open unit within this open unit. process as a sub element.
        else if (unit.UnitCode == XmlUnitCode.Open)
        {
          curNode = BuildElemTree_OpenUnit(openElem, curNode);
        }

        // any other kind of XmlUnit is an error.
        else
          ThrowIncorrectlyFormedXmlException("Unexpected XmlUnit", openUnit);
      }

      return closeUnitNode;
    }

    private void ConstructCommon()
    {
      mTraits = new TextTraits();
      mTraits.WhitespacePatterns.Replace(
        new string[] { " ", "\t", "\r", "\n", Environment.NewLine }, DelimClassification.Whitespace);
    }

    void CrackUnits()
    {
      XmlUnit curUnit = null;
      XmlUnit nxUnit = null;

      while (true)
      {
        curUnit = nxUnit;
        nxUnit = CrackUnits_CrackNext(curUnit);
        if (nxUnit == null)
          break;
        if (curUnit != null)
          curUnit.NextUnit = nxUnit;
        mUnits.AddLast(nxUnit);
      }
    }

    XmlUnit CrackUnits_CrackNext(XmlUnit InCurUnit)
    {
      int ix;
      int vluBx;
      XmlUnit unit = null;

      // calc scan start point
      if (InCurUnit == null)
        ix = 0;
      else if (InCurUnit.UnitCode == XmlUnitCode.None)
        ix = 0;
      else
        ix = InCurUnit.Bx + InCurUnit.Lx;

      // advance past whitespace
      vluBx = ix;
      int streamEx = mStream.Length - 1 ;
      ScanPatternResults res = Scanner.ScanNotEqual(
        mStream, ix, streamEx, mTraits.WhitespacePatterns ) ;
      ix = res.FoundPos ;

      // end of stream.
      if (ix == -1)
        unit = null;

        // some sort of open or close unit.
      else if (res.FoundChar.Value == '<')
      {
        Nullable<char> nxChar = Scanner.ScanNotEqual(
          mStream, ix + 1, streamEx, mTraits.WhitespacePatterns ).FoundChar ;

        // starting an xml close unit </name>
        if ((nxChar != null ) && (nxChar.Value == '/'))
        {
          unit = CrackUnits_ScanCloseUnit(res.FoundPos);
        }

          // a document infrastructure unit <? ...... ?>
        else if ((nxChar != null ) && ( nxChar.Value == '?'))
        {
          unit = CrackUnits_ScanInfrastructureUnit(res.FoundPos);
        }

        // starting an xml open unit <name xxxxxx>
        else
        {
          unit = CrackUnits_ScanOpenUnit(res.FoundPos);
        }
      }

        // a value unit is the xml text between the open and close unit.
      else
        unit = ScanValueUnit(vluBx);

      return unit;
    }

    private XmlUnit CrackUnits_ScanCloseUnit(int InBx)
    {
      Scanner.ScanCharResults res;
      XmlUnit unit = new XmlUnit();
      unit.UnitCode = XmlUnitCode.Close;
      WordCursor csr = null;

      BoundedString boundedStream = new BoundedString(mStream);

      // unit starts with "<"
      if (boundedStream[InBx] != '<')
        ThrowIncorrectlyFormedXmlException(InBx);
      unit.Bx = InBx;

      // scan for the end of the unit. ( there should be a > before an < )
      res = Scanner.ScanEqualAny_BypassQuoted(
        boundedStream, InBx + 1, new char[] { '>', '<' }, QuoteEncapsulation.Double);
      if ((res.ResultPos == -1) || (res.ResultChar == '<'))
        ThrowIncorrectlyFormedXmlException(InBx);
      else
        unit.Ex = res.ResultPos;

      // setup to step from word to word in the close unit.
      boundedStream = new BoundedString(mStream, InBx + 1, res.ResultPos - 1);
      TextTraits traits = new TextTraits();
      traits.OpenNamedBracedPatterns.Clear();
      traits.DividerPatterns.Add("/", "=", DelimClassification.DividerSymbol);

      // first word must be an empty word w/ "/" delim.
      csr = Scanner.ScanFirstWord(boundedStream, traits);
      if ((csr.IsDelimOnly) && (csr.DelimValue == "/"))
      { }
      else
        ThrowIncorrectlyFormedXmlException(InBx);

      // next is a name with end of string delim.
      csr = Scanner.ScanNextWord(boundedStream, csr);
      if ((csr.IsEndOfString) ||
        (csr.DelimClass == DelimClassification.EndOfString))
      { }
      else
        ThrowIncorrectlyFormedXmlException(InBx);

      // if there is an element name, store it.
      if (csr.Word != null)
        unit.NameWord = csr;

      return unit;
    }

    XmlUnit CrackUnits_ScanOpenUnit(int InBx)
    {
      Scanner.ScanCharResults res;
      XmlUnit unit = new XmlUnit();
      unit.UnitCode = XmlUnitCode.Open;
      WordCursor nxWord = null;

      BoundedString boundedStream = new BoundedString(mStream);

      // unit starts with "<"
      if (boundedStream[InBx] != '<')
        ThrowIncorrectlyFormedXmlException(InBx);
      unit.Bx = InBx;

      // scan for the end of the unit. ( there should be a > before an < )
      res = Scanner.ScanEqualAny_BypassQuoted(
        boundedStream, InBx + 1, new char[] { '>', '<' }, QuoteEncapsulation.Double);
      if ((res.ResultPos == -1) || (res.ResultChar == '<'))
        ThrowIncorrectlyFormedXmlException(InBx);
      else
        unit.Ex = res.ResultPos;

      // setup to step from word to word in the unit.
      boundedStream = new BoundedString(mStream, InBx + 1, res.ResultPos - 1);
      TextTraits traits = new TextTraits();
      traits.OpenNamedBracedPatterns.Clear( ) ;
      traits.DividerPatterns.Add( "/", "=", DelimClassification.DividerSymbol );
      traits.WhitespacePatterns.AddDistinct(
        Environment.NewLine, DelimClassification.Whitespace);

      // isolate the words of the open unit.
      WordCursor csr = Scanner.ScanFirstWord(boundedStream, traits);
      while (true)
      {
        if (csr.IsEndOfString == true)
          break;

        // the unit name
        if (ScanOpenUnit_CursorAtUnitName(csr) == true)
        {
          if (unit.NameWord != null)
            ThrowIncorrectlyFormedXmlException(InBx); // already have a unit name
          else
          {
            unit.NameWord = csr;
          }
        }

          // no word. just the ending "/".  ( handle a little later. )
        else if ((csr.Word == null) && (csr.DelimValue == "/"))
        {
        }
        else if (csr.Word == null)
          ThrowIncorrectlyFormedXmlException(InBx);

          // handle as an element attribute ( a named value pair )
        else
        {
          nxWord = ScanOpenUnit_Attribute_GetValue(boundedStream, csr);
          if (nxWord != null)
          {
            // note: attributes values are stored in their xml encoded
            //       state. 
            unit.AddAttribute(csr, nxWord);
            csr = nxWord;
          }
          else
            ThrowIncorrectlyFormedXmlException(InBx);
        }

        // process the "/" delimeter. ( must be the end of the OpenUnit )
        if (csr.DelimValue == "/")
        {
          WordCursor nx = Scanner.ScanNextWord(boundedStream, csr);
          if (nx.IsEndOfString == true)
          {
            unit.UnitCode = XmlUnitCode.Single;
            break;
          }
          else
            ThrowIncorrectlyFormedXmlException(InBx);
        }

        csr = Scanner.ScanNextWord(boundedStream, csr);
      }

      return unit;
    }

    /// <summary>
    /// an Infrastructure element is braced by <? .... ?>
    /// </summary>
    /// <param name="InBx"></param>
    /// <returns></returns>
    private XmlUnit CrackUnits_ScanInfrastructureUnit(int InBx)
    {
      XmlUnit unit = null;
      Scanner.ScanCharResults res;

      BoundedString boundedStream = new BoundedString(mStream);

      // scan for the end of the unit. ( there should be a > before an < )
      res = Scanner.ScanEqualAny_BypassQuoted(
        boundedStream, InBx + 1, new char[] { '>', '<' }, QuoteEncapsulation.Double);
      if ((res.ResultPos == -1) || (res.ResultChar == '<'))
        ThrowIncorrectlyFormedXmlException(InBx);

        // "?" within the angle brackets.
      else if ((boundedStream[InBx + 1] != '?') ||
        (boundedStream[res.ResultPos - 1] != '?'))
        ThrowIncorrectlyFormedXmlException(InBx);

        // setup the XmlUnit.
      else
      {
        unit = new XmlUnit();
        unit.UnitCode = XmlUnitCode.InfraStructure;
        unit.Bx = InBx;
        unit.Ex = res.ResultPos;
        unit.UnitValue = boundedStream.Substring(unit.Bx, unit.Lx);
      }

      return unit;
    }

    public static string DecodeXmlString(string InText)
    {
      int lx;
      StringBuilder sb = new StringBuilder(InText.Length);
      int ix = 0;
      while (ix < InText.Length)
      {
        int fx = InText.IndexOf('&', ix);
        if (fx == -1)
        {
          sb.Append(InText.Substring(ix));
          break;
        }

        // copy text up to the "&" asis to the decoded output.
        if (fx > ix)
        {
          lx = fx - ix;
          sb.Append(InText.Substring(ix, lx));
        }
        ix = fx;

        // test that an encoded char has been found.
        string encodedText = Stringer.SubstringPadded(InText, ix, 6);
        if (encodedText.Substring(0, 4) == "&gt;")
        {
          sb.Append('>');
          ix += 4;
        }
        else if (encodedText.Substring(0, 4) == "&lt;")
        {
          sb.Append('<');
          ix += 4;
        }
        else if (encodedText.Substring(0, 5) == "&amp;")
        {
          sb.Append('&');
          ix += 5;
        }
        else if (encodedText.Substring(0, 6) == "&apos;")
        {
          sb.Append('\'');
          ix += 6;
        }
        else if (encodedText.Substring(0, 6) == "&quot;")
        {
          sb.Append('"');
          ix += 6;
        }

        // unexpected encoded character.
        else
          throw new ApplicationException("unexpected xml encoded character " + encodedText);
      }

      return sb.ToString();
    }

    public static string EncodeXmlChar(char InChar)
    {
      if (InChar == '<')
        return "&lt;";
      else if (InChar == '>')
        return "&gt;";
      else if (InChar == '&')
        return "&amp;";
      else if (InChar == '"')
        return "&quot;";
      else if (InChar == '\'')
        return "&apos;";
      else
        return new string( InChar, 1 );
    }

    public static string EncodeXmlString(string InText)
    {
      int fx = 0 ;
      int ix = 0;
      int asisLx = 0 ;
      int asisBx = 0 ;
      char[] encodeChars = new char[] { '<', '>', '&', '"', '\'' };
      StringBuilder sb = new StringBuilder(InText.Length + 512);

      while (ix < InText.Length)
      {
        fx = InText.IndexOfAny(encodeChars, ix);

        // quick handle of input string that contains no characters thar require
        // encoding.
        if ((fx == -1) && (ix == 0))
          return InText;

        // extract the characters that dont require encoding.
        if (fx == -1)
        {
          asisBx = ix;
          asisLx = InText.Length - ix;
        }
        else
        {
          asisBx = ix;
          asisLx = fx - ix;
        }
        if (asisLx > 0)
          sb.Append(InText.Substring(asisBx, asisLx));

        // encode the character.
        if (fx >= 0)
        {
          sb.Append(EncodeXmlChar(InText[fx]));
          ix = fx + 1;
        }
        else
          ix = InText.Length;
      }

      return sb.ToString(); 
    }

    public XmlElem FindChildElem(string[] InKeys)
    {
      XmlElem foundElem = null;
      if (InKeys.Length == 0)
      { }
      else if (DocumentElem.ElemName != InKeys[0])
      { }
      else
      {
        foundElem = DocumentElem.FindChildElem(InKeys, 1);
      }

      return foundElem;
    }

    public XmlElem FindChildElem(string InKey)
    {
      XmlElem foundElem = null;
      string[] keys = new string[] { InKey };
      foundElem = DocumentElem.FindChildElem(keys);
      return foundElem;
    }

    /// <summary>
    /// Return the top level XmlElem of the document. Use the 
    /// VerifyElemName argument to throw an error if the name of the
    /// document element is not the one expected.
    /// </summary>
    /// <param name="InVerifyElemName"></param>
    /// <returns></returns>
    public XmlElem GetDocumentElem(string InVerifyElemName)
    {
      if (InVerifyElemName != DocumentElem.ElemName)
        throw new ApplicationException(
          "Document does not contain document element " + InVerifyElemName);
      return DocumentElem;
    }

    static void ThrowIncorrectlyFormedXmlException(int InStartIx)
    {
      throw new ApplicationException(
        "Incorrectly formed XML element starting at position " +
        InStartIx.ToString());
    }

    static void ThrowIncorrectlyFormedXmlException(
      string InText, XmlUnit InUnit)
    {
      throw new ApplicationException(
        "Incorrectly formed XML. " + InText);
    }

    /// <summary>
    /// The WordCursor locates the named part of a named=value pair.
    /// Advance to and return the WordCursor of the value part.
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InCsr"></param>
    /// <returns></returns>
    private static WordCursor ScanOpenUnit_Attribute_GetValue(
      BoundedString InBoundedString, WordCursor InCsr)
    {
      if (InCsr.Word == null)
        return null;
      else if (InCsr.DelimValue != "=")
        return null;

      // scan to the value part of the attribute.
      WordCursor nxCsr = Scanner.ScanNextWord(InBoundedString, InCsr);
      
      // no value to scan to. the caller should handle this as a 
      // mal formed xml error. 
      if (nxCsr.IsEndOfString == true)
        return null;
      else if (nxCsr.Word == null)
        return null;

        // got a word. is an attribute value as long as the delimeter
        // is legit.
      else if (nxCsr.DelimClass == DelimClassification.Whitespace)
        return nxCsr;
      else if (nxCsr.DelimClass == DelimClassification.EndOfString)
        return nxCsr;
      else if (nxCsr.DelimValue == "/")
        return nxCsr;

        // likely the wrong type of delimiter. return null so the caller
        // can signal malformed xml error.
      else
        return null;
    }

    private static bool ScanOpenUnit_CursorAtUnitName(WordCursor InCsr)
    {
      if (InCsr.Word == null)
        return false;
      else if (InCsr.DelimValue == "=")
        return false;
      else if ((InCsr.DelimClass == DelimClassification.EndOfString) ||
        (InCsr.DelimClass == DelimClassification.Whitespace) ||
        (InCsr.DelimValue == "/"))
      {
        return true;
      }
      else
      {
        ThrowIncorrectlyFormedXmlException(InCsr.ScanBx);
        return false;
      }
    }

    XmlUnit ScanValueUnit(int InBx)
    {
      Scanner.ScanCharResults res;
      XmlUnit unit = new XmlUnit();
      unit.UnitCode = XmlUnitCode.Value;

      // unit value starts immed after prior open unit. ( whitespace is significant )
      unit.Bx = InBx;

      // value runs until an xml bracket char.
      char[] braceChars = new char[] { '<', '>' };
      res = Scanner.ScanEqualAny(mStream, unit.Bx, braceChars);
      if (res.ResultChar == '>')
        throw new ApplicationException("xml close bracket not preceeded by open bracket");
      else if (res.ResultPos == -1)
        unit.Ex = mStream.Length - 1;
      else
        unit.Ex = res.ResultPos - 1;

      // store the unit value.
      if (unit.Bx != -1)
      {
        unit.UnitValue =
          DecodeXmlString(mStream.Substring(unit.Bx, unit.Ex - unit.Bx + 1));
      }

      return unit;
    }

    public string UnitToString(XmlUnit InUnit)
    {
      int bx = InUnit.Bx;
      int lx = InUnit.Lx;
      if (lx > 0)
        return mStream.Substring(bx, lx);
      else
        return "";
    }

    IEnumerator<XmlElem> IEnumerable<XmlElem>.GetEnumerator()
    {
      if (DocumentElem == null)
        yield break;
      else
      {
        foreach (XmlElem elem in DocumentElem )
        {
          yield return elem;
        }
      }
    }

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion
  
  }
}
