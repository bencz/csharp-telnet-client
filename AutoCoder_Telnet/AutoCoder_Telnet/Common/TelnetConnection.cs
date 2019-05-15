using AutoCoder.Core;
using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Text;
using AutoCoder.Systm;
using AutoCoder.Telnet.Cipher;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Exceptions;
using AutoCoder.Text;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.Threads;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Core.Interfaces;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.ThreadMessages;

namespace AutoCoder.Telnet.Common
{
  public class TelnetConnection
  {
    TcpClient mSock;

    // see Write method.  Sometimes the Read thread has to write back responses
    // to IAC commands sent by the telnet server. Don't want these writes to mix
    // with the commands that the Send thread writes to the server. 
    // The Write method locks the mWriteLock object so that each threads writes
    // don't interfere with each other.
    object mWriteLock = new object();

    // method called whenever data is received from telnet server.
    // Used for debugging purposes.
    delWriteEventLog mReceiveEventLogCallback;

    // event that is signaled whenever the connection is not active
    ExtendedManualResetEvent mIsDisconnectedSignal =
      new ExtendedManualResetEvent(true);

    public TelnetConnection(string InHostname, int InPort)
    {
      mSock = new TcpClient(InHostname, InPort);

      // socket is now ready to send and receive. Clear the disconnected signal.
      IsDisconnectedSignal.Reset();
    }
    /// <summary>
    /// read and process the inital telnet data stream from the server. Perform
    /// the back and forth telnet negotiation steps until a non telnet command is
    /// received.
    /// </summary>
    /// <param name="Host"></param>
    /// <param name="NegotiateSettings"></param>
    /// <param name="Client"></param>
    /// <param name="NetStream"></param>
    /// <returns></returns>
    public static Tuple<SessionSettings, TelnetLogList>
      TelnetConnectAndNegotiate(
      string Host, NegotiateSettings NegotiateSettings,
      ConcurrentMessageQueue TelnetQueue, ToThread ToThread)
    {
      var sessionSettings = new SessionSettings();
      TelnetLogList logList = null;
      bool breakLoop = false;

      // loop reading from NetworkStream and processing the telnet command.
      // loop until break flag is set.
      while (breakLoop == false)
      {
        var item = TelnetQueue.WaitAndPeek();
        if ((item is TelnetCommand) == false)
          break;
        var telCmd = TelnetQueue.WaitAndDequeue() as TelnetCommand;

        byte[] responseBytes = null;
        {
          var rv = ProcessTelnetCommand(telCmd, NegotiateSettings);
          var cx = rv.Item1;
          responseBytes = rv.Item2;
        }

        if ((responseBytes != null) && (responseBytes.Length > 0))
        {
          var dataMessage = new SendDataMessage(responseBytes);
          ToThread.PostInputMessage(dataMessage);
        }
      }
      return new Tuple<SessionSettings, TelnetLogList>(
        sessionSettings, logList);
    }

    /// <summary>
    /// read and process the inital telnet data stream from the server. Perform
    /// the back and forth telnet negotiation steps until a non telnet command is
    /// received.
    /// </summary>
    /// <param name="Host"></param>
    /// <param name="NegotiateSettings"></param>
    /// <param name="Client"></param>
    /// <param name="NetStream"></param>
    /// <returns></returns>
    public static Tuple<SessionSettings, TelnetLogList>
      TelnetPrinterConnectAndNegotiate(
      string Host, NegotiateSettings NegotiateSettings, 
      ConcurrentMessageQueue TelnetQueue,
      ServerConnectPack ConnectPack, bool doLog = false)
    {
      var sessionSettings = new SessionSettings();
      TelnetLogList logList = null;

      // BgnTemp
      doLog = true;
      // EndTemp

      if (doLog == true)
        logList = new TelnetLogList();
      bool breakLoop = false;

      // loop reading from NetworkStream and processing the telnet command.
      // loop until break flag is set.
      while (breakLoop == false)
      {
        var item = TelnetQueue.WaitAndPeek();
        if ((item is TelnetCommand) == false)
          break;
        var telCmd = TelnetQueue.WaitAndDequeue() as TelnetCommand;

        byte[] responseBytes = null;
        {
          var rv = ProcessTelnetCommand(telCmd, NegotiateSettings, doLog);
          var cx = rv.Item1;
          responseBytes = rv.Item2;
          if (doLog == true)
            logList.AddItems(rv.Item3);
        }

        if ((responseBytes != null) && (responseBytes.Length > 0))
          WriteToHost(logList, responseBytes, ConnectPack.TcpClient.GetStream());
      }
      return new Tuple<SessionSettings, TelnetLogList>(
        sessionSettings, logList);
    }

