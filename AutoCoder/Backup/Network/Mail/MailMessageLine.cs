using System ;
using System.Text ;

namespace AutoCoder.Network.Mail
{

	// ---------------------------- MailMessageLine -----------------------------
	// A text line in a mail message.
	// Concept:
	//   Each mail message is a sequence of text lines. The lines, however, cannot
	//   be places as is in the message file. The line has to be encoded in the folowing
	//   ways:
	//      - the unicode characters are encoded in the charset of the message
	//        ( usually some single byte charset like ISO-8859-1 )
	//      - the unicode characters which are not in the set of standard ascii characters
	//        ( 33 thru 126 ), ( that is, the characters which align with every character
  //        set?? ) are encoded in Quoted-Printable form.
  //      - spaces are replaced by FOLDS when the line size exceeds 78 bytes.
  
	public class MailMessageLine
	{
		StringBuilder mInput ;

		public MailMessageLine( )
		{
		}

		public StringBuilder Input
		{
			get { return mInput ; }
		}

	}

}
