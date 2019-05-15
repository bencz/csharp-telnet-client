using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Response
{
  public class ResponseDataStreamBuilder
  {
    private ByteArrayBuilder ByteArray
    {
      get; set;
    }

    public ResponseDataStreamBuilder( )
    {
      this.ByteArray = new ByteArrayBuilder();
    }

    public void Append( byte[] Bytes )
    {
      this.ByteArray.Append(Bytes);
    }
  }

}
