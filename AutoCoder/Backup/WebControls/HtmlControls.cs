using System;
using System.Collections ;
using System.Text ;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder.WebControls
{

	// ---------------------- AcHtmlDiv -------------------------------
	public class AcHtmlDiv : HtmlGenericControl
	{
		public AcHtmlDiv( ) :
			base( "div" )
		{
		}

		public AcHtmlDiv AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ;
		}

		// ---------------------- AcHtmlDiv.AddNewHtmlLiteral ------------------
		public AcHtmlLiteral AddNewHtmlLiteral( string InText )
		{
			AcHtmlLiteral lit = new AcHtmlLiteral( InText ) ;
			Controls.Add( lit ) ;
			return lit ;
		}

		// ---------------------- AcHtmlDiv.AddNewHtmlSpan ------------------
		public AcHtmlSpan AddNewHtmlSpan( string InText )
		{
			AcHtmlSpan span = new AcHtmlSpan( InText ) ;
			Controls.Add( span ) ;
			return span ;
		}

		// ---------------------- AcHtmlDiv.AddNewHtmlSpan ------------------
		public AcHtmlSpan AddNewHtmlSpan( )
		{
			AcHtmlSpan span = new AcHtmlSpan( ) ;
			Controls.Add( span ) ;
			return span ;
		}

		public AcHtmlDiv AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlDiv SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				Attributes["class"] = InValue ;
			return this ;
		}
		
		// -------------------------- AcHtmlDiv.SetFontName ----------------------
		public AcHtmlDiv SetFontName( string InName )
		{
			Style.Add( "font-family", InName ) ;
			return this ;
		}
	}

	// ----------------------- AcHtmlInputCheckBox ----------------------------
	public class AcHtmlInputCheckBox : System.Web.UI.HtmlControls.HtmlInputCheckBox
	{
		public AcHtmlInputCheckBox( )
		{
		}

		public AcHtmlInputCheckBox AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlInputCheckBox SetChecked( bool InChecked )
		{
			Checked = InChecked ;
			return this ;
		}

		public AcHtmlInputCheckBox SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				Attributes["class"] = InValue ;
			return this ;
		}

		public AcHtmlInputCheckBox SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}
	}

	// ----------------------- AcHtmlInputText ----------------------------
	public class AcHtmlInputText : System.Web.UI.HtmlControls.HtmlInputText
	{
		public AcHtmlInputText( )
		{
		}
		public AcHtmlInputText( string InControlType )
			: base( InControlType )
		{
		}

		public AcHtmlInputText AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlInputText SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				Attributes["class"] = InValue ;
			return this ;
		}

		public AcHtmlInputText SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}

		public AcHtmlInputText SetSize( int InSize )
		{
			Size = InSize ;
			return this ;
		}

		public AcHtmlInputText SetValue( string InValue )
		{
			Value = InValue ;
			return this ;
		}
	}

	// ------------------------ AcHtmlLi ----------------------------
	public class AcHtmlLi : HtmlGenericControl
	{
		public AcHtmlLi( )
			: base( "li" )
		{
		}

		public AcHtmlLi( string InText )
			: base( "li" )
		{
			InnerText = InText ;
		}

		public AcHtmlLi AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}
	}

	// ---------------------------- AcHtmlLink ------------------------------
	// control whose job it is to render text which can be clicked on to link to
	// the specified URL.
	public class AcHtmlLink : HtmlContainerControl
	{
		bool mLaunch ;
		string mUrl ;
		string mLinkText ;

		public AcHtmlLink( )
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
				throw( new ApplicationException( "AcHtmlLink missing URL" )) ;

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
			// collection of the control  <AcHtmlLink>xxxx</AcHtmlLink>
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

	// ------------------------ AcHtmlLiteral ----------------------------
	public class AcHtmlLiteral : LiteralControl
	{
		public AcHtmlLiteral( string InText )
			: base( InText )
		{
		}
		public AcHtmlLiteral( )
		{
		}

		public AcHtmlLiteral SetText( string InValue )
		{
			Text = InValue ;
			return this ; 
		}
		public AcHtmlLiteral SetVisible( bool InValue )
		{
			Visible = InValue ;
			return this ; 
		}

		public AcHtmlLiteral SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}
	}

	public enum OrderedListType
	{ Numeric, LowerCase, UpperCase, LowerCaseRoman, UpperCaseRoman, Disc, Circle, Square }
	
	// ------------------------- AcHtmlOl ------------------------------
	public class AcHtmlOl : HtmlGenericControl
	{
		public AcHtmlOl( )
			: base( "ol" )
		{
		}
		public AcHtmlOl( string InValue )
			: base( "ol" )
		{
		}

		public AcHtmlLi AddNewHtmlLi( string InText )
		{
			AcHtmlLi li = new AcHtmlLi( InText ) ;
			Controls.Add( li ) ;
			return li ;
		}

		public AcHtmlLi AddNewHtmlLi( )
		{
			AcHtmlLi li = new AcHtmlLi( ) ;
			Controls.Add( li ) ;
			return li ;
		}

		public AcHtmlOl AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlOl SetListType( OrderedListType InType )
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

	} // end class AcHtmlOl

	// ------------------------ AcHtmlSpan -------------------------------
	public class AcHtmlSpan : HtmlGenericControl
	{
		public AcHtmlSpan( )
			: base( "span" )
		{
		}

		public AcHtmlSpan( string InInnerText )
			: base( "span" )
		{
			InnerText = InInnerText ;
		}

		public AcHtmlSpan AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlSpan SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}

		public AcHtmlSpan SetInnerText( string InText )
		{
			InnerText = InText ;
			return this ;
		}

		public AcHtmlSpan SetStyleColor( string InColorName )
		{
			Style.Add( "color", InColorName ) ;
			return this ;
		}

		public AcHtmlSpan SetVisible( bool InValue )
		{
			Visible = InValue ;
			return this ;
		}
		
	} // end class AcHtmlSpan

	// ----------------------- AcHtmlSubmitButton ----------------------------
	public class AcHtmlSubmitButton : System.Web.UI.HtmlControls.HtmlInputButton
	{
		public AcHtmlSubmitButton( )
			: base( "submit" )
		{
		}

		public AcHtmlSubmitButton AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlSubmitButton SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}

		public AcHtmlSubmitButton SetValue( string InValue )
		{
			Value = InValue ;
			return this ;
		}
	}

	// ---------------------- AcHtmlTable --------------------------------
	public class AcHtmlTable : System.Web.UI.HtmlControls.HtmlTable
	{
		PropertyTableAttributes mPropertyAttributes = null ;
	
		public TablePropertyRow AddNewPropertyRow(
			string InPromptText, string InPropertyValue )
		{
			TablePropertyRow row =
				new TablePropertyRow( this, InPromptText, InPropertyValue ) ;
			return row ;
		}

		public AcHtmlTableRow AddNewRow( )
		{
			AcHtmlTableRow row = new AcHtmlTableRow( ) ;
			Rows.Add( row ) ;
			return row ;
		}

		public AcHtmlTable AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ; 
		}

		public AcHtmlTable AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public PropertyTableAttributes PropertyAttributes
		{
			get
			{
				if ( mPropertyAttributes == null )
					mPropertyAttributes = new PropertyTableAttributes( ) ;
				return mPropertyAttributes ;
			}
			set { mPropertyAttributes = value ; }
		}

		public AcHtmlTable SetCellPadding( int InPadding )
		{
			CellPadding = InPadding ;
			return this ;
		}

		public AcHtmlTable SetCellSpacing( int InSpacing )
		{
			CellSpacing = InSpacing ;
			return this ;
		}

		public AcHtmlTable SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				AddAttribute( "class", InValue ) ;
			return this ;
		}

		public AcHtmlTable SetWidth( string InValue )
		{
			Width = InValue ;
			return this ;
		}

	} // end class AcHtmlTable

	// ----------------------- AcHtmlTableCell ----------------------------
	public class AcHtmlTableCell : System.Web.UI.HtmlControls.HtmlTableCell
	{
		public AcHtmlTableCell( )
		{
		}

		public AcHtmlDiv AddNewDiv( )
		{
			AcHtmlDiv div = new AcHtmlDiv( ) ;
			Controls.Add( div ) ;
			return div ;
		}

		public AcHtmlInputCheckBox AddNewInputCheckBox( )
		{
			AcHtmlInputCheckBox cb = new AcHtmlInputCheckBox( ) ;
			Controls.Add( cb ) ;
			return cb ;
		}

		public AcHtmlInputText AddNewInputText( )
		{
			AcHtmlInputText it = new AcHtmlInputText( ) ;
			Controls.Add( it ) ;
			return it ;
		}

		public AcHtmlInputText AddNewInputText( string InControlType )
		{
			AcHtmlInputText it = new AcHtmlInputText( InControlType ) ;
			Controls.Add( it ) ;
			return it ;
		}

		public AcAspLinkButton AddNewLinkButton( )
		{
			AcAspLinkButton link = new AcAspLinkButton( ) ;
			Controls.Add( link ) ;
			return link ;
		}

		public AcHtmlOl AddNewOl( )
		{
			AcHtmlOl ol = new AcHtmlOl( ) ;
			Controls.Add( ol ) ;
			return ol ;
		}

		public AcHtmlSpan AddNewSpan( string InText )
		{
			AcHtmlSpan span = new AcHtmlSpan( InText ) ;
			Controls.Add( span ) ;
			return span ;
		}

		public AcHtmlSubmitButton AddNewSubmitButton( )
		{
			AcHtmlSubmitButton but = new AcHtmlSubmitButton( ) ;
			Controls.Add( but ) ;
			return but ;
		}

		public AcHtmlTable AddNewTable( )
		{
			AcHtmlTable table = new AcHtmlTable( ) ;
			Controls.Add( table ) ;
			return table ;
		}

		public AcHtmlTableCell AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ; 
		}

		public AcHtmlTableCell AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlTableCell SetAlign( string InValue )
		{
			Align = InValue ;
			return this ; 
		}

		public AcHtmlTableCell SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				Attributes["class"] = InValue ;
			return this ;
		}

		public AcHtmlTableCell SetColSpan( int InNumberOfColumns )
		{
			ColSpan = InNumberOfColumns ;
			return this ;
		}

		public AcHtmlTableCell SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}

		public AcHtmlTableCell SetInnerHtml( string InHtml )
		{
			InnerHtml = InHtml ;
			return this ; 
		}

		public AcHtmlTableCell SetInnerText( string InText )
		{
			InnerText = InText ;
			return this ; 
		}

		public AcHtmlTableCell SetVAlign( string InValue )
		{
			VAlign = InValue ;
			return this ; 
		}
	}

	// ----------------------- AcHtmlTableRow --------------------------
	public class AcHtmlTableRow : System.Web.UI.HtmlControls.HtmlTableRow 
	{
		public AcHtmlTableRow( )
		{
		}

		public AcHtmlTableCell AddCell( AcHtmlTableCell InCell )
		{
			Cells.Add( InCell ) ;
			return InCell ;
		}

		public AcHtmlTableRow SetClass( string InValue )
		{
			if (( InValue != null ) && ( InValue != "" ))
				Attributes["class"] = InValue ;
			return this ;
		}
	
		// ------------------- AddNewBlankCell ------------------------------
		// add blank cell without any attributes.
		public AcHtmlTableRow AddNewBlankCell( )
		{
			AcHtmlTableCell cell = new AcHtmlTableCell( ) ;
			Cells.Add( cell ) ;
			return this ;
		}
	
		public AcHtmlTableCell AddNewCell( )
		{
			AcHtmlTableCell cell = new AcHtmlTableCell( ) ;
			Cells.Add( cell ) ;
			return cell ;
		}
	
		public AcHtmlTableCell AddNewCell( string InText )
		{
			AcHtmlTableCell cell = new AcHtmlTableCell( ) ;
			cell.InnerText = InText ;
			Cells.Add( cell ) ;
			return cell ;
		}

		public AcHtmlTableRow AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcHtmlTableRow SetID( string InValue )
		{
			ID = InValue ;
			return this ; 
		}
	}

	// ----------------------- HtmlElementAttributes ----------------------------------
	public class HtmlElementAttributes
	{
		int mWidthPct = 0 ;

		public int WidthPct
		{
			get { return mWidthPct ; }
			set { mWidthPct = value ; }
		}
		public string WidthPctString
		{
			get
			{
				return( mWidthPct.ToString( ) + "%" ) ;
			}
		}
	}

	public class PropertyTableAttributes
	{
		HtmlElementAttributes mPromptColumn ;
		HtmlElementAttributes mValueColumn ;

		string mPromptColumnWidth ;
		string mPromptColumnBackColor ;
		string mValueColumnWidth ;
		string mValueColumnBackColor ;

		public HtmlElementAttributes PromptColumn
		{
			get
			{
				if ( mPromptColumn == null )
					mPromptColumn = new HtmlElementAttributes( ) ;
				return mPromptColumn ;
			}
			set { mPromptColumn = value ; }
		}

		public HtmlElementAttributes ValueColumn
		{
			get
			{
				if ( mValueColumn == null )
					mValueColumn = new HtmlElementAttributes( ) ;
				return mValueColumn ;
			}
			set { mValueColumn = value ; }
		}

		public string PromptColumnWidth
		{
			get { return mPromptColumnWidth ; }
			set { mPromptColumnWidth = value ; }
		}
		public string ValueColumnWidth
		{
			get { return mValueColumnWidth ; }
			set { mValueColumnWidth = value ; }
		}
	}


	/// <summary>
	/// Property row within an AcHtmlTable. 
	/// </summary>
	public class TablePropertyRow
	{
		AcHtmlTable mTable ;
		AcHtmlTableRow mRow ;
		AcHtmlTableCell mPromptCell ;
		AcHtmlTableCell mValueCell ;

		public TablePropertyRow(
			AcHtmlTable InTable, string InPromptText, string InPropertyValue )
		{
			mTable = InTable ;
			PropertyTableAttributes pta = mTable.PropertyAttributes ;
			mRow = mTable.AddNewRow( ) ;

			// add the prompt text cell.
			mPromptCell = mRow.AddNewCell( ) ;

			// fill the prompt text cell.
			if ( pta.PromptColumn.WidthPct > 0 )
				mPromptCell.AddAttribute( "width", pta.PromptColumn.WidthPctString ) ; 
			mPromptCell.SetInnerText( InPromptText ) ;

			// add the property value cell.
			mValueCell = mRow.AddNewCell( ) ;

			// fill the property value cell.
			if ( pta.ValueColumn.WidthPct > 0 )
				mValueCell.AddAttribute( "width", pta.ValueColumn.WidthPctString ) ; 
			mValueCell.SetInnerText( InPropertyValue ) ;
		}
	}

}
