using System;
using System.Collections ;

namespace AutoCoder
{

	public class StringxUtil
	{
		public StringxUtil()
		{
		}

		// ---------------------- SmartSplit ------------------------
		// split on ",", but not if comma within quotes.
		public static ArrayList SmartxSplit( string InLine )
		{ 
			ArrayList parts = new ArrayList(); 
			int ix, iStart = 0 ; 
			bool bInQuotes = false ; 

			int lx = InLine.Length ;
			for ( ix = 0 ; ix < lx ; ++ix ) 
			{ 
				switch (InLine[ix]) 
				{ 
					case ',': 
						if ( bInQuotes == false ) 
						{ 
							parts.Add( InLine.Substring( iStart, ix - iStart ) ); 
							iStart = ix + 1 ; 
						} 
						break; 
					case '"':   // read: single double single quote 
						bInQuotes = !bInQuotes ;
						break; 
				} 
			} // end for 
			parts.Add( InLine.Substring( iStart, ix - iStart )) ; 
			return parts ;
		} 

	}
}
