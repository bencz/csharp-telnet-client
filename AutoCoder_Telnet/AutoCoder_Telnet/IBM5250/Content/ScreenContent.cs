using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ScreenContent : IDataStreamReport
  {
    public static byte[] EbcdicWhitespace = { 0x00, 0x40 };
    public ScreenContent Parent
    { get; set; }

    public ScreenContent[] Children
    { get; set; }

    /// <summary>
    /// index into Children array of the current child screenContent block.
    /// This is the window which has focus or which WTD orders are being applied
    /// to.
    /// </summary>
    public int? CurrentChildIndex
    { get; set; }

    public ScreenContent CurrentChildContent
    {
      get
      {
        if (CurrentChildIndex == null)
          return null;
        else
        {
          var ix = this.CurrentChildIndex.Value;
          return this.Children[ix];
        }
      }
    }

    public byte[] ContentArray
    { get; set; }
    private int ContentArrayLx
    { get; set; }

    public ContentDict FieldDict
    { get; set; }
    private bool FieldDictContainsContentText
    { get; set; }

    public ScreenDim ScreenDim
    { get; set; }

    public bool DoClearUnit
    { get; set; }

    public OneRowCol CaretRowCol
    { get; set; }

    public bool HasChildren
    {
      get
      {
        if ((this.Children == null) || (this.Children.Length == 0))
          return false;
        else
          return true;
      }
    }
    public HowReadScreen? HowRead
    { get; set; }

    /// <summary>
    /// odom used to assign unique number to fields within the content block.
    /// </summary>
    private int FieldNumOdom
    { get; set; }

    /// <summary>
    /// the AidKey that was posted to the server which resulted in this screen
    /// content being filled and displayed.
    /// </summary>
    public AidKey? PostAidKey
    { get; set; }

    /// <summary>
    /// start pos of the window on the parent screen. ( this is the start pos
    /// of the ItemCanvas. Does not account for border of the window. )
    /// </summary>
    public ZeroRowCol StartRowCol
    {
      get
      {
        if (_Start == null)
          _Start = new ZeroRowCol(0, 0, this);
        return _Start;
      }
      set { _Start = value; }
    }
    private ZeroRowCol _Start;

    /// <summary>
    /// the startRowCol of this SCB converted to a CharPoint ( more simple object. )
    /// </summary>
    public CharPoint StartCharPoint
    {
      get
      {
        if (_Start == null)
          return new CharPoint(0, 0);
        else
          return StartRowCol.ToCharPoint();
      }
    }

    /// <summary>
    /// see ContentFieldKey. segment num combines with field num to form fieldKey.
    /// Segment number is assigned to segments of continued entry fields. Current
    /// segnum stores the segnum of the prior created continued entry field.
    /// </summary>
    private ContentFieldKey CurrentFieldKey
    { get; set; }

    /// <summary>
    /// reference to global odom that assigns a unique ContentNum when creating a
    /// new ScreenContent.
    /// </summary>
    public ConcurrentOdom ContentOdom
    { get; private set; }

    /// <summary>
    /// unique id number of this ScreenContent block. A new ContentNum is
    /// assigned each time ClearUnit is run and a new Master is created. Also
    /// a new ContentNum is assigned when a window ScreenContent is created.
    /// ContentNum is not assigned when copying the ScreenContent.
    /// </summary>
    public int ContentNum
    { get; private set; }

    /// <summary>
    /// enum that indicates if this SCB is a window within a screen level content
    /// block. Or the larger screen itself.
    /// </summary>
    public RowColRelative ContentRelative
    {
      get
      {
        if (this.Parent == null)
          return RowColRelative.Parent;
        else
          return RowColRelative.Local;
      }
    }

    public string ReportTitle
    {
      get
      {
        return "ScreenContent";
      }
    }

    public ScreenContent(
      ScreenContent Parent, ScreenDim ScreenDim, ConcurrentOdom ContentOdom)
      : this(ContentOdom)
    {
      this.ScreenDim = ScreenDim;

      this.ContentArray = new byte[4000];
      this.ContentArrayLx = this.ScreenDim.Width * this.ScreenDim.Height;

      this.FieldDict = new ContentDict();

      // add this ScreenContent as a child of the Parent SCB.
      if ( Parent != null)
      {
        Parent.AddChild(this);
      }
    }

    private ScreenContent(ConcurrentOdom ContentOdom)
    {
      this.ContentOdom = ContentOdom;
      this.ContentNum = this.ContentOdom.Assign();
    }

    /// <summary>
    /// create new ScreenContent that is a copy of input parm ScreenContent.
    /// </summary>
    /// <param name="ToParent"></param>
    /// <param name="From"></param>
    public ScreenContent(ScreenContent ToParent, ScreenContent From)
    {
      this.Parent = ToParent;
      this.ContentArray = From.ContentArray.SubArray(0);
      this.FieldDict = From.FieldDict.Copy();
      this.FieldDictContainsContentText = From.FieldDictContainsContentText;
      this.ScreenDim = From.ScreenDim;
      this.StartRowCol = From.StartRowCol;
      this.ContentOdom = From.ContentOdom;
      this.ContentNum = From.ContentNum;

      this.DoClearUnit = From.DoClearUnit;
      this.CaretRowCol = From.CaretRowCol;
      this.HowRead = From.HowRead;
      this.PostAidKey = From.PostAidKey;

      // copy the Children ScreenContent also.
      if (From.Children != null)
      {
        var childList = new List<ScreenContent>();
        foreach (var child in From.Children)
        {
          var sc = child.Copy(this);
          childList.Add(sc);
        }
        this.Children = childList.ToArray();
      }
      this.CurrentChildIndex = From.CurrentChildIndex;
    }

    public void AddAllContentText( )
    {
      if ( this.FieldDictContainsContentText == false )
      {
        this.FieldDict.AddAllContentText(this);
        this.FieldDictContainsContentText = true;
      }
    }

    public ScreenContent FindScreenContent(int parentContentNum, int childContentNum)
    {
      ScreenContent found = null;
      if ( parentContentNum == this.ContentNum)
      {
        if ( this.Children != null )
        {
          foreach(var child in this.Children)
          {
            if ( child.ContentNum == childContentNum)
            {
              found = child;
              break;
            }
          }
        }
      }
      return found;
    }

    /// <summary>
    /// add the Child ScreenContent to Children collection of this parent.
    /// </summary>
    /// <param name="Child"></param>
    private void AddChild(ScreenContent Child)
    {
      // the child points up to this parent.
      Child.Parent = this;

      // add child to end of list of Children.
      if ( this.Children == null)
      {
        this.Children = new ScreenContent[1];
        this.Children[0] = Child;
      }
      else
      {
        var childList = this.Children.ToList();
        childList.Add(Child);
        this.Children = childList.ToArray();
      }
    }

    public void ApplyInput(KeyboardInputMessage Input)
    {
      ContentField contentField = null;
      var showText = Input.Text;

      // find the content field in which the input is being applied.
      contentField = this.GetContentField(Input.RowCol);

      // set the "modified" flag of the input field.
      if (contentField != null)
        contentField.ModifiedFlag = true;

      // change the input text to uppercase.
      if ((contentField != null) && (contentField.IsMonocase == true))
      {
        showText = showText.ToUpper();
      }

      // apply the text to the array of content bytes.
      ApplyTextBytes(Input.RowCol, showText.ToEbcdicBytes());
    }

    /// <summary>
    /// assign segment number key of a new ContentField. See ContentFieldKey class
    /// for usage.
    /// </summary>
    /// <param name="SegCode"></param>
    /// <returns></returns>
    public ContentFieldKey AssignFieldKey(ContinuedFieldSegmentCode? SegCode)
    {
      ContentFieldKey key = null;
      int segNum = 0;
      int fieldNum = 0;
      if (SegCode == null)
      {
        fieldNum = this.NextFieldNum();
        segNum = 0;
      }
      else if (SegCode.Value == ContinuedFieldSegmentCode.First)
      {
        fieldNum = this.NextFieldNum();
        segNum = 1;
      }
      else
      {
        fieldNum = this.CurrentFieldKey.FieldNum;
        segNum = this.CurrentFieldKey.SegNum + 1;
      }

      key = new ContentFieldKey(fieldNum, segNum);
      this.CurrentFieldKey = key;

      return key;
    }

    public void ApplyTextBytes(IRowCol CurRowCol, byte[] TextBytes)
    {
      int lx = TextBytes.Length;
      var itemRange = new RowColRange(CurRowCol, lx);
      var buf = this.GetContentBytes(CurRowCol, lx);

      // get the fields located within the bounds of the StartField. All of these
      // fields are removed from the ScreenTable.
      var curIndex = this.ToContentIndex(CurRowCol);
      foreach (var contentField in this.ContentFields(itemRange))
      {
        var fieldIndex = contentField.GetContentIndex(this);
        var ix = fieldIndex - curIndex;
        if (TextBytes[ix].IsAttrByte() == false)
        {
          this.FieldDict.Remove(contentField.RowCol);
          SetContentBytes(contentField.RowCol, 0x00);
        }
      }

      // apply the textbytes ( including leading and trailing attrbyte ) directly
      // to the content array.
      {
        // store the attrByte at ItemRowCol location of the field.
        SetContentBytes(CurRowCol, TextBytes);
      }
    }

    public void ApplyCaretMove(CaretMoveMessage caretMove)
    {
      if (caretMove.RowCol == null)
        this.CaretRowCol = null;
      else
        this.CaretRowCol = caretMove.RowCol.ToOneRowCol() as OneRowCol;
    }

    public void ApplyTextDataOrder(IRowCol CurRowCol, TextDataOrder TextData)
    {
      int lx = TextData.ItemLength();
      var itemRange = new RowColRange(CurRowCol, lx);
      var buf = this.GetContentBytes(CurRowCol, lx);

      // get the fields located within the bounds of the StartField. All of these
      // fields are removed from the ScreenTable.
      if (buf.AllMatch(0x00) == false)
      {
        foreach (var contentField in this.ContentFields(itemRange))
        {
          var ix = contentField.RowCol.DistanceInclusive(CurRowCol) - 1;
          var textByte = TextData.RawBytes[ix];
          if ( textByte.IsAttrByte( ) == false )
          {
            this.FieldDict.Remove(contentField.RowCol);
            SetContentBytes(contentField.RowCol, 0x00);
          }
        }
      }

      // apply the textbytes ( including leading and trailing attrbyte ) directly
      // to the content array.
      {
        // store the attrByte at ItemRowCol location of the field.
        SetContentBytes(CurRowCol, TextData.RawBytes);
      }
    }

    /// <summary>
    /// build a byte stream containing the WTD workstation command orders from all
    /// of the visual items on the canvas. The visual item list is the equivalent of
    /// the 5250 format table.
    /// </summary>
    /// <returns></returns>
    public byte[] BuildOrderStream()
    {
      var ba = new ByteArrayBuilder();

      // start header order.
      {
        byte[] cmdKeySwitches = new byte[] { 0x00, 0x00, 0x00 };
        var buf = StartHeaderOrder.Build(0x00, 0x00, 24, cmdKeySwitches);
        ba.Append(buf);
      }

      // insert cursor order.
      {
        var rowCol = this.CaretRowCol;
        if ( rowCol == null)
        {
          rowCol = new OneRowCol(1, 1, this);
        }
        var buf = InsertCursorOrder.Build(rowCol);
        ba.Append(buf);
      }

      // build sets of SBA, StartField and TextData orders for each visual item. The
      // visual item represents something on the screen. Whether output literal or
      // and input field.
      // VisualItem visualItem = null;
      foreach (var dictItem in this.FieldDict)
      {
        var contentItem = dictItem.Value;
        {
          var buf = SetBufferAddressOrder.Build(
            contentItem.RowCol.ToOneRowCol() as OneRowCol);
          ba.Append(buf);
        }

        if (contentItem is ContentField)
        {
          var contentField = contentItem as ContentField;
          var ffw = contentField.FFW_Bytes;

          byte attrByte = contentField.GetAttrByte(this).Value;
          int lgth = contentField.LL_Length;
          var buf = StartFieldOrder.Build(ffw[0], ffw[1], attrByte, lgth);
          ba.Append(buf);
        }

        // create a text data order from either the text of the literal item. Or the
        // text value of the entry field.
        {
          byte[] buf = null;
          var s1 = contentItem.GetShowText(this);
          if (contentItem is ContentField)
          {
            buf = TextDataOrder.Build(s1, null, null);
          }
          else
          {
            buf = TextDataOrder.Build(s1, contentItem.GetAttrByte(this), null);
          }
          ba.Append(buf);
        }
      }

      return ba.ToByteArray(); ;
    }

    public void Clear()
    {
      Array.Clear(this.ContentArray, 0, this.ContentArray.Length);
      FieldDict.Clear();
      this.CaretRowCol = null;
      this.HowRead = null;
      this.FieldNumOdom = 0;
      this.CurrentFieldKey = null;
      this.Children = null;
      this.CurrentChildIndex = null;
      this.Parent = null;
      this.StartRowCol = null;
    }

    /// <summary>
    /// return a sequence containing each byte of the ContentArray within the
    /// specified range.
    /// </summary>
    /// <param name="Range"></param>
    /// <returns></returns>
    public IEnumerable<byte> ContentBytes(RowColRange Range)
    {
      var bx = ToContentIndex(Range.From);
      var lx = Range.Length;
      for( int ix = 0; ix < lx; ++ix )
      {
        byte contentByte = this.ContentArray[bx + ix];
        yield return contentByte;
      }
      yield break;
    }

    /// <summary>
    /// return the sequence of fields that start within the rowcol range of the
    /// content table.
    /// </summary>
    /// <param name="Range"></param>
    /// <returns></returns>
    public IEnumerable<ContentField> ContentFields(RowColRange Range)
    {
      int ix = 0;
      ContentItemBase contentItem = null;
      foreach( var b1 in ContentBytes(Range))
      {
        if ( b1.IsAttrByte( ))
        {
          var rowCol = Range.From.Advance(ix) as ZeroRowCol;
          var rc = this.FieldDict.TryGetValue(rowCol, out contentItem);
          if ((rc == true) && ( contentItem is ContentField ))
            yield return contentItem as ContentField;
        }
        ix += 1;
      }
      yield break;
    }

    public IEnumerable<ContentItemBase> ContentItems()
    {
      int ix = -1;
      var blankBytes = new byte[] { 0x00, 0x40 };
      while (true)
      {
        ix += 1;
        if (ix >= this.ContentArray.Length)
          break;

        // advance to next non blank.
        ix = this.ContentArray.IndexOfNotEqual(ix, blankBytes);
        if (ix == -1)
          break;
        var curByte = this.ContentArray[ix];

        // not an attribute byte. process bytes up until an attr byte as 
        // ContentText.
        if (curByte.IsAttrByte() == false)
        {
          var bx = ix;
          var rv = ContentArray_ScanUntilAttrByte(ix);
          var textBuf = rv.Item1;
          var abIx = rv.Item2;
          if (textBuf.Length > 0)
          {
            // only if the text bytes are not all blanks.
            var fx = textBuf.IndexOfNotEqual(0, blankBytes);
            if ( fx != -1 )
            {
              var rowcol = ix.ToZeroRowCol(this.ScreenDim);
              var contentText = new ContentText(rowcol, null, textBuf.Length, null);
              yield return contentText;
            }
          }

          // setup ix for next iteration of the loop.
          if (abIx == -1)
            ix = this.ContentArray.Length;
          else
            ix = abIx - 1;
          continue;
        }

        // Got attr byte. Check that start of a field defined in the FieldDict.
        {
          ContentItemBase contentItem = null;
          var rowCol = ix.ToZeroRowCol(this.ScreenDim);
          var rc = this.FieldDict.TryGetValue(rowCol, out contentItem);

          // attr byte is start of a field.
          if ((rc == true) && ( contentItem is ContentField))
          {
            var contentField = contentItem as ContentField;
            contentField.LoadContents(this);
            ix += 1;                           // advance past attr byte.
            ix =  (ix + contentField.LL_Length) - 1;
            yield return contentField;
            continue;
          }
        }

        // attribute byte is not a field. Look for whitespace immed after the attr
        // byte.
        {
          var abIx = ix;
          if ( curByte == 0x20)
          {
            var fx = this.ContentArray.IndexOfNotEqual(ix + 1, blankBytes);

            // nothing but whitespace after the attr byte. Setup ix so next iter will
            // be EOF.
            if (fx == -1)
            {
              ix = this.ContentArray.Length;
              continue;
            }

            // blanks until another attr byte. skip the current attr and the 
            // whitespace that follows.
            else if (this.ContentArray[fx].IsAttrByte() == true)
            {
              ix = fx - 1;
              continue;
            }
          }

          // got a text char to display as ContentText.
          {
            int textStart = abIx + 1;
            var rv = ContentArray_ScanUntilAttrByte(textStart);
            var rowCol = abIx.ToZeroRowCol(this.ScreenDim);
            var textLength = rv.Item1.Length;

            // the end of content text attr byte is x'20'. Treat as tail attr byte.
            int? tailAttrByteIx = null;
            int nextIx = rv.Item2;
            if ((nextIx != -1 ) && ( rv.Item3 == 0x20 ))
            {
              var nextByte = ContentArray_NextNonZeroByte(nextIx + 1);
              if ((nextByte == null) || (nextByte.Value.IsAttrByte( ) == true ))
              {
                tailAttrByteIx = nextIx;
                nextIx += 1;
              }
            }

            // no attribute found. trim blanks from byte buffer.
            if ( nextIx == -1 )
            {
              var byteBuf = rv.Item1;
              var fx = byteBuf.IndexOfLastNotEqual(blankBytes);
              if (fx == -1)
                textLength = 0;
              else
                textLength = fx + 1;
            }

            // return the text constant.
            var contentText = new ContentText(
              rowCol, this.ContentArray[abIx], textLength, tailAttrByteIx);
            yield return contentText;

            if (nextIx == -1)
              break;
            ix = nextIx - 1;
          }
        }
      }

      yield break;
    }

    /// <summary>
    /// scan ahead in ContentArray until an attribute byte. Return length of bytes
    /// scanned over. And return index of the attribute type.
    /// </summary>
    /// <param name="ix"></param>
    /// <returns></returns>
    private Tuple<byte[], int, byte> ContentArray_ScanUntilAttrByte( int Start )
    {
      int ix = Start - 1;
      int abIx = -1;
      byte attrByte = 0x00;
      int arrayLx = this.ContentArray.Length;
      while(true)
      {
        ix += 1;
        if (ix >= arrayLx)
          break;
        byte b1 = this.ContentArray[ix];
        if ( b1.IsAttrByte( ))
        {
          abIx = ix;
          attrByte = b1;
          break;
        }
      }

      // setup array of bytes that preceed the attr byte.
      byte[] buf = null;
      if (abIx == -1)
        buf = this.ContentArray.SubArray(Start);
      else
        buf = this.ContentArray.SubArray(Start, abIx - Start);

      return new Tuple<byte[], int, byte>(buf, abIx, attrByte);
    }

    /// <summary>
    /// scan aheah in ContentArray until a byte that is not 0x00.
    /// </summary>
    /// <param name="Start"></param>
    /// <returns></returns>
    private byte? ContentArray_NextNonZeroByte( int Start)
    {
      int ix = Start;
      byte? nextByte = null;
      while(true)
      {
        if (ix >= this.ContentArray.Length)
          break;
        if (this.ContentArray[ix] != 0x00)
        {
          nextByte = this.ContentArray[ix];
          break;
        }
        ix += 1;
      }
      return nextByte;
    }

    /// <summary>
    /// return a sequence of continued field segments of a fieldNum.
    /// </summary>
    /// <param name="FieldNum"></param>
    /// <returns></returns>
    public IEnumerable<ContinuedContentField> ContinuedFieldSegments(int FieldNum)
    {
      var aaa = from a in this.FieldDict
                where ( a.Value is ContinuedContentField
                        && a.Value.FieldNum == FieldNum)
                orderby ((ContentField)a.Value).FieldKey.SegNum
                select a.Value as ContinuedContentField;
      return aaa;
    }

    /// <summary>
    /// return a new ScreenContent object that is copy of this object.
    /// </summary>
    /// <returns></returns>
    public virtual ScreenContent Copy( ScreenContent ToParent = null )
    {
      var sc = new ScreenContent(ToParent, this);
      return sc;
    }

    /// <summary>
    /// return the ContentField with a textbyte that covers the specified rowcol
    /// location. 
    /// </summary>
    /// <param name="ix"></param>
    /// <returns></returns>
    public ContentField GetContentField( IRowCol RowCol )
    {
      ContentField getField = null;
      int ix = ToContentIndex(RowCol);

      // look prior to the location, looking for that closest field.
      while(ix > 0)
      {
        ix -= 1;
        var textByte = this.ContentArray[ix];
        if ( textByte.IsAttrByte( ))
        {
          ContentItemBase contentItem = null;
          var rowCol = ix.ToZeroRowCol(this.ScreenDim);
          var rc = this.FieldDict.TryGetValue(rowCol, out contentItem);

          // attr byte is start of a field.
          if ((rc == true) && (contentItem is ContentField))
          {
            var field = contentItem as ContentField;

            var fieldRange = new RowColRange(field.RowCol.AdvanceRight( ), field.LL_Length);
            if ( fieldRange.Contains(RowCol))
            {
              getField = field;
              break;
            }
          }
        }
      }

      return getField;
    }

    /// <summary>
    /// see 5494 functional ref. Read Screen command. page 15.13.  Return the 
    /// regeneration buffer of the screen. This is the raw contents of each byte on
    /// the screen.
    /// </summary>
    /// <returns></returns>
    public byte[] GetRegenerationBuffer( )
    {
      // make sure tailAttrByte of all the contentFields is set.
      foreach( var dictItem  in this.FieldDict)
      {
        if ( dictItem.Value is ContentField )
        {
          var contentField = dictItem.Value as ContentField;
          if (contentField.GetTailAttrByte(this) == null)
            contentField.SetTailAttrByte(this, 0x20);
        }
      }

      // byte size of the screen.
      int byteSx = this.ScreenDim.Height * this.ScreenDim.Width;
      var buf = this.GetContentBytes(new ZeroRowCol(0, 0), byteSx);

      return buf;
    }

    public IRowCol GetStartRowCol( )
    { return this.StartRowCol; }

    public ScreenContent GetWorkingContentBlock()
    {
      if (1 == 2)
      {
        var content = this.CurrentChildContent;
        if (content == null)
          return this;
        else
          return content;
      }

      if (this.CurrentChildIndex != null)
      {
        var ix = this.CurrentChildIndex.Value;
        return this.Children[ix];
      }
      else
        return this;
    }

    /// <summary>
    /// check that a row in the content space if all blanks.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <returns></returns>
    public bool RowIsBlank(int RowNum)
    {
      var rowcol = new ZeroRowCol(RowNum, 0);
      var buf = this.GetContentBytes(rowcol, this.ScreenDim.Width);
      var fx = buf.IndexOfNotEqual(0, EbcdicWhitespace);
      if (fx == -1)
        return true;
      else
        return false;
    }

    public int ToContentIndex(IRowCol RowCol, int Offset = 0)
    {
      if (RowCol.LocationFrame == LocationFrame.OneBased)
      {
        RowCol = RowCol.ToZeroRowCol();
      }
      var ix = RowCol.RowNum * this.ScreenDim.Width + RowCol.ColNum;
      ix += Offset;
      return ix;
    }

    public byte[] GetContentBytes( IRowCol RowCol, int Length)
    {
      var ix = ToContentIndex(RowCol);
      return this.ContentArray.SubArray(ix, Length);
    }

    public byte[] GetContentBytes_NonNull(IRowCol RowCol, int Length)
    {
      var jx = ToContentIndex(RowCol);
      var buf = this.ContentArray.SubArray(jx, Length);

      for( int ix = 0; ix < buf.Length; ++ix )
      {
        if (buf[ix] == 0x00)
          buf[ix] = 0x40;
      }
      return buf;
    }

    public int NextFieldNum( )
    {
      this.FieldNumOdom += 1;
      return this.FieldNumOdom;
    }

    public void SetContentBytes( IRowCol RowCol, byte Value)
    {
      var ix = ToContentIndex(RowCol);
      this.ContentArray[ix] = Value;
    }
    private void SetContentBytes(IRowCol RowCol, byte[] Value)
    {
      var ix = ToContentIndex(RowCol);
      Array.Copy(Value, 0, this.ContentArray, ix, Value.Length);
    }

    public IEnumerable<string> ToContentLines( )
    {
      var lineList = new List<string>();

      for( var rx = 0; rx < this.ScreenDim.Height; ++rx)
      {
        var sb = new StringBuilder();
        for( var colNum = 0; colNum < this.ScreenDim.Width; ++colNum)
        {
          var rowCol = new ZeroRowCol(rx, colNum, this.ScreenDim);
          var ix = ToContentIndex(rowCol);
          var b1 = this.ContentArray[ix];
          if (b1 == 0x24)
            sb.Append('#');
          else if ( b1.IsAttrByte( ))
            sb.Append('@');
          else
            sb.Append(b1.ToShowChar());
        }

        lineList.Add(sb.ToString());
      }

      return lineList;
    }

    public override string ToString()
    {
      return "ScreenContent. " + "Num fields:" + this.FieldDict.Count;
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ColumnReport(this.ReportTitle, 60);
      DefineReportColumns(report);

      // report.WriteColumnHeading();

      foreach (var item in this.ContentItems( ))
      {
        var valueList = new List<string>();
        valueList.Add(item.RowCol.RowNum.ToString());
        valueList.Add(item.RowCol.ColNum.ToString());
        valueList.Add(item.GetItemLength(this).ToString());
        valueList.Add(item.GetShowText(this));

        report.WriteDetail(valueList.ToArray());
      }

      return report.ToLines();
    }

    private static void DefineReportColumns(ColumnReport Report)
    {
      Report.AddColDefn("Row");
      Report.AddColDefn("Col");
      Report.AddColDefn("Length");
      Report.AddColDefn("Value text", 40);
    }

  }

  public static class ScreenContentExt
  {
    /// <summary>
    /// test that all elements of byte array are equal the MatchByte.
    /// </summary>
    /// <param name="Buf"></param>
    /// <param name="MatchByte"></param>
    /// <returns></returns>
    public static bool AllMatch( this byte[] Buf, byte MatchByte)
    {
      bool isMatch = true;
      for( int ix = 0; ix < Buf.Length; ++ix)
      {
        if ( Buf[ix] != MatchByte )
        {
          isMatch = false;
          break;
        }
      }
      return isMatch;
    }

    public static ContentItemBase GetContentItem(this ScreenContent Content, IScreenLoc loc)
    {
      ContentItemBase foundItem = null;
      foreach (var contentItem in Content.ContentItems())
      {
        var itemLoc = contentItem.RowCol as IScreenLoc;
        var range = new ScreenLocRange(
          itemLoc, contentItem.GetItemLength(Content), Content.ScreenDim);
        if (range.Contains(loc))
        {
          foundItem = contentItem;
          break;
        }
      }
      return foundItem;
    }

    public static bool IsAttrByte( this byte b1)
    {
      if ((b1 >= 0x20) && (b1 <= 0x3f))
        return true;
      else
        return false;
    }

    public static ZeroRowCol ToZeroRowCol(this int ix, ScreenDim Dim)
    {
      int colNum = 0;
      int rowNum = Math.DivRem(ix, Dim.Width, out colNum);
      var rowCol = new ZeroRowCol(rowNum, colNum, Dim);
      return rowCol;
    }
  }
}
