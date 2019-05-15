using System;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder.WebControls
{
	/// <summary>
	/// Summary description for ServerControlBase.
	/// </summary>
	public class ServerControlBase : System.Web.UI.WebControls.WebControl
	{
		public ServerControlBase( )
		{
		}

		public AcHtmlLiteral AddNewLiteralControl( )
		{
			AcHtmlLiteral lit = new AcHtmlLiteral( ) ;
			Controls.Add( lit ) ;
			return lit ;
		}

		public AcHtmlSpan AddNewSpanControl( )
		{
			AcHtmlSpan lit = new AcHtmlSpan( ) ;
			Controls.Add( lit ) ;
			return lit ;
		}

		public AcHtmlTable AddNewTableControl( )
		{
			AcHtmlTable tbl = new AcHtmlTable( ) ;
			Controls.Add( tbl ) ;
			return tbl ;
		}

		// ----------------------- FindCheckBoxControl ---------------------------
		public AcHtmlInputCheckBox FindCheckBoxControl( string InFindId )
		{
			AcHtmlInputCheckBox checkControl =
				(AcHtmlInputCheckBox) FindControl( InFindId ) ;
			return checkControl ;
		}

		// ----------------------- FindLiteralControl ---------------------------
		public AcHtmlLiteral FindLiteralControl( string InFindId )
		{
			AcHtmlLiteral control =
				(AcHtmlLiteral) FindControl( InFindId ) ;
			return control ;
		}

		// ----------------------- FindInputTextControl ---------------------------
		public AcHtmlInputText FindInputTextControl( string InFindId )
		{
			AcHtmlInputText textControl =
				(AcHtmlInputText) FindControl( InFindId ) ;
			return textControl ;
		}

		// ----------------------- FindLinkButtonControl ---------------------------
		public AcAspLinkButton FindLinkButtonControl( string InFindId )
		{
			AcAspLinkButton control =
				(AcAspLinkButton) FindControl( InFindId ) ;
			return control ;
		}

		// ----------------------- FindSpanControl ---------------------------
		public AcHtmlSpan FindSpanControl( string InFindId )
		{
			AcHtmlSpan control =
				(AcHtmlSpan) FindControl( InFindId ) ;
			return control ;
		}

		// ----------------------- FindTableCellControl ---------------------------
		public AcHtmlTableCell FindTableCellControl( string InFindId )
		{
			AcHtmlTableCell control =
				(AcHtmlTableCell) FindControl( InFindId ) ;
			return control ;
		}

		// ----------------------- FindTableRowControl ---------------------------
		public AcHtmlTableRow FindTableRowControl( string InFindId )
		{
			AcHtmlTableRow control =
				(AcHtmlTableRow) FindControl( InFindId ) ;
			return control ;
		}

		// ------------------------ GetViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		public string GetViewStateValue( string InKey, string InNullValue  )
		{
			object vs = ViewState[InKey] ;
			if ( vs == null )
				return InNullValue ;
			else
				return vs.ToString( ) ;
		}

		// --------------------- GetViewStateBoolValue ---------------------
		// get boolean value from state bag.
		public bool GetViewStateBoolValue( string InKey, bool InNullValue  )
		{
			object vs = ViewState[InKey] ;
			if ( vs == null )
				return InNullValue ;
			else
			{
				string vsv = vs.ToString( ) ;
				if ( vsv == "True" )
					return true ;
				else
					return false ;
			}
		}

		// ----------------------- ToInputTextControl ---------------------------
		public AcHtmlInputText ToInputTextControl( HtmlControl InControl )
		{
			AcHtmlInputText textControl = (AcHtmlInputText) InControl ;
			return textControl ;
		}

		// ----------------------- ToCheckBoxControl ---------------------------
		public AcHtmlInputCheckBox ToCheckBoxControl( HtmlControl InControl )
		{
			AcHtmlInputCheckBox checkControl = (AcHtmlInputCheckBox) InControl ;
			return checkControl ;
		}
	}
}
