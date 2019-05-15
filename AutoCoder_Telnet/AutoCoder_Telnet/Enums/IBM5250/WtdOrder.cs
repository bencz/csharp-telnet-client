using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.IBM5250
{
  public enum WtdOrder : byte
  {
    SetBufferAddress = 0x11,
    InsertCursor = 0x13,
    MoveCursor = 0x14,
    RepeatToAddress = 0x02,
    EraseToAddress = 0x03,
    StartHeader = 0x01,
    TransparentData = 0x10,
    WriteExtendedAttribute = 0x12,
    StartField = 0x1d,
    WriteStructuredField = 0x15
  }

  public static class WtdorderExt
  {
    public static WtdOrder? ToWtdOrder(this byte Value)
    {
      if (Value == 0x11)
        return WtdOrder.SetBufferAddress;
      else if (Value == 0x13)
        return WtdOrder.InsertCursor;
      else if (Value == 0x14)
        return WtdOrder.MoveCursor;
      else if (Value == 0x02)
        return WtdOrder.RepeatToAddress;
      else if (Value == 0x03)
        return WtdOrder.EraseToAddress;
      else if (Value == 0x01)
        return WtdOrder.StartHeader;
      else if (Value == 0x10)
        return WtdOrder.TransparentData;
      else if (Value == 0x12)
        return WtdOrder.WriteExtendedAttribute;
      else if (Value == 0x1d)
        return WtdOrder.StartField;
      else if (Value == 0x15)
        return WtdOrder.WriteStructuredField;
      else
        return null;
    }

    public static string ToItemName(this WtdOrder OrderCode)
    {
      if (OrderCode == WtdOrder.SetBufferAddress)
        return "SetBufferAddress";
      else if (OrderCode == WtdOrder.InsertCursor)
        return "InsertCursor";
      else if (OrderCode == WtdOrder.MoveCursor)
        return "MoveCursor";
      else if (OrderCode == WtdOrder.RepeatToAddress)
        return "RepeatToAddress";
      else if (OrderCode == WtdOrder.EraseToAddress)
        return "EraseToAddress";
      else if (OrderCode == WtdOrder.StartHeader)
        return "StartHeader";
      else if (OrderCode == WtdOrder.TransparentData)
        return "TransparentData";
      else if (OrderCode == WtdOrder.WriteExtendedAttribute)
        return "WriteExtendedAttribute";
      else if (OrderCode == WtdOrder.StartField)
        return "StartField";
      else if (OrderCode == WtdOrder.WriteStructuredField)
        return "WriteStructuredField";
      else
        return "unexpected WTD Order";
    }
  }
}
