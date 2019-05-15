using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.SCS
{
  public enum ControlFunctionCode
  {
    Backspace = 0x16,
    Bell = 0x2f,
    CarriageReturn = 0x0d,
    CustomerUse1 = 0x1b,
    CustomerUse3 = 0x3b,
    EnablePresentation = 0x14,
    FormFeed = 0x0c,
    HorizontalTab = 0x05,
    LineFeed = 0x25,
    NewLine = 0x15,
    Null = 0x00,
    PresentationPosition = 0x34,
    SetPrint = 0x91,
    SetAttr,
    SetGraphicError,   // 2BC8
    SetTranslation,    // 2BD1
    SetChain,          // 2BD2
    SetUndocumented,    // undocumented code 2BD3 
    Text,               // printable text
    Undocumented1 = 0xc0,   // undocumented control code. followed by byte param.
    Undocumented2 = 0x50,   // undocumented control code. no parm.
    Undocumented3 = 0xe0
  }

  public static class ControlFunctionCodeExt
  {
    static ControlFunctionCode[] FunctionCodeArray = {ControlFunctionCode.Backspace,
    ControlFunctionCode.Bell, ControlFunctionCode.CarriageReturn,
      ControlFunctionCode.CustomerUse1,
    ControlFunctionCode.CustomerUse3, ControlFunctionCode.EnablePresentation,
    ControlFunctionCode.FormFeed, ControlFunctionCode.HorizontalTab,
    ControlFunctionCode.LineFeed, ControlFunctionCode.NewLine,
    ControlFunctionCode.Null, ControlFunctionCode.PresentationPosition,
    ControlFunctionCode.Undocumented1, ControlFunctionCode.Undocumented2,
    ControlFunctionCode.Undocumented3};

    static byte[] FunctionByteArray = {0x16,
    0x2f, 0x0d, 0x1b,
    0x3b, 0x14,
    0x0c, 0x05,
    0x25, 0x15,
    0x00, 0x34,
    0xc0, 0x50, 0xe0 };
    static byte[] SetPrintBytes = { 0x2b, 0xd2 };
    static byte[] SetUndocumentedBytes = { 0x2b, 0xd3 };
    static byte[] SetTranslationBytes = { 0x2b, 0xd1 };
    static byte[] SetGraphicErrorBytes = { 0x2b, 0xc8 };

    // the set of byte codes that represent SCS printable text characters.
    public static byte[] TextBytes = {
      0x40, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
      0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
      0x60, 0x61, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
      0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
      0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
      0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99,
      0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9,
      0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9,
      0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9,
      0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9,
      0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9
    };


    public static Tuple<ControlFunctionCode?,int> ToControlFunctionCode( this byte[] Input)
    {
      ControlFunctionCode? code = null;
      int length = 0;
      var b1 = Input[0];

      {
        var fx = Array.IndexOf(FunctionByteArray, b1);
        if (fx >= 0)
        {
          code = FunctionCodeArray[fx];
          length = 1;
        }
      }

      // check that byte is a printable text character.
      if ( code == null)
      {
        var fx = Array.IndexOf(TextBytes, b1);
        if ( fx >= 0)
        {
          code = ControlFunctionCode.Text;
          length = 1;
        }
      }

      if (( code == null) && (Input.Length >= 2))
      {
        if (Input.SubArray(0,2).CompareEqual(SetPrintBytes) == true )
        {
          code = ControlFunctionCode.SetPrint;
          length = 2;
        }
        else if (Input.SubArray(0, 2).CompareEqual(SetUndocumentedBytes) == true)
        {
          code = ControlFunctionCode.SetUndocumented;
          length = 2;
        }
        else if (Input.SubArray(0, 2).CompareEqual(SetTranslationBytes) == true)
        {
          code = ControlFunctionCode.SetTranslation;
          length = 2;
        }
        else if (Input.SubArray(0, 2).CompareEqual(SetGraphicErrorBytes) == true)
        {
          code = ControlFunctionCode.SetGraphicError;
          length = 2;
        }
      }

      return new Tuple<ControlFunctionCode?, int>(code, length);
    }
  }
}