    public void Disconnect()
    {
      if (IsDisconnectedSignal.State == false)
      {
        try
        {
          mSock.Client.Disconnect(false);
        }
        catch (SocketException)
        {
        }
        IsDisconnectedSignal.Set();
      }
    }

    public bool IsConnected
    {
      get { return mSock.Connected; }
    }

    public ExtendedManualResetEvent IsDisconnectedSignal
    {
      get { return mIsDisconnectedSignal; }
    }

    private static int ProcessNewEnvironStatement(
      NewEnvironCommand Stmt, ByteArrayBuilder WriteStream,
      NegotiateSettings NegotiateSettings)
    {
      int sendStmtCx = 0;

      // got the stmt with the SEND parm. Send back the NEW_ENVIRON stmt with the IS parm.
      if ((Stmt.SubOption != null)
        && (Stmt.SubOption.Value == TelnetOptionParm.SEND))
      {
        var neSend =
          new NewEnvironCommand(Enums.CommandCode.SB, TelnetOptionParm.IS);

        foreach (var newEnv in NegotiateSettings.SendNewEnvironList)
        {
          var optnVar = newEnv.ToOptionVariable(NegotiateSettings.SbsValuesDict);
          neSend.AddOptionVar(optnVar);
        }

        int xx = 25;
        if (xx == 26)
        {
          neSend.AddOptionVar(EnvironVarCode.VAR, "USER", "SRICHTER");
          neSend.AddOptionVar(EnvironVarCode.VAR, "DEVNAME", "STEVE25");

          // reply to newEnviron IBMRSEED stmt SEND statement.
          if (Stmt.ContainsUserVar_IBMRSEED() == true)
          {
            byte[] clientSeed = new byte[8] { 0x55, 0x56, 0x59, 0x5a, 0x5b, 0x5c, 0x4c, 0x48 };
            string passWord = "denville";
            string userName = "srichter";
            var serverSeed = Stmt.IBMRSEED_SeedValue();
            var substitutionPassword =
              CipherCommon.EncryptPassword(passWord, userName, serverSeed, clientSeed);
            neSend.AddOptionVar(EnvironVarCode.USERVAR, "IBMRSEED", clientSeed);
            neSend.AddOptionVar(EnvironVarCode.USERVAR, "IBMSUBSPW", substitutionPassword);
          }
        }

        WriteStream.Append(neSend.ToBytes());

        // send the SE command immed after NEW-ENVIRON
        {
          var ts = new TelnetCommand(Enums.CommandCode.SE);
          WriteStream.Append(ts.ToBytes());
        }
      }
      return sendStmtCx;
    }

    public static Tuple<int, byte[], TelnetLogList> ProcessTelnetCommand(
      TelnetCommand TelnetCmd, NegotiateSettings NegotiateSettings,
      bool doLog = false)
    {
      TelnetLogList logList = null;
      if (doLog == true)
        logList = new TelnetLogList();
      int cx = 0;
      var writeStream = new ByteArrayBuilder();

      // write to the run log.
      if (logList != null)
        logList.AddItem(Direction.Read, TelnetCmd.ToString());

      if (TelnetCmd.CmdCode == Enums.CommandCode.DO)
      {
        var replyStmt = TelnetCmd.BuildReply(NegotiateSettings);
        writeStream.Append(replyStmt.ToBytes());
      }

      else if (TelnetCmd is NewEnvironCommand)
      {
        var envStmt = TelnetCmd as NewEnvironCommand;
        cx = ProcessNewEnvironStatement(envStmt, writeStream, NegotiateSettings);
      }

      else if (TelnetCmd is TerminalTypeCommand)
      {
        var ttStmt = TelnetCmd as TerminalTypeCommand;
        cx = ProcessTerminalTypeStatement(ttStmt, writeStream, NegotiateSettings);
      }

      else if ((TelnetCmd is EndOfRecordStatement) || (TelnetCmd is TransmitBinaryCommand))
      {
        if (TelnetCmd.CmdCode == Enums.CommandCode.DO)
        {
          var replyStmt = TelnetCmd.BuildReply(NegotiateSettings);
          writeStream.Append(replyStmt.ToBytes());

          if ((replyStmt.CmdCode == Enums.CommandCode.DO)
            && (replyStmt.Subject.IsEqual(TelnetSubject.TRANSMIT_BINARY)))
          {

          }
        }
        else if (TelnetCmd.CmdCode == Enums.CommandCode.WILL)
        {
          var replyStmt = TelnetCmd.BuildDoReply();
          writeStream.Append(replyStmt.ToBytes());
        }
      }
      return new Tuple<int, byte[], TelnetLogList>(cx, writeStream.ToByteArray(), logList);
    }

