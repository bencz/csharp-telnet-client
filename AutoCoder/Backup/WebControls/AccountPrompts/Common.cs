using System;
using System.Text ;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AutoCoder.Web.Controls ;
using AutoCoder.WebControls.Classes ;

namespace AutoCoder.WebControls.AccountPrompts
{

	// ----------------------- AccountPromptEventHandler ------------------------
	// some sort of event associated with a menu option ( ex: the option is selected ) 
	public delegate void
	AccountPromptEventHandler( object o, AccountPromptEventArgs args ) ;

	// ----------------------- AccountPromptEventArgs -----------------------------
	// the "EventArgs" parm of a AccountPrompt event.
	public class AccountPromptEventArgs : EventArgs
	{
		public AccountPromptInfo PromptInfo = null ;
		bool mCancelClick = false ;
		bool mLoginClick = false ;
		bool mNewAccountClick = false ;
		bool mCreateAccountClick = false ;

		public bool CancelClick
		{
			get { return mCancelClick ; }
			set { mCancelClick = value ; }
		}
		public bool LoginClick
		{
			get { return mLoginClick ; }
			set { mLoginClick = value ; }
		}
		public bool NewAccountClick
		{
			get { return mNewAccountClick ; }
			set { mNewAccountClick = value ; }
		}
		public bool CreateAccountClick
		{
			get { return mCreateAccountClick ; }
			set { mCreateAccountClick = value ; }
		}
	}

	// ------------------------ AccountPromptInfo -------------------------
	public class AccountPromptInfo
	{
		string mAccountName ;
		string mAccountNbr ;
		string mConfirmPassword ;
		string mEmailAddress ;
		string mPassword ;
		bool mRememberPassword ;

		public AccountPromptInfo( )
		{
			mAccountName = null ;
			mAccountNbr = null ;
			mEmailAddress = null ;
			mPassword = null ;
			mRememberPassword = false ;
		}
		public string AccountName
		{
			get { return mAccountName ; }
			set { mAccountName = value ; }
		}
		public string AccountNbr
		{
			get { return mAccountNbr ; }
			set { mAccountNbr = value ; }
		}
		public string ConfirmPassword
		{
			get { return mConfirmPassword ; }
			set { mConfirmPassword = value ; }
		}
		public string EmailAddress
		{
			get { return mEmailAddress ; }
			set { mEmailAddress = value ; }
		}
		public string Password
		{
			get { return mPassword ; }
			set { mPassword = value ; }
		}
		public bool RememberPassword
		{
			get { return mRememberPassword ; }
			set { mRememberPassword = value ; }
		}
	} // end class AccountPromptInfo


	// ---------------------------- Common -----------------------------------
	public class Common
	{
		public Common()
		{
		}

		// ----------------------- FindCheckBoxControl ---------------------------
		public static AcHtmlInputCheckBox FindCheckBoxControl(
			WebControl InControl, string InFindId )
		{
			AcHtmlInputCheckBox checkControl =
				(AcHtmlInputCheckBox) InControl.FindControl( InFindId ) ;
			return checkControl ;
		}

		// ----------------------- FindInputTextControl ---------------------------
		public static AcHtmlInputText FindInputTextControl(
			WebControl InControl, string InFindId )
	{
			AcHtmlInputText textControl =
				(AcHtmlInputText) InControl.FindControl( InFindId ) ;
			return textControl ;
		}

		// ------------------------ GetViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		public static string GetViewStateValue(
			StateBag InViewState, string InKey, string InNullValue  )
		{
			object vs = InViewState[InKey] ;
			if ( vs == null )
				return InNullValue ;
			else
				return vs.ToString( ) ;
		}

		// --------------------- GetViewStateBoolValue ---------------------
		// get boolean value from state bag.
		public static bool GetViewStateBoolValue(
			StateBag InViewState, string InKey, bool InNullValue  )
		{
			object vs = InViewState[InKey] ;
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
		private AcHtmlInputText ToInputTextControl( HtmlControl InControl )
		{
			AcHtmlInputText textControl = (AcHtmlInputText) InControl ;
			return textControl ;
		}

		// ----------------------- ToCheckBoxControl ---------------------------
		private AcHtmlInputCheckBox ToCheckBoxControl( HtmlControl InControl )
		{
			AcHtmlInputCheckBox checkControl = (AcHtmlInputCheckBox) InControl ;
			return checkControl ;
		}



	}

}
