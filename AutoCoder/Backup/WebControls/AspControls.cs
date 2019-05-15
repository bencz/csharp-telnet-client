using System;
using System.Web.UI.WebControls;

namespace AutoCoder.WebControls
{

	// -------------------------- AcAspLinkButton ---------------------------
	// some extentions to asp:LinkButton which allow chaining of the member functions.
	public class AcAspLinkButton : LinkButton
	{
		public AcAspLinkButton( )
			: base( )
		{
		}

		public AcAspLinkButton SetText( string InValue )
		{
			Text = InValue ;
			return this ;
		}

		public AcAspLinkButton SetID( string InValue )
		{
			ID = InValue ;
			return this ;
		}

		public AcAspLinkButton AddClickHandler( System.EventHandler InHandler )
		{
			Click += InHandler ;
			return this ;
		}
	}
}
