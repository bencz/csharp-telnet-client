using System ;
using System.Collections ;
using System.IO ;

namespace AutoCoder.Network.Mail.InMail
{
	/// <summary>
	/// Summary description for MimeAttachment.
	/// </summary>
	public class MimeAttachment
	{
		MimeMessagePart mPart ;
		string mFileName ;

		public MimeAttachment( MimeMessagePart InPart )
		{
			mPart = InPart ;
			mFileName = InPart.Properties.ContentDisposition.FileName ;
		}

		/// <summary>
		/// Save the attachment to the file.
		/// </summary>
		/// <param name="InFilePath"></param>
		public void SaveAs( string InFilePath )
		{
			// saveas depends on the ContentType of the attachment part.
			PartProperty.ContentType ct = mPart.Properties.ContentType ;
			if (( ct.Type == "text" ) && ( ct.SubType == "plain" ))
			{
				StreamWriter sw = new StreamWriter( InFilePath, false ) ;
				for( int Ix = 0 ; Ix < mPart.MessageLines.Length ; ++Ix )
				{
					string line = mPart.MessageLines[Ix] ;
					sw.WriteLine( line ) ;
				}
				sw.Close( ) ;
			}

			else
			{
				MemoryStream ms = ToMemoryStream( ) ;
				FileStream fs = new FileStream( InFilePath, FileMode.Create ) ;
				BinaryWriter w = new BinaryWriter( fs ) ;
				w.Write( ms.ToArray( )) ;
				w.Close( ) ;
				fs.Close( ) ;
			}
		}

		/// <summary>
		/// Decode the message lines of the attachment and write to a memory stream.
		/// </summary>
		/// <returns></returns>
		public MemoryStream ToMemoryStream( )
		{
			MemoryStream ms = new MemoryStream( ) ;
			if ( mPart.Properties.ContentTransferEncoding == "base64" )
			{
				foreach( string line in mPart.MessageLines )
				{
					if ( line != "" )
					{
						byte[] bytes = Convert.FromBase64String( line ) ;
						ms.Write( bytes, 0, bytes.Length ) ;
					}
				}
			}
			return ms ;
		}
	}

	/// <summary>
	/// List of mail message attachments
	/// Typical use: build list of attachments from each message part that contains 
	/// an attachment.  Then iterate thru the list. 
	/// </summary>
 	public class MimeAttachmentList : ArrayList
	{

		/// <summary>
		/// Create a new attachment, add it to the list.
		/// </summary>
		/// <param name="InPart"></param>
		/// <returns></returns>
		public MimeAttachment AddNewAttachment( MimeMessagePart InPart )
		{
			MimeAttachment attach = new MimeAttachment( InPart ) ;
			base.Add( attach ) ;
			return attach ;
		}

		/// <summary>
		/// Return an AcEnumerator positioned at the start of the list. 
		/// </summary>
		/// <returns></returns>
		public AcEnumerator BeginAttachments( )
		{
			AcEnumerator it = new AcEnumerator( GetEnumerator( )) ;
			return it ;
		}

		/// <summary>
		/// Build list of attachments in the mime message.
		/// </summary>
		/// <param name="InPartList"></param>
		/// <returns></returns>
		public MimeAttachmentList BuildList( MimeMessagePartList InPartList )
		{
			// clear the list
			Clear( ) ;

			AcEnumerator it = InPartList.BeginParts( ) ;
			for( it = InPartList.FirstPart( ) ; it.Current != null ; it.MoveNext( ))
			{
				MimeMessagePart part = (MimeMessagePart) it.Current ;
				PartProperty.ContentDisposition disp = part.Properties.ContentDisposition ;
				if (( disp != null ) && ( disp.Disposition == "attachment" ))
				{
					AddNewAttachment( part ) ;
				}
			}
			return this ; 
		}

		public AcEnumerator FirstAttachment( )
		{
			AcEnumerator it = BeginAttachments( ) ;
			it.MoveNext( ) ;
			return it ;
		}
	}

}
