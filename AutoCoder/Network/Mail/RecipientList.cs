using System ;
using System.Collections ;
using System.Text ;

namespace AutoCoder.Network.Mail
{
	// ------------------------ RecipientList -------------------------------
	public class RecipientList : ArrayList
	{
		public RecipientList( )
		{
		}

		// --------------------------- AddRecipient ----------------------------
		public RecipientList AddRecipient( EmailAddress InAddress )
		{
			Add( InAddress ) ;
			return this ;
		}

		// --------------------------- ToMessageHeaderString -------------------------
		// the recipient list in internet message header string form.
		public string ToMessageHeaderString( string InCharSet )
		{
			StringBuilder sb = new StringBuilder( 2000 ) ;

			foreach( EmailAddress addr in this )
			{
				string addrMhs = addr.ToMessageHeaderString( InCharSet ) ;

				// rfc2822 folding rule.
				// Keep lines short, any CrLf followed by WS is considered WS. 
				if ( sb.Length > 0 )
					sb.Append( SmtpConstants.CommaFold ) ;

				sb.Append( addrMhs ) ;
			}
			return sb.ToString( ) ;
		}


	}
}
