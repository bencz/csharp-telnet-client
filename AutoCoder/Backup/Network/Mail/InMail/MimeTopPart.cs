using System;
using System.Collections ;

namespace AutoCoder.Network.Mail.InMail
{

	// -------------------------- MimeTopPart ---------------------------
	// the top ( first ) part of the mime formated message.
	// This part has the same structure as the content parts, but it contains
	// mail message properties ( subject, from, to, ... ). The top part also contains
	// the content boundary string.
	public class MimeTopPart : MimeMessagePart
	{

		public MimeTopPart( )
		{
		}


	}
}