    private static int ProcessTerminalTypeStatement(
      TerminalTypeCommand Stmt, ByteArrayBuilder WriteStream,
      NegotiateSettings NegotiateSettings)
    {
      int sendStmtCx = 0;

      if ((Stmt.SubOption != null)
        && (Stmt.SubOption.Value == TelnetOptionParm.SEND))
      {
        var ttSend =
          new TerminalTypeCommand(Enums.CommandCode.SB, TelnetOptionParm.IS);
        ttSend.AssignTerminalName(NegotiateSettings.TerminalType);
        ttSend.GotClosingSE = true;
        WriteStream.Append(ttSend.ToBytes());
      }
      return sendStmtCx;
    }

    /// <summary>
    /// Read characters as is from the telnet server. Return whatever is
    /// received, whether it is complete or not. 
    /// </summary>
    /// <returns></returns>
    public string Read()
    {
      ThrowNotConnected();

      StringBuilder sb = ReadIntoBuffer();

      return sb.ToString();
    }

    public string Read(int InTimeOutMs)
    {
      if (mSock.Connected == false)
      {
        return null;
      }

      StringBuilder sb = ReadIntoBuffer();

      //      do
      //      {
      //        ReadIntoBuffer(sb);
      //        ParseTelnet(sb);
      //        System.Threading.Thread.Sleep(InTimeOutMs);
      //      }
      //      while (mSock.Available > 0);

      return sb.ToString();
    }

    void Read_LogTelnetCommandByte(byte InCmdByte, string InDesc)
    {
      var sb = new StringBuilder();
      sb.Append(InDesc);

      string hexEf = InCmdByte.ToString("X").PadLeft(2, '0');
      sb.SentenceAppend(hexEf);

      var cmdCode = InCmdByte.ToTelnetCommandCode();
      if (cmdCode != null)
      {
        sb.SentenceAppend("(" + cmdCode.Value.ToString() + ")");
      }

      ReceiveEventLogCallback(null, sb.ToString(), "SocketRead");

    }

    // ------------------------------ Read_ProcessTelnetCommand ------------------------
    // got an IAC control code from the server. Process the command that follows.
    Nullable<byte> Read_ProcessTelnetCommand(NetworkStream InNws)
    {
      Nullable<byte> escapeData = null;

      // read the cmd code byte which follows the IAC byte.
      int cmdCodeInt = InNws.ReadByte();
      if (cmdCodeInt == -1)
        throw (new ConnectException("Unexpected code following IAC from server"));
      var cmdCode = ((byte)cmdCodeInt).ToTelnetCommandCode();

      // write cmdCode to event log
      if (mReceiveEventLogCallback != null)
      {
        Read_LogTelnetCommandByte((byte)cmdCodeInt, "CmdCode");
      }

      // another IAC after the IAC. Interpret as a single data char.
      if (cmdCode == CommandCode.IAC)
      {
        escapeData = (byte)cmdCodeInt;
      }

      else
      {
        escapeData = null;
        byte b1, b2, b3;

        // argument byte follows cmd byte.
        int cmdArgInt = InNws.ReadByte();
        if (cmdArgInt == -1)
        {
          string errMsg =
            "Unexpected arg byte (" + cmdArgInt.ToString("X").PadLeft(2, '0') + ")" +
            " following Telnet command code " + cmdCode.ToString();
          throw (new ConnectException(errMsg));
        }
        byte cmdArg = (byte)cmdArgInt;

        // write cmdArg to event log
        if (mReceiveEventLogCallback != null)
        {
          Read_LogTelnetCommandByte(cmdArg, "CmdArg");
        }

        // reply to DO with a WILL or WONT depending on argument.
        if (cmdCode == CommandCode.DO)
        {

          b1 = (byte)CommandCode.IAC;
          if (cmdArg == (byte)TelnetSubject.SUPPRESS_GO_AHEAD)
          {
            b2 = (byte)CommandCode.WILL;
          }
          else
          {
            b2 = (byte)CommandCode.WONT;
          }
          b3 = (byte)cmdArg;

          lock (mWriteLock)
          {
            InNws.WriteByte(b1);
            InNws.WriteByte(b2);
            InNws.WriteByte(b3);
          }
        }

        // reply to WILL with DO or DONT
        else if (cmdCode == CommandCode.WILL)
        {
          b1 = (byte)CommandCode.IAC;
          if (cmdArg == (byte)TelnetSubject.SUPPRESS_GO_AHEAD)
          {
            b2 = (byte)CommandCode.DO;
          }
          else
          {
            b2 = (byte)CommandCode.DONT;
          }
          b3 = cmdArg;

          lock (mWriteLock)
          {
            InNws.WriteByte(b1);
            InNws.WriteByte(b2);
            InNws.WriteByte(b3);
          }

        }

        else
        {
          string errMsg =
            "Invalid cmd code (" + cmdCodeInt.ToString("X").PadLeft(2, '0') + ")" +
            " following Telnet IAC byte";
          escapeData = (byte)cmdCodeInt;
          //          throw new TelnetConnException(errMsg);
        }
      }

      return escapeData;
    }

