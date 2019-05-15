using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCanvasLib.Canvas;
using TextCanvasLib.ContentExt;
using TextCanvasLib.ThreadMessages;
using AutoCoder.Telnet.SCS.ControlFunctions;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using AutoCoder.Report;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums.SCS;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Systm;
using AutoCoder.Telnet.LogFiles;

namespace TextCanvasLib.Threads
{
  /// <summary>
  /// when device type is Printer, this PrinterThread receives input from the
  /// FromThread. Otherwise input is sent to the MasterThread.
  /// </summary>
  public class PrinterThread : ThreadBase, IThreadBase
  {
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }
    public ToThread ToThread
    { get; set; }

    public PaintThread PaintThread
    { get; set; }
    public IThreadBase MatchThread
    { get; set; }

    private ExtendedManualResetEvent ConnectionFailedEvent
    { get; set; }

    public ConcurrentOdom ContentOdom
    {
      get; set;
    }

    private OpenPdfDocument OpenDoc
    { get; set; }

    /// <summary>
    /// bytes remaining from the prior printer data stream.
    /// </summary>
    private byte[] RemainingBytes
    { get; set; }


    public PrinterThread(
      ExtendedManualResetEvent ShutdownFlag, ExtendedManualResetEvent ConnectionFailedEvent)
      : base(ShutdownFlag)
    {
      this.ContentOdom = new ConcurrentOdom();
      this.InputQueue = new ConcurrentMessageQueue();
      this.ConnectionFailedEvent = ConnectionFailedEvent;
    }

