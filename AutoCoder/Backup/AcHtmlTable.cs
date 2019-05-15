using System;
using System.IO;
using System.Text ;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls ;
using System.Web.UI.HtmlControls;
using AutoCoder.Web.HtmlControls ;

namespace AutoCoder.Web.Controls
{

	// -------------------------- AcControl -----------------------------
	// class holding a series of static methods that relate to WebControls
	public class AcControl
	{
		public static string RenderControlToString( System.Web.UI.Control InControl )
		{
			StringBuilder sb = new StringBuilder( 2000 ) ;
			StringWriter sw = new StringWriter( sb ) ;
			HtmlTextWriter tw = new HtmlTextWriter( sw ) ;
			InControl.RenderControl( tw ) ;
			return sb.ToString( ) ;
		}
	}

	// ---------------------- AcTable --------------------------------
	public class AcTable : System.Web.UI.WebControls.Table
	{
	
		public AcTableRow AddNewRow( )
		{
			AcTableRow row = new AcTableRow( ) ;
			Rows.Add( row ) ;
			return row ;
		}

		public AcTable AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ; 
		}

		public AcTable AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcTable SetBorderColor( System.Drawing.Color InColor )
		{
			BorderColor = InColor ;
			return this ;
		}

		public AcTable SetBorderColorRgb( int InRed, int InGreen, int InBlue )
		{
			BorderColor = System.Drawing.Color.FromArgb( InRed, InGreen, InBlue ) ;
			return this ;
		}

		public AcTable SetBorderStyle( BorderStyle InStyle )
		{
			BorderStyle = InStyle ;
			return this ;
		}

		public AcTable SetBorderWidth( int InWidth )
		{
			BorderWidth = InWidth ;
			return this ;
		}

		public AcTable SetCellPadding( int InPadding )
		{
			CellPadding = InPadding ;
			return this ;
		}

		public AcTable SetCellSpacing( int InSpacing )
		{
			CellSpacing = InSpacing ;
			return this ;
		}

		public AcTable SetHorizontalAlign( HorizontalAlign InAlign )
		{
			HorizontalAlign = InAlign ;
			return this ;
		}

		public AcTable SetWidth( Unit InUnits )
		{
			Width = InUnits ;
			return this ;
		}

		public AcTable SetWidthPixel( int InPixels )
		{
			Width = Unit.Pixel( InPixels ) ;
			return this ;
		}

	}

	// ----------------------- AcTableCell ----------------------------
	public class AcTableCell : System.Web.UI.WebControls.TableCell
	{
		public AcTableCell( )
		{
		}

		public AcTableCell( string InText )
		{
			Text = InText ;
		}
	
		public AcDivControl AddNewDivControl( )
		{
			AcDivControl div = new AcDivControl( ) ;
			Controls.Add( div ) ;
			return div ;
		}

		public AcOlControl AddNewOlControl( )
		{
			AcOlControl ol = new AcOlControl( ) ;
			Controls.Add( ol ) ;
			return ol ;
		}

		public AcTable AddNewTable( )
		{
			AcTable table = new AcTable( ) ;
			Controls.Add( table ) ;
			return table ;
		}

		public AcTableCell AddAttribute( string InKey, string InValue )
		{
			Attributes.Add( InKey, InValue ) ;
			return this ; 
		}

		public AcTableCell AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcTableCell SetFontBold( bool InBold )
		{
			Font.Bold = InBold ;
			return this ;
		}

		public AcTableCell SetFontName( string InName )
		{
			Font.Name = InName ;
			return this ;
		}

		public AcTableCell SetFontSize( FontUnit InUnit )
		{
			Font.Size = InUnit ;
			return this ;
		}

		public AcTableCell SetForeColor( System.Drawing.Color color )
		{
			ForeColor = color ;
			return this ;
		}

		public AcTableCell SetText( string InText )
		{
			Text = InText ;
			return this ; 
		}

		public AcTableCell SetWidth( Unit InUnits )
		{
			Width = InUnits ;
			return this ;
		}

		public AcTableCell SetWidthPct( double InPct )
		{
			Width = Unit.Percentage( InPct ) ;
			return this ;
		}

		public AcTableCell SetWidthPixel( int InPixels )
		{
			Width = Unit.Pixel( InPixels ) ;
			return this ;
		}

		public AcTableCell SetWidthPoint( int InPoints )
		{
			Width = Unit.Point( InPoints ) ;
			return this ;
		}
	}

	// ----------------------- AcTableRow --------------------------
	public class AcTableRow : System.Web.UI.WebControls.TableRow 
	{
		public AcTableRow( )
		{
		}

		public AcTableCell AddCell( AcTableCell InCell )
		{
			Cells.Add( InCell ) ;
			return InCell ;
		}
	
		public AcTableCell AddNewCell( )
		{
			AcTableCell cell = new AcTableCell( ) ;
			Cells.Add( cell ) ;
			return cell ;
		}
	
		public AcTableCell AddNewCell( string InText )
		{
			AcTableCell cell = new AcTableCell( ) ;
			cell.Text = InText ;
			Cells.Add( cell ) ;
			return cell ;
		}

		public AcTableRow AddStyle( string InKey, string InValue )
		{
			Style.Add( InKey, InValue ) ;
			return this ;
		}

		public AcTableRow SetForeColor( System.Drawing.Color color )
		{
			ForeColor = color ;
			return this ;
		}

		public AcTableRow SetFontBold( bool InBold )
		{
			Font.Bold = InBold ;
			return this ;
		}

		public AcTableRow SetFontName( string InName )
		{
			Font.Name = InName ;
			return this ;
		}

		public AcTableRow SetFontSize( FontUnit InUnit )
		{
			Font.Size = InUnit ;
			return this ;
		}


	}

} // end namespace AutoCoder.Controls.Web

