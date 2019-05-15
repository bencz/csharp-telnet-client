using System;

namespace AutoCoder.Network.Mail
{
	// ------------------------ MailEvent ----------------------------
	public enum MailEvent {
		None, Connected, Authenticated, StartDataSend, EndDataSend, Disconnected }

	// ----------------------- MailEventHandler ------------------------
	// some sort of event associated with a mail message
	public delegate void
	MailEventHandler( object o, MailEventArgs args ) ;

	// ----------------------- MailEventArgs -----------------------------
	public class MailEventArgs : EventArgs
	{
		MailEvent mEvent ;

		// ------------------------ constructor ------------------------------
		public MailEventArgs( MailEvent InEvent )
		{
			mEvent = InEvent ;
		}

		public MailEvent MailEvent
		{
			get { return mEvent ; }
		}
	}

}
