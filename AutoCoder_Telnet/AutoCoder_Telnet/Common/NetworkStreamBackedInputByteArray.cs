using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
#if skip
  /// <summary>
  /// InputByteArray that also contains the telnet connection that is used to
  /// receive more data into the InputByteArray.
  /// </summary>
  public class xNetworkStreamBackedInputByteArray : InputByteArray
  {
    public TcpClient Client
    { get; set; }

    public NetworkStream NetStream
    { get; set; }
    public FromThread FromThread
    { get; set; }

    public xNetworkStreamBackedInputByteArray( TcpClient Client, NetworkStream NetStream, FromThread FromThread )
      : base()
    {
      this.Client = Client;
      this.NetStream = NetStream;
      this.FromThread = FromThread;
    }
    public xNetworkStreamBackedInputByteArray(byte[] Bytes, int DataLgth)
      : base(Bytes, DataLgth)
    {
      this.Client = null;
      this.NetStream = null;
      this.FromThread = null;
    }

    public TelnetLogList ReadFromNetworkStream(
      int SleepTime = 0, int MaxTry = 0, string LogText = null, bool ForceRead = false)
    {
      this.EmptyArray();
      var logList = new TelnetLogList();
      logList.AddImportantItem(Direction.Read, "ReadFromNetworkStream. Enter method");
      int tryCx = 0;
      if (this.NetStream != null)
      {
        while (true)
        {
          if ((NetStream.CanRead == true) && (NetStream.DataAvailable == true))
          {
            var readBuffer = new byte[Client.ReceiveBufferSize];
            int readLx = NetStream.Read(readBuffer, 0, Client.ReceiveBufferSize);
            this.LoadArray(readBuffer, readLx);
            {
              var bufText = readBuffer.ToHex(0, readLx, ' ');
              logList.AddChunk(Direction.Read, readBuffer.SubArray(0, readLx), LogText);
            }
          }

          else
          {
            this.EmptyArray();
          }

          // got something.
          if (this.DataLgth > 0)
          {
            break;
          }

          // max number of read tries reached.
          tryCx += 1;
          if (tryCx >= MaxTry)
            break;

          // sleep before another read attempt.
          for(int cx = 0; cx < SleepTime; cx++ )
          {
            Thread.Sleep(1);
          }
          logList.AddImportantItem(Direction.Read, "ReadFromNetworkStream. Sleep " + SleepTime);
        }
      }
      logList.AddImportantItem(Direction.Read, "ReadFromNetworkStream. Exit method. " + this.DataLgth);
      return logList;
    }
  }
#endif

}
