using System ;
using System.Collections ;
using System.Configuration ;
using System.IO ;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder.Web
{
	// -------------------------- Utility -------------------------
	public class Utility
	{
		public Utility()
		{
		}

		// ------------------------ GetSessionString ----------------------------
		public static string GetSessionString(
			System.Web.UI.Page InPage,
			string InKey,
			string InDefaultValue )
		{
			string Value = (string) InPage.Session[ InKey ] ; 
			if ( Value == null )
				Value = InDefaultValue ;
			return Value ;
		}

		// --------------------------- HttpGet --------------------------------
		// connect to the URL in HTTP GET format ( parms in the url in querystring form ).
		// Return the stream response data.
		public static ArrayList HttpGet( string InUrl )
		{
			HttpWebResponse resp = null ;
			StreamReader sr = null ;
			ArrayList RespLines = null ;

			HttpWebRequest rqs = (HttpWebRequest)WebRequest.Create( InUrl ) ;
			rqs.MaximumAutomaticRedirections = 4;
			rqs.MaximumResponseHeadersLength = 4;

			// setup the proxy server if such a server exists in the web.config file.
			string ProxyUrl = ConfigurationSettings.AppSettings["WebRequestProxyUrl"] ;
			if ( ProxyUrl != null )
			{
				WebProxy proxy = new WebProxy( ProxyUrl ) ;
				rqs.Proxy = proxy ;
			}

			rqs.Credentials = CredentialCache.DefaultCredentials;
			try
			{
				resp = (HttpWebResponse)rqs.GetResponse( ) ;

				Stream receiveStream = resp.GetResponseStream( ) ;
				sr = new StreamReader( receiveStream, Encoding.UTF8 ) ;
				RespLines = new ArrayList( ) ;

				string line = sr.ReadLine( ) ;
				while( line != null )
				{
					line = sr.ReadLine( ) ;
					RespLines.Add( line ) ;
				}
			}
			finally
			{
				if ( sr != null )
					sr.Close( ) ;
				if ( resp != null )
					resp.Close( ) ;
			}
			return RespLines ;
		}

	}
}
