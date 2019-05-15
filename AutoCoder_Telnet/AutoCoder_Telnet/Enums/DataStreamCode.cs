using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  /// <summary>
  /// code that describes the purpose of the data stream and functions that
  /// follow.
  /// </summary>
  public enum DataStreamCode
  {
    Terminal,
    PrinterStartup,
    StartPrinterFile,
    PrinterPrint,
    PrinterResponse
  }

  public static class DataStreamCodeExt
  {
    static byte[] TerminalBytes = { 0x00, 0x00 };
    static byte[] PrinterStartupBytes = { 0x90, 0x00 };
    static byte[] PrinterPrintBytes = { 0x01, 0x01 };
    static byte[] PrinterResponseBytes = { 0x01, 0x02 };

    public static DataStreamCode? ToDataStreamCode(this byte[] CodeBytes )
    {
      DataStreamCode? code = null;

      if ( CodeBytes.Length == 2)
      {
        if (CodeBytes.CompareEqual(TerminalBytes) == true)
          code = DataStreamCode.Terminal;
        else if (CodeBytes.CompareEqual(PrinterStartupBytes) == true)
          code = DataStreamCode.PrinterStartup;
        else if (CodeBytes.CompareEqual(PrinterPrintBytes) == true)
          code = DataStreamCode.PrinterPrint;
        else if (CodeBytes.CompareEqual(PrinterResponseBytes) == true)
          code = DataStreamCode.PrinterResponse;
      }

      return code;
    }

    public static TypeTelnetDevice? ToTypeTelnetDevice(this DataStreamCode? DataStreamCode)
    {
      TypeTelnetDevice? typeDevice = null;
      if (DataStreamCode == null)
        typeDevice = null;
      else if (DataStreamCode.Value == Enums.DataStreamCode.Terminal)
        typeDevice = TypeTelnetDevice.Terminal;
      else if (DataStreamCode.Value == Enums.DataStreamCode.PrinterPrint)
        typeDevice = TypeTelnetDevice.Printer;
      else if (DataStreamCode.Value == Enums.DataStreamCode.PrinterStartup)
        typeDevice = TypeTelnetDevice.Printer;
      return typeDevice;
    }
  }
}