    /// <summary>
    /// read and return all the bytes to be read from the network stream. 
    /// </summary>
    /// <param name="Client"></param>
    /// <param name="NetStream"></param>
    /// <returns></returns>
    public static Tuple<InputByteArray, List<string>> ReadInputBlock(
      TcpClient Client, NetworkStream NetStream, bool ForceRead = false)
    {
      InputByteArray inputArray = null;
      List<string> logList = new List<string>();

      if ((NetStream.CanRead == true) && (NetStream.DataAvailable == true))
      {
        var readBuffer = new byte[Client.ReceiveBufferSize];
        int readLx = NetStream.Read(readBuffer, 0, Client.ReceiveBufferSize);
        inputArray = new InputByteArray(readBuffer, readLx);
      }

      else if ((NetStream.CanRead == true) && (ForceRead == true))
      {
        logList.Add("==== Force Read ======");

        var readBuffer = new byte[Client.ReceiveBufferSize];
        int readLx = NetStream.Read(readBuffer, 0, Client.ReceiveBufferSize);
        inputArray = new InputByteArray(readBuffer, readLx);
        {
          var bufText = readBuffer.ToHex(0, readLx, ' ');
          logList.Add("Read network stream. Num bytes: "
            + readLx.ToString() + "  " + bufText);
        }
      }

      else
      {
        inputArray = new InputByteArray();
      }

      return new Tuple<InputByteArray, List<string>>(inputArray, logList);
    }

    StringBuilder ReadIntoBuffer()
    {
      StringBuilder sb = new StringBuilder();
      NetworkStream nws = mSock.GetStream();

      try
      {

        bool hasDisconnected = false;
        while (hasDisconnected == false)
        {
          // break out if have some data and there is no more to get.
          if ((sb.Length > 0) && (nws.DataAvailable == false))
            break;

          int dx = nws.ReadByte();

          switch (dx)
          {
            // got nothing. Since Read blocks until data arrives, a return value
            // indicating zero bytes means the remote has disconnected.
            case -1:
              {
                this.Disconnect();
                hasDisconnected = true;
                break;
              }

            // interpret as command
            case (int)CommandCode.IAC:
              {
                Nullable<byte> escapeData = null;
                escapeData = Read_ProcessTelnetCommand(nws);
                if (escapeData != null)
                {
                  sb.Append(escapeData.Value);
                }
                break;
              }

            default:
              sb.Append((char)dx);
              break;
          }
        }

        // call the event log hook with hex rep of data received.
        if (ReceiveEventLogCallback != null)
        {
          foreach (string s1 in Stringer.ToHexExternalFormLines(sb.ToString()))
          {
            ReceiveEventLogCallback(null, s1, "");
          }
        }

      }
      catch (InvalidOperationException)
      {
      }

      return sb;
    }

    public string[] ReadLines(int InTimeOutMs)
    {
      string[] lines = null;
      string s1 = Read(InTimeOutMs);
      if (s1 == null)
      {
        lines = null;
      }
      else
      {
        string[] splitPatterns = new string[] { Environment.NewLine, "\r" };
        lines =
          s1.Split(splitPatterns, StringSplitOptions.None);
      }
      return lines;
    }

    public delWriteEventLog ReceiveEventLogCallback
    {
      get { return mReceiveEventLogCallback; }
      set { mReceiveEventLogCallback = value; }
    }

    public void ThrowNotConnected()
    {
      if (mSock.Connected == false)
        throw new NotConnectedException();
    }

    public void Write(string InCmd)
    {
      ThrowNotConnected();

      byte[] buf =
        System.Text.ASCIIEncoding.ASCII.GetBytes(InCmd.Replace("\0xFF", "\0xFF\0xFF"));

      NetworkStream nws = mSock.GetStream();
      lock (mWriteLock)
      {
        nws.Write(buf, 0, buf.Length);
      }
    }

    public void WriteLine(string cmd)
    {
      Write(cmd + Environment.NewLine);
    }
    private static void WriteToHost(
      TelnetLogList LogList, TelnetCommand Stmt, NetworkStream NetStream)
    {
      WriteToHost(LogList, Stmt.ToBytes(), NetStream);
    }

    public static void WriteToHost(
      TelnetLogList LogList, byte[] Buffer, NetworkStream NetStream)
    {
      if (NetStream.CanWrite == true)
      {
        if (LogList != null)
        {
          LogList.AddChunk(Direction.Write, Buffer);
        }
        NetStream.Write(Buffer, 0, Buffer.Length);

        TrafficLogFile.Write(Direction.Write, Buffer);
      }
      else
      {
        throw new Exception("cannot write to network stream");
      }
    }
  }

  public static class TelnetExt
  {
  }
}
