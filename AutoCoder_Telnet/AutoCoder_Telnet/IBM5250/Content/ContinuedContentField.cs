using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.CodedBytes;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContinuedContentField : ContentField
  {
    public ContinuedFieldSegmentCode ContinuedFieldSegmentCode
    {
      get
      {
        return this.FCW_Bytes.ToContinuedFieldSegmentCode().Value;
      }
    }

    public int SegmentNum
    { get; set; }

    public ContinuedContentField(ZeroRowCol RowCol, StartFieldOrder FieldOrder) 
      : base(RowCol, FieldOrder)
    {

    }

    public override string GetFieldShowText(ScreenContent ScreenContent)
    {
      var sb = new StringBuilder();
      foreach (var contFld in ScreenContent.ContinuedFieldSegments(this.FieldKey.FieldNum))
      {
        sb.Append(contFld.GetShowText(ScreenContent));
      }
      return sb.ToString();
    }

    /// <summary>
    /// return the modified flag of the continued entry field. Examine each of the
    /// segment fields.
    /// </summary>
    /// <param name="ScreenContent"></param>
    /// <returns></returns>
    public override bool GetModifiedFlag(ScreenContent ScreenContent)
    {
      bool isModified = false;
      foreach (var contFld in ScreenContent.ContinuedFieldSegments(this.FieldKey.FieldNum))
      {
        if (contFld.ModifiedFlag == true)
        {
          isModified = true;
          break;
        }
      }
      return isModified;
    }
    public override string ToString()
    {
      string s1 = "ContentField. " + this.RowCol.ToString() +
        " Length:" + this.LL_Length;
      return s1;
    }

  }
}
