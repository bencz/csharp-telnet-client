using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel ;
using System.Configuration ;
using System.Globalization ;
using System.Text ;

namespace AutoCoder.WebControls.Classes
{

	// --------------------- MenuOptionCategory ----------------------
	public enum MenuOptionCategory { email, ebay, Document, SitePage, Url } ;

	// -------------------------- MenuOptionException ---------------------
	public class MenuOptionException : ApplicationException
	{

		public MenuOptionException( string InMessage )
			: base( InMessage )
		{
		}
		public MenuOptionException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	}

	// ----------------------- MenuOptionList ---------------------------------
	public class MenuOptionList : System.Collections.ArrayList 
	{
		public MenuOptionList()
		{
		}

		public MenuOptionRow AddNewMenuOption( )
		{
			MenuOptionRow optn = new MenuOptionRow( ) ;
			Add( optn ) ;
			return optn ;
		}

		public MenuOptionRow FindMenuOption( string InOptionName )
		{
			MenuOptionRow optn = null ;
			IEnumerator it = GetEnumerator( ) ; 
			while ( it.MoveNext( ) == true ) 
			{
				optn = (MenuOptionRow) it.Current ;
				if ( optn.OptionName == InOptionName )
					return optn ;
			}
			return null ;
		}
	} // end MenuOptionList class


	// ------------------------- MenuOptionRow ----------------------
	[ TypeConverter( typeof( MenuOptionRowConverter)) ] 
	public class MenuOptionRow
	{
		string mMenuName ;
		int mOptionNumber ;
		MenuOptionCategory mCategory ; 
		string mOptionText ;
		string mOptionName ;
		string mDocumentFileName ;
		string mUrl ;

		public MenuOptionRow( )
		{
			mCategory = MenuOptionCategory.Document ;
			mOptionText = "" ;
		}

		public MenuOptionCategory Category
		{
			get { return mCategory ; }
			set { mCategory = value ; }
		}

		public string MenuName
		{
			get { return mMenuName ; }
			set { mMenuName = value ; }
		}
		public int OptionNumber
		{
			get { return mOptionNumber ; }
			set { mOptionNumber = value ; }
		}
		public string OptionText
		{
			get
			{
				if ( mOptionText == null )
					return "" ;
				else
					return mOptionText ;
			}
			set { mOptionText = value ; }
		}
		public string OptionName
		{
			get
			{
				if ( mOptionName == null )
					return "" ;
				else
					return mOptionName ;
			}
			set { mOptionName = value ; }
		}

		public string DocumentFileName
		{
			get
			{
				if ( mDocumentFileName == null )
					return "" ;
				else
					return mDocumentFileName ;
			}
			set { mDocumentFileName = value ; }
		}
		public string Url
		{
			get
			{
				if ( mUrl == null )
					return "" ;
				else
					return mUrl ;
			}
			set { mUrl = value ; }
		}

		// fill the MenuOptionRow with contents of DataRow
		public MenuOptionRow AssignDataRow( DataRow InRow )
		{
			MenuName = InRow[0].ToString( ) ;
			OptionNumber = (int) InRow[1] ;
			SetOptionCategory( InRow[2].ToString( )) ;
			OptionText = InRow[3].ToString( ) ;
			OptionName = InRow[4].ToString( ) ;
			DocumentFileName = InRow[5].ToString( ) ;
			Url = InRow[6].ToString( ) ;
			return this ;
		}

		// ------------------------ FromString --------------------------
		// create a MenuOptionRow object from a comma delimeted string.
		public static MenuOptionRow FromString( string InValue )
		{
			MenuOptionRow row = new MenuOptionRow( ) ;
			ArrayList v = SmartSplit( InValue ) ;
//			string[] v = ((string)value).Split(new char[] {','}) ;
			row.SetMenuName( (string)v[0] )
				.SetOptionNumber( int.Parse( (string)v[1] ))
				.SetOptionCategory( (string)v[2] )
				.SetOptionText( (string)v[3] )
				.SetOptionName( (string)v[4] )
				.SetDocumentFileName( (string)v[5] )
				.SetUrl( (string)v[6] ) ;
			return row ;
		}

		// ---------------------- SmartSplit ------------------------
		// split on ",", but not if comma within quotes.
		private static ArrayList SmartSplit( string InLine )
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

		// ------------------------ ToString ------------------------
		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( 512 ) ;
			sb.Append( mMenuName ) ;
			sb.Append( ", " + mOptionNumber.ToString( )) ;
			sb.Append( ", " + mCategory.ToString( )) ;
			sb.Append( ", " + mOptionText ) ;
			sb.Append( ", " + mOptionName ) ;
			sb.Append( ", " + mDocumentFileName ) ;
			sb.Append( ", " + mUrl ) ;
			return sb.ToString( ) ;
		}

		public MenuOptionRow SetMenuName( string InMenuName )
		{
			mMenuName = InMenuName ;
			return this ; 
		}
		public MenuOptionRow SetOptionCategory( MenuOptionCategory InCat )
		{
			mCategory = InCat ;
			return this ; 
		}
		public MenuOptionRow SetOptionCategory( string InCat )
		{
			mCategory =
				(MenuOptionCategory) Enum.Parse( typeof(MenuOptionCategory), InCat ) ;
			return this ; 
		}
		public MenuOptionRow SetOptionNumber( int InOptionNumber )
		{
			mOptionNumber = InOptionNumber ;
			return this ; 
		}
		public MenuOptionRow SetOptionText( string InText )
		{
			mOptionText = InText ;
			return this ; 
		}
		public MenuOptionRow SetOptionName( string InName )
		{
			mOptionName = InName ;
			return this ; 
		}
		public MenuOptionRow SetDocumentFileName( string InDocName )
		{
			mDocumentFileName = InDocName ;
			return this ; 
		}
		public MenuOptionRow SetUrl( string InUrl )
		{
			mUrl = InUrl ;
			return this ; 
		}
	} // end MenuOptionRow


	// --------------------- MenuOptionRowConverter ---------------------------
	public class MenuOptionRowConverter : TypeConverter 
	{

		// this TypeConverter can convert from string.
		public override bool CanConvertFrom(
			ITypeDescriptorContext context, 
			Type sourceType) 
		{
			if (sourceType == typeof(string)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		// convert from string to MenuOptionRow
		public override object ConvertFrom(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue ) 
		{
			if ( InValue is string ) 
			{
				return MenuOptionRow.FromString( (string)InValue ) ;
			}
			return base.ConvertFrom(context, culture, InValue ) ;
		}

		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue,
			Type destinationType) 
		{  
			if (destinationType == typeof(string)) 
			{
				MenuOptionRow row = (MenuOptionRow) InValue ;
				return( row.ToString( )) ;
			}
			return base.ConvertTo(context, culture, InValue, destinationType);
		}
	}

}
