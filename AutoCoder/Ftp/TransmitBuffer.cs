using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Ftp
{
  public class TransmitBuffer : List<TransmitBufferItem>
  {

    public TransmitBufferItem AddItem(CommDirection InDir, int InBufferSx)
    {
      TransmitBufferItem item = new TransmitBufferItem(InDir, InBufferSx);
      base.Add(item);
      return item;
    }
  }
}
