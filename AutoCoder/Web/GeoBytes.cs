using System ;
using System.Collections ;
using AutoCoder.TextStmt ;
using AutoCoder.Text ;

namespace AutoCoder.Web.GeoBytes
{

	// ----------------------------- IpInfo ----------------------------
	public class IpInfo
	{
		public string Country ;
		public string Region ;
		public string City ;
		public string TimeZone ;
	}

	// --------------------------- GeoBytes ----------------------------
	public class GeoBytes
	{
		public GeoBytes()
		{
		}

		// ------------------------- GetIpGeoInfo ---------------------------
		public static ArrayList GetFullIpLocator( string InIpAddr )
		{
			string GeoUrl =
				"http://www.geobytes.com/IpLocator.htm" +
				"?GetLocation&template=valuepairs.txt" +
				"&IpAddress=" + InIpAddr ;
			ArrayList results = AutoCoder.Web.Utility.HttpGet( GeoUrl ) ;
			return results ;
		}

		public static IpInfo GetIpGeoInfo( string InIpAddr )
		{
			IpInfo ipInfo = new IpInfo( ) ;

			// get an arraylist containing all the ip info from GeoBytes.
			ArrayList ts = GetFullIpLocator( InIpAddr ) ;

			// step thru the arraylist of ip info.
			IEnumerator it = ts.GetEnumerator( ) ; 
			while ( it.MoveNext( ) == true ) 
			{

				// load the ip info stmt line.
				string IpInfoLine =	(string) it.Current ;
				if ( IpInfoLine == null )		continue ;
				IpInfoLine = Stringer.TrimStartAndEnd(
					IpInfoLine,
					Chars.WhitespaceChars( ),
					Chars.WhitespaceChars( ';' )) ;
				TextStmt.TextStmt stmt = new TextStmt.TextStmt( IpInfoLine ) ;

				// each stmt line is in assignment form. split the stmt into its lhs and
				// rhs parts.
				SplitResults lineParts = stmt.Split( '=' ) ;
				if ( lineParts.Length != 2 )
					continue ;
				string lhs = lineParts.Content[0].Trim( ) ;
				string rhs = lineParts.Content[1].Trim( ) ;
				
				// depending on the lhs value name, store the rhs value.
				if ( lhs == "country" )
					ipInfo.Country = rhs ;
				else if ( lhs == "region" )
					ipInfo.Region = rhs ;
				else if ( lhs == "city" )
					ipInfo.City = rhs ;
			}
			return ipInfo ;
		}

	}
}