    public void EntryPoint()
    {
      this.ThreadEndedEvent.Reset();

      try
      {
        // loop receiving from the server until:
        //   - the foreground thread wants to shutdown the connection. It has set
        //     the ShutdownFlag event.
        while ((ShutdownFlag.State == false)
          && (this.ConnectionFailedEvent.State == false))
        {
          var message = InputQueue.WaitAndDequeue(
            this.ShutdownFlag.EventObject, this.ConnectionFailedEvent.EventObject);
          if (message != null)
          {
            if (message is DataStreamHeaderMessage)
            {
              var dshMessage = message as DataStreamHeaderMessage;
              var dataBytes = new byte[] {
                  0x00, 0x0A, 0x12, 0xA0, 0x01, 0x02,
                  0x04, 0x00, 0x00, 0x01, 0xFF, 0xEF
                };
              var dataMessage = new SendDataMessage(dataBytes);
              this.ToThread.PostInputMessage(dataMessage);
            }

            else if (message is DataStreamHeader)
            {
              var dsh = message as DataStreamHeader;
              var dataBytes = new byte[] {
                  0x00, 0x0A, 0x12, 0xA0, 0x01, 0x02,
                  0x04, 0x00, 0x00, 0x01, 0xFF, 0xEF
                };
              var dataMessage = new SendDataMessage(dataBytes);
              this.ToThread.PostInputMessage(dataMessage);
            }

            else if ( message is PrinterDataBytesMessage)
            {
              var dataMessage = message as PrinterDataBytesMessage;
              var dataBytes = dataMessage.DataBytes;
              var dataBytesLength = dataBytes.Length;

              // remove the IAC EOR from the end of the stream.
              // ( the data stream may end with a partial control function which
              //   is continued in the next data stream. Do not want to 
              //   confuse the FF EF EOR bytes as data bytes of the possible
              //   incomplete control function. )
              {
                var endBytes = dataBytes.Tail(2);
                var cmdCode = endBytes.ParseTelnetCommandCode();
                if ((cmdCode != null) && (cmdCode.Value == CommandCode.EOR))
                {
                  dataBytesLength -= 2;
//                  dataBytes = dataBytes.SubArray(0, dataBytesLength);
                }
              }

              // there are remaining bytes from the previous dataBytes message.
              // insert these remaining bytes after the dataStreamHeader.
              if (this.RemainingBytes != null)
              {
                dataBytes = dataBytes.SubArray(0, dataBytesLength);
                var headerLength = dataBytes.GetDataStreamHeaderLength();
                if (headerLength != null)
                {
                  dataBytes = 
                    dataBytes.Insert(headerLength.Value, this.RemainingBytes);
                  dataBytesLength += this.RemainingBytes.Length;
                }
              }

              // parse the bytes.
              var rv = ServerDataStream.ParseByteArray(dataBytes, dataBytesLength );
              var wrkstnCmdList = rv.Item1;
              var responseList = rv.Item2;
              var dsh = rv.Item3;
              var telList = rv.Item4;
              var funcList = rv.Item5;

              // bytes at the end of the data stream that were not recognized as 
              // complete SCS function codes.  Save now and add to the front of the
              // next data stream that arrives. ( add to front after the data 
              // stream header. )
              this.RemainingBytes = rv.Item6;

              if ( 1 == 1 )
              {
                this.OpenDoc = PrintToPdf(dsh, funcList, this.OpenDoc);
              }

              var respBytes = new byte[] {
                  0x00, 0x0A, 0x12, 0xA0, 0x01, 0x02,
                  0x04, 0x00, 0x00, 0x01, 0xFF, 0xEF
                };
              var respMessage = new SendDataMessage(respBytes);
              this.ToThread.PostInputMessage(respMessage);
            }

            else if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              switch (generalMessage.MessageCode)
              {
                case ThreadMessageCode.ClearLog:
                  {
                    break;
                  }

              }
            }

            else if (message is ExchangeMessage)
            {
              var exchangeMessage = message as ExchangeMessage;
              if (exchangeMessage.MessageCode == ThreadMessageCode.GetScreenContent)
              {
              }
            }
          }
        }
      }
      finally
      {
        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();
      }
    }

    private OpenPdfDocument PrintToPdf(
      DataStreamHeader dsh, ControlFunctionList funcList,
      OpenPdfDocument OpenDoc = null)
    {
      OpenPdfDocument openDoc = OpenDoc;

      if ( dsh is StartPrinterFileDataStreamHeader)
      {
        var docPath = "c:\\downloads\\printout.pdf";
        openDoc = new OpenPdfDocument(docPath);
      }

      else if ( dsh is PrinterDataStreamHeader)
      {

        if ( openDoc == null)
        {
          var docPath = "c:\\downloads\\printout.pdf";
          openDoc = new OpenPdfDocument(docPath);
        }

        if (funcList != null)
        {
          foreach (var func in funcList)
          {
            if (func.ControlCode == ControlFunctionCode.CarriageReturn)
              openDoc.ApplyCarriageReturn();
            else if (func.ControlCode == ControlFunctionCode.NewLine)
              openDoc.ApplyNewLine();
            else if (func.ControlCode == ControlFunctionCode.PresentationPosition)
            {
              var presentPos = func as PresentationPositionControlFunction;
              if (presentPos.Direction == PresentationPositionDirection.Horizontal)
                openDoc.ApplyPos(presentPos.PositionValue - 1);
            }
            else if (func.ControlCode == ControlFunctionCode.Text)
            {
              var textFunc = func as TextControlFunction;
              openDoc.ApplyText(textFunc.TextBytes.EbcdicBytesToString());
            }

            else if ((func.ControlCode == ControlFunctionCode.Undocumented2)
              || (func.ControlCode == ControlFunctionCode.Undocumented3))
            {
              openDoc.ApplyText(" ");
            }

            else if (func.ControlCode == ControlFunctionCode.FormFeed)
              openDoc.ApplyNewPage();
          }

          if (funcList.IsSingleNullList() == true)
          {
            openDoc.CloseDoc();
            openDoc = null;
          }
        }
      }
      return openDoc;
    }

  }

  public class OpenPdfDocument
  {
    public Document Document
    { get; set; }

    public string DocPath
    { get; set;  }

    public FileStream FileStream
    { get; set;  }

    public PdfWriter Writer
    { get; set;  }

    public BlankFillLineBuilder CurrentLine
    { get; set; }

    public int CurrentPos
    { get; set; }
    public OpenPdfDocument( string DocPath )
    {
      this.DocPath = DocPath;
      this.Document = new Document(PageSize.LETTER.Rotate(), 10, 10, 42, 30);
      this.FileStream = new FileStream(this.DocPath, FileMode.Create);
      this.Writer = PdfWriter.GetInstance(this.Document, this.FileStream);
      this.Document.Open();

      ClearCurrentLine();
    }

    public void ApplyText(string Text)
    {
      // BgnTemp
      int lx = Text.Length;
      var s1 = this.CurrentLine.ToString();
      int lb = s1.Length;
      // EndTemp

      CurrentLine.Put(Text, this.CurrentPos);
      this.CurrentPos += Text.Length;
    }

    public void ApplyPos( int Pos )
    {
      this.CurrentPos = Pos;
    }

    public void ApplyCarriageReturn( )
    {
      this.CurrentPos = 0;
    }

    public void ApplyNewLine()
    {
      if (this.CurrentLine.IsEmpty() == false)
      {
        var font = FontFactory.GetFont("courier", 8.2f, BaseColor.BLACK);
        var line = this.CurrentLine.ToString().TrimEndWhitespace();
        var para = new iTextSharp.text.Paragraph(line, font);
        para.SetLeading(0, 1.15f);
        this.Document.Add(para);
      }

      ClearCurrentLine();
    }

    public void ApplyNewPage( )
    {
      ApplyNewLine();
      this.Document.NewPage();
      ClearCurrentLine();
    }

    private void ClearCurrentLine( )
    {
      if (this.CurrentLine == null)
        this.CurrentLine = new BlankFillLineBuilder();
      this.CurrentPos = 0;
      this.CurrentLine.Empty();
    }

    public void CloseDoc( )
    {
      ApplyNewLine();
      this.Document.Close();
      this.Document = null;
    }

    private void SimpleFontDoc(string pdfDocPath)
    {
      Document doc = new Document(PageSize.LETTER, 10, 10, 42, 30);
      var fs = new FileStream(pdfDocPath, FileMode.Create);
      PdfWriter writer = PdfWriter.GetInstance(doc, fs);
      doc.Open();

      string[] lines = new string[]
        {
            "First   text   line",
            "Second  text   line"
        };
      var font = FontFactory.GetFont("courier", 12.0f, BaseColor.BLACK);

      foreach (var line in lines)
      {
        var para = new iTextSharp.text.Paragraph(line, font);
        doc.Add(para);
      }

      doc.Close();
    }

  }
}
