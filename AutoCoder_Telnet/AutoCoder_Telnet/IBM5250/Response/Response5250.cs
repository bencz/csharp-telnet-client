using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Response
{
  /// <summary>
  /// methods for building and parsing the data stream that is sent to the 5250 
  /// telnet server.
  /// </summary>
  public static class Response5250 
  {
    /// <summary>
    /// parse the 5250 data stream that is sent from the client to the server.
    /// </summary>
    /// <param name="LogFile"></param>
    /// <param name="ToServerStream"></param>
    /// <returns></returns>
    public static Tuple<ResponseItemList, string> ParseResponseStream(
      InputByteArray InputArray, ResponseHeader ResponseHeader = null)
    {
      var responseItemList = new ResponseItemList();
      string errmsg = null;

      var writeStream = new ByteArrayBuilder();
      DataStreamHeader dsHeader = null;
      ResponseHeader responseHeader = ResponseHeader;

      // stream starts with data stream header.
      if (responseHeader == null)
      {
        var rv = DataStreamHeader.Factory(InputArray);
        dsHeader = rv.Item1;
        responseItemList.Add(dsHeader);
        errmsg = dsHeader.Errmsg;

        // next is the response header.
        if (errmsg == null)
        {
          responseHeader = new ResponseHeader(InputArray);
          responseItemList.Add(responseHeader);
          errmsg = responseHeader.Errmsg;
        }
      }

      // look for 5250 query reply.
      if ((errmsg == null) && (responseHeader.AidKey != null)
        && (responseHeader.AidKey.Value == AidKey.Query5250Reply))
      {
        var queryResp = new Query5250Response(InputArray);
        if (queryResp.Errmsg == null)
        {
          responseItemList.Add(queryResp);
        }
      }

      // repeating instances of sbaOrder, textDataOrder pairs.
      while (true)
      {
        var telCmd = TelnetCommand.Factory(InputArray);
        if (telCmd != null)
        {
          continue;
        }

        // check that an SBA order is starting. Leave loop when it is not.
        if (SetBufferAddressOrder.CheckOrder(InputArray) != null)
          break;

        var orderPair = new LocatedTextDataOrderPair(InputArray);
        if (orderPair.Errmsg != null)
        {
          break;
        }
        responseItemList.Add(orderPair);
      }

      return new Tuple<ResponseItemList, string>(responseItemList, errmsg);
    }
  }
}
