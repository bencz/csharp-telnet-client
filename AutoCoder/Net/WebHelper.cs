using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

#if skip

namespace AutoCoder.Net
{

  public static class WebHelper
  {

    public static string BuildEndPoint(
      string Url, string WebServiceType, QueryStringBuilder QueryString)
    {
      StringBuilder sb = new StringBuilder();

      if (Url.IsNullOrEmpty())
        throw new ApplicationException("URL is empty");

      // add the url to the builder.
      sb.Append(Url);

      // add servicetype
      if (WebServiceType.IsNullOrEmpty() == false)
      {
        if (sb.EndsWith("/"))
          sb.Length = sb.Length - 1;

        if (WebServiceType[0] != '/')
          sb.Append('/');

        sb.Append(WebServiceType);
      }

      // QueryString
      if (QueryString.IsNullOrEmpty() == false)
      {
        sb.Append('?' + QueryString.ToString( ));
      }

      return sb.ToString();
    }

    public static void CopyStream(Stream from, Stream to)
    {
      if (!from.CanRead)
      {
        throw new ArgumentException("from Stream must implement the Read method.");
      }

      if (!to.CanWrite)
      {
        throw new ArgumentException("to Stream must implement the Write method.");
      }

      from.Position = 0;

      const int SIZE = 1024 * 1024;
      byte[] buffer = new byte[SIZE];

      int read = 0;
      while ((read = from.Read(buffer, 0, buffer.Length)) > 0)
      {
        to.Write(buffer, 0, read);
      }
    }

    /// <summary>
    /// Send web request to server using GET method.
    /// Return with response stream as a string.
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="WebServiceType"></param>
    /// <param name="QueryString"></param>
    /// <returns></returns>
    public static string SendWebRequest_Get(
      string Url, string WebServiceType, QueryStringBuilder QueryString)
    {
      string endPoint = BuildEndPoint(Url, WebServiceType, QueryString);
      string respText = SendWebRequest_Get(endPoint);
      return respText;
    }

    public static string SendWebRequest_Get(string EndPoint)
    {
      HttpWebRequest request = null;
      {
        request = (HttpWebRequest)WebRequest.Create(EndPoint);
        request.Method = "GET";
        request.ContentLength = 0;
        request.ContentType = "text/xml";
      }

      try
      {
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          string respText = null;

          if (response.StatusCode != HttpStatusCode.OK)
          {
            string message = String.Format(
              "GET failed. Received HTTP {0}", response.StatusCode);
            throw new ApplicationException(message);
          }

          // read the response
          using (var responseStream = response.GetResponseStream())
          {
            using (var reader = new StreamReader(responseStream))
            {
              respText = reader.ReadToEnd();
            }
          }

          return respText;
        }
      }
      catch (WebException excp)
      {
        if (excp.Message.Contains("(503) Server Unavailable"))
        {
          throw new ServiceUnavailableException(excp);
        }
        throw excp;
      }
    }

#if skip
    private string ToQueryString(NameValueCollection nvc)
    {
      return "?" + 
        string.Join("&", 
        Array.ConvertAll(
        nvc.AllKeys, 
        key => string.Format("{0}={1}", 
          HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
    } 
#endif


    /// <summary>
    /// Send web request to server using the POST method.
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="WebServiceType"></param>
    /// <param name="QueryString"></param>
    /// <param name="ContentData"></param>
    /// <param name="UserAgent"></param>
    /// <param name="ContentType"></param>
    /// <param name="Timeout"></param>
    /// <returns></returns>
    public static string SendWebRequest_Post(
      string Url, string WebServiceType, QueryStringBuilder QueryString,
      byte[] ContentData,
      string UserAgent = null,
      string ContentType = "application/x-www-form-urlencoded; charset=utf-8",
      IDictionary<string, string> HeaderMap = null,
      Stream SendData = null,
      int Timeout = 50000)
    {
      string endPoint = BuildEndPoint(Url, WebServiceType, QueryString);

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
      request.Timeout = Timeout;
      request.UserAgent = UserAgent;
      request.Method = "POST";
      request.ContentType = ContentType;

      if (ContentData != null)
      {
        request.ContentLength = ContentData.Length;
      }

      // add headers
      if (HeaderMap != null)
      {
        WebHeaderCollection headers = request.Headers;
        foreach (String key in HeaderMap.Keys)
        {
          headers.Add(key, HeaderMap[key]);
        }
      }

      // fill the WebRequest content stream.
      if (ContentData != null)
      {
        using (Stream requestStream = request.GetRequestStream())
        {
          requestStream.Write(ContentData, 0, ContentData.Length);
          requestStream.Close();
        }
      }

      // copy the SendData into the web request.
      if (SendData != null)
      {
        using (Stream requestStream = request.GetRequestStream())
        {
          SendData.Position = 0;
          CopyStream(SendData, requestStream);
          requestStream.Close();
        }
      }

      try
      {
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          string respText = null;

          if (response.StatusCode != HttpStatusCode.OK)
          {
            string message = String.Format(
              "POST failed. Received HTTP {0}", response.StatusCode);
            throw new ApplicationException(message);
          }

          // read the response
          using (var responseStream = response.GetResponseStream())
          {
            using (var reader = new StreamReader(responseStream))
            {
              respText = reader.ReadToEnd();
            }
          }

          return respText;
        }
      }
      catch (WebException excp)
      {
        throw excp;
      }
    }
  }

}

#endif
