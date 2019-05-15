using System;
using System.Collections ;
using System.Text ;
using System.Web.UI ;
using System.Web.UI.HtmlControls;

namespace AutoCoder.Web.HtmlControls
{

	// ---------------------- AcDivControl -------------------------------
	public class AcDivControl : HtmlGenericControl
	{
		public AcDivControl( ) :
			base( "div" )
		{
		}

		public AcDivControl AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ;
		}

		// ---------------------- AcDivControl.AddNewLiteralControl ------------------
		public AcLiteralControl AddNewLiteralControl( string InText )
		{
			AcLiteralControl lit = new AcLiteralControl( InText ) ;
			Controls.Add( lit ) ;
			return lit ;
		}

		// ---------------------- AcDivControl.AddNewSpanControl ------------------
		public AcSpanControl AddNewSpanControl( string InText )
		{
			AcSpanControl span = new AcSpanControl( InText ) ;
			Controls.Add( span ) ;
			return span ;
		}

		// ---------------------- AcDivControl.AddNewSpanControl ------------------
		public AcSpanControl AddNewSpanControl( )
		{
			AcSpanControl span = new AcSpanControl( ) ;
			Controls.Add( span ) ;
			return span ;
		}

		public AcDivControl AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}
		
		// -------------------------- AcDivControl.SetFontName ----------------------
		public AcDivControl SetFontName( string InName )
		{
			Style.Add( "font-family", InName ) ;
			return this ;
		}
	}

	// ------------------------ AcLiControl ----------------------------
	public class AcLiControl : HtmlGenericControl
	{
		public AcLiControl( )
			: base( "li" )
		{
		}

		public AcLiControl( string InText )
			: base( "li" )
		{
			InnerText = InText ;
		}

		public AcLiControl AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}
	}

	// ---------------------------- AcLinkControl ------------------------------
	// control whose job it is to render text which can be clicked on to link to
	// the specified URL.
	public class AcLinkControl : HtmlContainerControl
	{
		bool mLaunch ;
		string mUrl ;
		string mLinkText ;

		public AcLinkControl( )
		{
			mLaunch = false ;
			mLinkText = null ;
		}

		public bool Launch
		{
			get { return mLaunch ; }
			set { mLaunch = value ; }
		}

		public string LinkText
		{
			get { return mLinkText ; }
			set { mLinkText = value ; }
		}

		public string Url
		{
			get { return mUrl ; }
			set { mUrl = value ; }
		}

		protected override void Render( HtmlTextWriter InWriter )
		{
			string span ;
			string quot = "\"" ;
			if ( mUrl == null )
				throw( new ApplicationException( "AcLinkControl missing URL" )) ;

			// make sure the control has some basic styles.
			AssureStyleKey( Style, "text-decoration", "underline" ) ;
			AssureStyleKey( Style, "cursor", "hand" ) ;

			// render the style collection to html attribute form.
			string StyleString = "Style=\"" + StyleCollectionToString( Style ) + quot ;

			span =
        "<span onclick=" + quot + "window.open( '" + mUrl + "', '', '' );" +
				quot + " " + StyleString + ">" ;
			InWriter.Write( span ) ;

			// the link text is either spcfd as LinkText= or is contained within the child
			// collection of the control  <AcLinkControl>xxxx</AcLinkControl>
			string linkText = null ; 
			if ( mLinkText != null )
				linkText = mLinkText ;
			else
			{
				foreach( LiteralControl control in Controls )
				{
					linkText = control.Text ;
				}
			}
			InWriter.Write( linkText ) ;

			InWriter.Write( "</span>" ) ;
		}

		private void AssureStyleKey( CssStyleCollection InStyle, string InKey, string InValue )
		{
			if ( InStyle[ InKey ] == null )
				InStyle.Add( InKey, InValue ) ;
		}

		private string StyleCollectionToString( CssStyleCollection InColl )
		{
			IEnumerator keys = InColl.Keys.GetEnumerator();
			StringBuilder sb = new StringBuilder( 1000 ) ;
 			while (keys.MoveNext()) 
			{
 				String key = (String)keys.Current ;
				sb.Append( key + ":" + InColl[key] + ";" ) ;
			}
			return sb.ToString( ) ;
		}

	}

	// ------------------------ AcLiteralControl ----------------------------
	public class AcLiteralControl : LiteralControl
	{
		public AcLiteralControl( string InText )
			: base( InText )
		{
		}

	}

	public enum OrderedListType
		{ Numeric, LowerCase, UpperCase, LowerCaseRoman, UpperCaseRoman, Disc, Circle, Square }
	
	// ------------------------- AcOlControl ------------------------------
	public class AcOlControl : HtmlGenericControl
	{
		public AcOlControl( )
			: base( "ol" )
		{
		}

		public AcLiControl AddNewLiControl( string InText )
		{
			AcLiControl li = new AcLiControl( InText ) ;
			Controls.Add( li ) ;
			return li ;
		}

		public AcLiControl AddNewLiControl( )
		{
			AcLiControl li = new AcLiControl( ) ;
			Controls.Add( li ) ;
			return li ;
		}

		public AcOlControl AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcOlControl SetListType( OrderedListType InType )
		{
			switch( InType )
			{
				case OrderedListType.Numeric:
					Attributes.Add( "type", "1" ) ;
					break ;
				case OrderedListType.LowerCase:
					Attributes.Add( "type", "a" ) ;
					break ;
				case OrderedListType.UpperCase:
					Attributes.Add( "type", "A" ) ;
					break ;
				case OrderedListType.LowerCaseRoman:
					Attributes.Add( "type", "i" ) ;
					break ;
				case OrderedListType.UpperCaseRoman:
					Attributes.Add( "type", "I" ) ;
					break ;
				case OrderedListType.Disc:
					Attributes.Add( "type", "disc" ) ;
					break ;
				case OrderedListType.Circle:
					Attributes.Add( "type", "circle" ) ;
					break ;
				case OrderedListType.Square:
					Attributes.Add( "type", "square" ) ;
					break ;
			}
			return this ;
		}

	} // end class AcOlControl

	// ------------------------ AcSpanControl -------------------------------
	public class AcSpanControl : HtmlGenericControl
	{
		public AcSpanControl( )
			: base( "span" )
		{
		}

		public AcSpanControl( string InInnerText )
			: base( "span" )
		{
			InnerText = InInnerText ;
		}

		public AcSpanControl AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcSpanControl SetInnerText( string InText )
		{
			InnerText = InText ;
			return this ;
		}

		public AcSpanControl SetStyleColor( string InColorName )
		{
			Style.Add( "color", InColorName ) ;
			return this ;
		}
		
	} // end class AcSpanControl

	
}
