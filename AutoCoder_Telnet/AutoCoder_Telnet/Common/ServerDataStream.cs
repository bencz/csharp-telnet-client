using AutoCoder.Ext.System;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using AutoCoder.Systm;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250;
using System.Collections.Generic;
using AutoCoder.Telnet.SCS.ControlFunctions;

namespace AutoCoder.Telnet.Common
{
  /// <summary>
  /// static methods which classify and process telnet data stream data received 
  /// from the server.
  /// </summary>
  public static class ServerDataStream
  {

    /// <summary>
    /// Parse the byte array. Return the WorkstationCommandList and ResponseItemList
    /// that the byte stream contains.
    /// </summary>
    /// <param name="ByteArray"></param>
    /// <param name="DataStreamName"></param>
    /// <returns></returns>
    public static Tuple<WorkstationCommandList, ResponseItemList,
      DataStreamHeader, TelnetCommandList, ControlFunctionList, byte[]>
      ParseByteArray(byte[] ByteArray, int ArrayLx = -1, string DataStreamName = "")
    {
      if (ArrayLx == -1)
        ArrayLx = ByteArray.Length;

      var inputArray = new InputByteArray(ByteArray, ArrayLx);
      var sessionSettings = new SessionSettings();

      var rv =
        ServerDataStream.ParseDataStream(inputArray, sessionSettings, 
        DataStreamName);
      var wrkstnCmdList = rv.Item1;
      var responseItemList = rv.Item2;
      var dsh = rv.Item3;
      var telList = rv.Item4;
      var funcList = rv.Item5;

      byte[] remainingBytes = null;
      if (inputArray.RemainingLength > 0)
      {
        remainingBytes = inputArray.PeekBytes();
        var report = remainingBytes.ToHexReport(16);
      }

      return new Tuple<WorkstationCommandList, ResponseItemList,
        DataStreamHeader, TelnetCommandList, ControlFunctionList, byte[]>(
        wrkstnCmdList, responseItemList, dsh, telList, funcList, remainingBytes);
    }

    /// <summary>
    /// parse the InputArray byte data stream. Return the WorkstationCommandList and
    /// ResponseItemList that the byte stream contains.
    /// </summary>
    /// <param name="inputArray"></param>
    /// <param name="sessionSettings"></param>
    /// <param name="DataStreamName"></param>
    /// <returns></returns>
    public static Tuple<WorkstationCommandList, ResponseItemList,
      DataStreamHeader, TelnetCommandList,ControlFunctionList>
      ParseDataStream(
      InputByteArray inputArray, SessionSettings sessionSettings,
      string DataStreamName, TypeTelnetDevice? TypeDevice = null)
    {
      var accumCmdList = new WorkstationCommandList();
      var responseItemList = new ResponseItemList();
      TelnetCommandList telList = null;
      ResponseHeader curRespHeader = null;
      ResponseHeader nextRespHeader = null;
      DataStreamHeader dsh = null;
      ControlFunctionList funcList = null;

      bool didParse = true;
      while ((didParse == true) && (inputArray.RemainingLength > 0))
      {
        didParse = false;
        curRespHeader = nextRespHeader;
        nextRespHeader = null;

        if ((didParse == false) && (dsh == null))
        {
          var telCmd = inputArray.NextTelnetCommand();
          if (telCmd != null)
          {
            if (telList == null)
              telList = new TelnetCommandList();
            telList.Add(telCmd);
            var s1 = telCmd.ToString();
            didParse = true;
          }
        }

        // parse the data stream header.
        if ((didParse == false) && (dsh == null))
        {
          var code = inputArray.PeekDataStreamCode();
          if (code != null)
          {
            var rv = DataStreamHeader.Factory(inputArray);
            dsh = rv.Item1;
            didParse = true;

            // update the type of device depending on data stream header stream
            // code.
            if ((TypeDevice == null) && (dsh != null) && (dsh.Errmsg == null))
            {
              TypeDevice = dsh.StreamCode.ToTypeTelnetDevice();
            }
          }
        }

        // parse as a sequence of workstation commands. 
        if (didParse == false)
        {
          if ((TypeDevice == null) || (TypeDevice.Value != TypeTelnetDevice.Printer))
          {
            var rv = Process5250.ParseWorkstationCommandList(
            inputArray, sessionSettings);

            var anotherDsh = rv.Item1;
            var wrkstnCmdList = rv.Item2;

            if ((anotherDsh != null) && (anotherDsh.Errmsg == null))
            {
              didParse = true;
              dsh = anotherDsh;
            }

            // draw the fields and literals on the canvas.
            if (wrkstnCmdList?.Count > 0)
            {
              accumCmdList.Append(wrkstnCmdList);
              didParse = true;
            }
          }
        }

        // parse as sequence of SCS control functions. ( printer data stream ).
        if (didParse == false)
        {
          if ((TypeDevice == null) || (TypeDevice.Value == TypeTelnetDevice.Printer))
          {
            var rv = ControlFunctionList.ParseDataStream(inputArray);
            if (rv.Item1?.Count > 0)
            {
              didParse = true;
              funcList = rv.Item1;
            }
          }
        }

        // parse as response stream header.
        if ((didParse == false ) 
          && (ResponseHeader.IsResponseHeader(inputArray).Item1 == true))
        {
          curRespHeader = new ResponseHeader(inputArray);
          didParse = true;

          responseItemList.Add(curRespHeader);
          var rv = Response5250.ParseResponseStream(inputArray, curRespHeader);
          responseItemList.Append(rv.Item1);
        }
      }

      return new Tuple<WorkstationCommandList, ResponseItemList,
        DataStreamHeader, TelnetCommandList,ControlFunctionList>(
        accumCmdList, responseItemList, dsh, telList, funcList);
    }

    /// <summary>
    /// look at the current bytes of the input array.  Determine what type of data 
    /// stream command is starting.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static TypeServerData? PeekServerCommand(
      InputByteArray InputArray)
    {
      TypeServerData? typeData = null;

      if (InputArray.IsEof() == true)
      {
        typeData = TypeServerData.eof;
      }
      else if (InputArray.PeekTelnetCommandCode() != null)
      {
        typeData = TypeServerData.telnetCommand;
      }

      else if (InputArray.IsDataStreamHeader() == true)
      {
        typeData = TypeServerData.workstationHeader;
      }

      return typeData;
    }

    public static IEnumerable<string> ParseAndReportByteArray(
      byte[] ByteArray, string DataStreamName)
    {
      var rv = ServerDataStream.ParseByteArray(ByteArray, -1, DataStreamName);
      var wrkstnCmdList = rv.Item1;
      var responseList = rv.Item2;
      var dsHeader = rv.Item3;
      var telList = rv.Item4;

      var report = ReportParseResults(dsHeader, wrkstnCmdList, responseList, telList);

      return report;
    }

    private static IEnumerable<string> ReportParseResults(
      DataStreamHeader dsHeader, WorkstationCommandList wrkstnCmdList,
      ResponseItemList responseList, TelnetCommandList telList)
    {
      var report = new List<string>();

      if (dsHeader != null)
        report.AddRange(dsHeader.ToColumnReport());

      if (wrkstnCmdList != null)
      {
        report.AddRange(wrkstnCmdList.ToColumnReport());
      }

      if (responseList != null)
      {
        report.AddRange(responseList.ReportResponseItems());
      }

      if (telList != null)
      {
        report.AddRange(telList.ToColumnReport());
      }

      return report;
    }
  }
}
