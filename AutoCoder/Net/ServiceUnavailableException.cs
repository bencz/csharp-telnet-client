using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

#if skip
namespace AutoCoder.Net
{
  public class ServiceUnavailableException : ApplicationException
  {
    public ServiceUnavailableException(WebException InnerExcp)
      : base("Service unavabile WebException received", InnerExcp )
    {
    }
  }
}

#endif