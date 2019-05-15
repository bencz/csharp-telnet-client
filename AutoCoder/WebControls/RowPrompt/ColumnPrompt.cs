using System;
using System.Web.UI ;

namespace AutoCoder.WebControls.RowPrompt
{

	// --------------------- ColumnPrompt -------------------------------
	public class ColumnPrompt 
//		: System.Web.UI.WebControls.WebControl, INamingContainer
	{
		string mColumnName ;
		string mCaption ;
		bool mAlwChg ;

		public ColumnPrompt( )
		{
		}

		public ColumnPrompt( string InColumnName )
		{
			ColumnName = InColumnName ;
			Caption = InColumnName ;
			AlwChg = true ;
		}

		public string ColumnName
		{
			get { return mColumnName ; }
			set { mColumnName = value ; }
		}

		public string Caption
		{
			get { return mCaption ; }
			set { mCaption = value ; }
		}

		public bool AlwChg
		{
			get { return mAlwChg ; }
			set { mAlwChg = value ; }
		}

	} // end class ColumnPrompt

} // end namespace AutoCoder.RowPrompt
