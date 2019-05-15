using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Web.Security;

namespace AutoCoder.Web
{

	public class AcUserControl : UserControl
	{

		public string GetUserId( ) 
		{
			return( GetPageOfControl( ).GetUserId( )) ;
		}

		public string GetVisitorId( ) 
		{
			return( GetPageOfControl( ).GetVisitorId( )) ;
		}

		public AcPage GetPageOfControl( )
		{
			return( (AcPage) Page ) ;
		}


		// ------------------------ GetControlViewStateIntValue ---------------------
		// get int value from state bag. if not found, return arg2 value.
		public int GetControlViewStateIntValue( string InKey, int InNullValue  )
		{
			object vsv = ViewState[InKey] ;
			if ( vsv == null )
				return InNullValue ;
			else
				return (int) vsv ;
		}

		// ------------------------ GetControlViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		public string GetControlViewStateValue( string InKey, string InNullValue  )
		{
			object vsv = ViewState[InKey] ;
			if ( vsv == null )
				return InNullValue ;
			else
				return vsv.ToString( ) ;
		}

	}
}
