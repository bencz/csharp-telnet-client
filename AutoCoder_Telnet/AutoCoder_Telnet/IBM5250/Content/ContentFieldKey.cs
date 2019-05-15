using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContentFieldKey
  {
    public int FieldNum
    { get; set; }
    public int SegNum
    { get; set; }

    public ContentFieldKey( int FieldNum, int SegNum)
    {
      this.FieldNum = FieldNum;
      this.SegNum = SegNum;
    }
    public override bool Equals(object obj)
    {
      if (obj is ContentFieldKey)
      {
        var objKey = obj as ContentFieldKey;
        if ((objKey.FieldNum == this.FieldNum) && (objKey.SegNum == this.SegNum))
          return true;
      }
      return false;
    }
    public override int GetHashCode()
    {
      var hashCode = (this.FieldNum * 10000) + this.SegNum;
      return hashCode;
    }

    public static ContentFieldKey AssignFieldKey(ScreenContent ScreenContent, ContentField Field)
    {
      ContinuedFieldSegmentCode? segCode = null;

      if (Field is ContinuedContentField)
      {
        var contField = Field as ContinuedContentField;
        segCode = contField.ContinuedFieldSegmentCode;
      }

      ContentFieldKey fieldKey = ScreenContent.AssignFieldKey(segCode);

      return fieldKey;
    }

#if skip
    private static int AssignFieldNum(ScreenContent ScreenContent, ContentField Field)
    {
      var fieldNum = 0;
      ContinuedFieldSegmentCode? segCode = null;
      if (Field is ContinuedContentField)
      {
        var contField = Field as ContinuedContentField;
        segCode = contField.ContinuedFieldSegmentCode;
      }

      if ((segCode == null) || (segCode.Value == ContinuedFieldSegmentCode.First))
      {
        fieldNum = ScreenContent.NextFieldNum();
      }
      else
        fieldNum = ScreenContent.AssignSegNum();

      return fieldNum;
    }
#endif

  }
}
