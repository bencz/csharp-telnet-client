using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail.OutMailo
{
	///		from 			= new EmailAddress("support@OpenSmtp.com", "Support");
	///		to				= new EmailAddress("recipient@OpenSmtp.com", "Joe Smith");
	///		cc				= new EmailAddress("cc@OpenSmtp.com", "CC Name");
	///
	///		msg 			= new MailMessage(from, to);
	///		msg.AddRecipient(cc, AddressType.Cc);
	///		msg.AddRecipient("bcc@OpenSmtp.com", AddressType.Bcc);
	///
	///		msg.Subject 	= "Testing OpenSmtp .Net SMTP component";
	///		msg.Body 		= "Hello Joe Smith.";
	///		msg.HtmlBody 	= "<html><body>Hello Joe Smith.</body></html>";
	///
	///		// mail message properties
	///		msg.Charset		= "ISO-8859-1";
	///		msg.Priority 	= MailPriority.High;
	///		msg.Notification = true;
	///
	///		// add custom headers
	///		msg.AddCustomHeader("X-CustomHeader", "Value");
	///		msg.AddCustomHeader("X-CompanyName", "Value");
	///
	///		// add attachments
	///		msg.AddAttachment(@"..\attachments\test.jpg");
	///		msg.AddAttachment(@"..\attachments\test.htm");
	public class MailMessage
	{
		EmailAddress mFrom ;
		EmailAddress mReplyTo ;
		RecipientList mToRecipients ;
		RecipientList mCcRecipients ;
		RecipientList mBccRecipients ;
		internal ArrayList				attachments;
		internal string					subject;
		internal string					body;
		internal string					htmlBody;
		internal string					mixedBoundary;
		internal string					altBoundary;
		internal string					relatedBoundary;
		internal string					charset = "ISO-8859-1";
		internal bool					notification;
		internal string					priority;
		internal ArrayList				customHeaders;
		internal ArrayList				images;

		// ------------------------- constructors ----------------------------
		public MailMessage()
		{
			ConstructCommon( ) ;
		}

		public MailMessage( EmailAddress InFrom, EmailAddress InTo )
		{
			ConstructCommon( ) ;
			mFrom = InFrom ;
			mToRecipients.Add( InTo ) ;
		}

		public MailMessage( string InFrom, string InTo )
		{
			ConstructCommon( ) ;
			mFrom = new EmailAddress( InFrom ) ;
			mToRecipients.Add( new EmailAddress( InTo )) ;
		}

		// -------------------------- Properties --------------------------
		// reply to address.  if not spcfd, use the from address.
		public EmailAddress ReplyTo
		{
			get
			{
				if ( mReplyTo == null )
					return mFrom ;
				else
					return mReplyTo ;
			}
			set { mReplyTo = value ; }
		}

		public EmailAddress From
		{
			get { return mFrom ; }
			set { mFrom = value ; }
		}

		public RecipientList ToRecipients
		{
			get { return mToRecipients ; }
			set { mToRecipients = value; }
		}

		/// <value>Stores the subject of the MailMessage</value>
		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		/// <value>Stores the text body of the MailMessage</value>
		public string Body
		{
			get { return body; }
			set { body = value; }
		}

		/// <value>Stores the HTML body of the MailMessage</value>
		public string HtmlBody
		{
			get { return htmlBody; }
			set { htmlBody = value; }
		}

		/// <value>Stores Mail Priority value</value>
		/// <seealso>MailPriority</seealso>
		public string Priority
		{
			get { return priority; }
			set { priority = value; }
		}


		/// <value>Stores the Read Confirmation Reciept</value>
		public bool Notification
		{
			get { return notification; }
			set { notification = value; }
		}

		public RecipientList CCRecipients
		{
			get { return mCcRecipients ; }
			set { mCcRecipients = value ; }
		}

		public RecipientList BCCRecipients
		{	get { return mBccRecipients ; }
			set { mBccRecipients = value ; }
		}

		/// <value>Stores the character set of the MailMessage</value>
		public string Charset
		{
			get { return charset; }
			set { charset = value; }
		}

		/// <value>Stores a list of Attachments</value>
		public ArrayList Attachments
		{
			get { return attachments; }
			set { attachments = value; }
		}

		/// <value>Stores a NameValueCollection of custom headers</value>
		public ArrayList CustomHeaders
		{
			get { return customHeaders; }
			set { customHeaders = value; }
		}

		/// <value>Stores the string boundary used between MIME parts</value>
		internal string AltBoundary
		{
			get { return altBoundary; }
			set { altBoundary = value; }
		}

         /// <value>Stores the string boundary used between MIME parts</value>
         internal string MixedBoundary
         {
             get { return mixedBoundary; }
             set { mixedBoundary = value; }
         }

		// ------------------------ AddRecipient ----------------------------------
		public void AddRecipient( EmailAddress InAddress, MailAddressType InType )
		{
			try
			{
				switch( InType )
				{
					case MailAddressType.To:
						mToRecipients.AddRecipient( InAddress ) ;
						break;

					case MailAddressType.Cc:
						CCRecipients.AddRecipient( InAddress ) ;
						break ;

					case MailAddressType.Bcc:
						BCCRecipients.AddRecipient( InAddress ) ;
						break ;
				}
			}
			catch(Exception e)
			{
				throw new MailException( "Exception in AddRecipient: " + e.ToString( )) ;
			}
		}

		public void AddRecipient( string InAddress, MailAddressType InType)
		{
			EmailAddress email = new EmailAddress( InAddress ) ;
			AddRecipient( email, InType ) ;
		}


		/// <summary>Adds an Attachment to the MailMessage using a file path</summary>
		/// <param name="filepath">File path to the file you want to attach to the MailMessage</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.AddAttachment(@"C:\file.jpg");
		/// </code>
		/// </example>
		// start added/modified by mb
		public void AddAttachment(string filepath)
		{
			AddAttachment(filepath, NewCid());
		}

		/// <summary>Adds an included image to the MailMessage using a file path</summary>
		/// <param name="filepath">File path to the file you want to attach to the MailMessage</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.AddImage(@"C:\file.jpg");
		/// </code>
		/// </example>
		// start added/modified by mb
		public void AddImage(string filepath)
		{
			AddImage(filepath, NewCid());
		}

		public void AddImage(string filepath, string cid) 
		{
			if (filepath != null)
			{
				Attachment image = new Attachment(filepath);
				image.contentid=cid;
				images.Add(image);
			}			
		}
	
	
		/// <summary>
		/// Generate a content id
		/// </summary>
		/// <returns></returns>
		private string NewCid() 
		{
			int attachmentid=attachments.Count+images.Count+1;
			return "att"+attachmentid;
		}

		public void AddAttachment(string filepath, string cid) 
		{
			if (filepath != null)
			{
				Attachment attachment = new Attachment(filepath);
				attachment.contentid=cid;
				attachments.Add(attachment);
			}			
		}
		// end added by mb

		/// <summary>Adds an Attachment to the MailMessage using an Attachment instance</summary>
		/// <param name="attachment">Attachment you want to attach to the MailMessage</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		Attachment att = new Attachment(@"C:\file.jpg");
		///		msg.AddAttachment(att);
		/// </code>
		/// </example>
		public void AddAttachment(Attachment attachment)
		{
			if (attachment != null)
			{
                 attachments.Add(attachment);
             }
		}

		/// <summary>Adds an Attachment to the MailMessage using a provided Stream</summary>
		/// <param name="stream">stream you want to attach to the MailMessage</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		Attachment att = new Attachment(new FileStream(@"C:\File.jpg", FileMode.Open, FileAccess.Read), "Test Name");
		///		msg.AddAttachment(att);
		/// </code>
		/// </example>
		public void AddAttachment(Stream stream)
		{
			if (stream != null)
			{
                 attachments.Add(stream);
             }
		}


		/// <summary>
		/// Adds an custom header to the MailMessage.
		///	NOTE: some SMTP servers will reject improper custom headers
		///</summary>
		/// <param name="name">Name of the custom header</param>
		/// <param name="body">Value of the custom header</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.AddCustomHeader("X-Something", "HeaderValue");
		/// </code>
		/// </example>
		public void AddCustomHeader(string name, string body)
		{
			if (name != null && body != null)
			{
                 AddCustomHeader(new MailHeader(name, body));
             }
		}

		/// <summary>
		/// Adds an custom header to the MailMessage.
		///	NOTE: some SMTP servers will reject improper custom headers
		///</summary>
		/// <param name="mailheader">MailHeader instance</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		MailHeader header = new MailHeader("X-Something", "HeaderValue");		
		///		msg.AddCustomHeader(header);
		/// </code>
		/// </example>
		public void AddCustomHeader(MailHeader mailheader)
		{
			if (mailheader.name != null && mailheader.body != null)
			{
                 customHeaders.Add(mailheader);
             }
		}

		// --------------------------- ConstructCommon -----------------------
		private void ConstructCommon( )
		{
			mToRecipients = new RecipientList( ) ;
			mCcRecipients = new RecipientList( ) ;
			mBccRecipients = new RecipientList( ) ;
			attachments = new ArrayList();
			images = new ArrayList();
			customHeaders = new ArrayList();
			mixedBoundary = MailMessage.generateMixedMimeBoundary( ) ;
			altBoundary = MailMessage.generateAltMimeBoundary( ) ;
			relatedBoundary = MailMessage.generateRelatedMimeBoundary( ) ;
		}

		/// <summary>Populates the Body property of the MailMessage from a text file</summary>
		/// <param name="filePath">Path to the file containing the MailMessage body</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.GetBodyFromFile(@"C:\body.txt");
		/// </code>
		/// </example>
		public void GetBodyFromFile(string filePath)
		{
			this.body = GetFileAsString(filePath);
		}

		/// <summary>Populates the HtmlBody property of the MailMessage from a HTML file</summary>
		/// <param name="filePath">Path to the file containing the MailMessage html body</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.GetHtmlBodyFromFile(@"C:\htmlbody.html");
		/// </code>
		/// </example>
		public void GetHtmlBodyFromFile(string filePath)
		{
			// add extension
			this.htmlBody = GetFileAsString(filePath);
		}

		/// <summary>Resets all of the MailMessage properties</summary>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.Reset();
		/// </code>
		/// </example>
		public void Reset( )
		{
			mFrom = null;
			mReplyTo = null;
			mToRecipients.Clear( ) ;
			mCcRecipients.Clear( ) ;
			mBccRecipients.Clear( ) ;
			customHeaders.Clear();
			attachments.Clear();
			subject = null;
			body = null;
			htmlBody = null;
			priority = null;
			mixedBoundary = null;
			altBoundary = null;
			charset = null;
			notification = false;
		}

		/// <summary>Saves the MailMessage as a RFC 822 formatted message</summary>
		/// <param name="filePath">Specifies the file path to save the message</param>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", recipient@OpenSmtp.com");
		///		msg.Body = "body";
		///		msg.Subject = "subject";
		///		msg.Save(@"C:\email.txt");
		/// </code>
		/// </example>
		public void Save(string filePath)
		{
			StreamWriter sw = System.IO.File.CreateText(filePath);
			sw.Write(this.ToString());
			sw.Close();
		}

		/// <summary>Returns the MailMessage as a RFC 822 formatted message</summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder( 60000 ) ;

			sb.Append( "Reply-To: " + ReplyTo.ToMessageHeaderString( charset )) ;
			sb.Append( SmtpConstants.CrLf ) ;
			sb.Append( "From: " + From.ToMessageHeaderString( charset )) ;
			sb.Append( SmtpConstants.CrLf ) ;

			if ( 1 == 2 )
			{
				if ( Stringer.IsNotEmpty( ReplyTo.FriendlyName ))
				{
					string rt = MessageHeaderString.EncodeAsRequired( 
						Stringer.Enquote( ReplyTo.FriendlyName, '"', QuoteEncapsulation.Escape ),
						new QuotedPrintableTraits( )) ;
					sb.Append( "Reply-To: " ) ;
					sb.Append( rt ) ;
					sb.Append( " <" + ReplyTo.Address + ">" + SmtpConstants.CrLf ) ;
					string debug = sb.ToString( ) ;
				}
				else
				{
					sb.Append("Reply-To: <" + ReplyTo.Address + ">" + SmtpConstants.CrLf ) ;
				}

			// from email address.
			if ( Stringer.IsNotEmpty( From.FriendlyName ))
			{
				string rt = MessageHeaderString.EncodeAsRequired( 
					Stringer.Enquote( From.FriendlyName, '"', QuoteEncapsulation.Escape ),
					new QuotedPrintableTraits( )) ;
				sb.Append( "From: " ) ;
				sb.Append( rt ) ;
				sb.Append( " <" + From.Address + ">" + SmtpConstants.CrLf ) ;
			}
			else
				sb.Append("From: <" + From.Address + ">" + SmtpConstants.CrLf ) ;
			}

			sb.Append("To: " + CreateAddressList( mToRecipients ) + "\r\n");

			if ( CCRecipients.Count != 0 )
			{
                 sb.Append("CC: " + CreateAddressList( CCRecipients ) + "\r\n");
             }

			if (subject != null && subject.Length > 0)
             {
				StringBuilder cleanSubject = new StringBuilder(subject);
				cleanSubject.Replace("\r\n", null);
				cleanSubject.Replace("\n", null);
                
				sb.Append("Subject: " + MailEncoder.ConvertHeaderToQP(cleanSubject.ToString(), charset) + "\r\n");
             }

			sb.Append("Date: " + DateTime.Now.ToUniversalTime().ToString("R") + "\r\n");
			sb.Append(SmtpConfig.X_MAILER_HEADER + "\r\n");

			if (notification)
			{
				if (ReplyTo.FriendlyName != null && ReplyTo.FriendlyName.Length != 0)
                 {
                     sb.Append("Disposition-Notification-To: " + MailEncoder.ConvertHeaderToQP(ReplyTo.FriendlyName, charset) + " <" + ReplyTo.Address + ">\r\n");
                 }
				else
				{
                     sb.Append("Disposition-Notification-To: <" + ReplyTo.Address + ">\r\n");
                 }
			}

			if (priority != null)
			{
                 sb.Append("X-Priority: " + priority + "\r\n");
             }

			if (customHeaders != null)
			{

				for (IEnumerator i = customHeaders.GetEnumerator(); i.MoveNext();)
				{
					MailHeader m = (MailHeader)i.Current;

					if (m.name.Length >= 0 && m.body.Length >= 0)
					{
						sb.Append(m.name + ":" + MailEncoder.ConvertHeaderToQP(m.body, charset) + "\r\n");
					}
					else
					{
						// TODO: Check if below is within RFC spec.
						sb.Append(m.name + ":\r\n");
					}

				}
			}

             sb.Append("MIME-Version: 1.0\r\n");
             sb.Append(GetMessageBody());

			return sb.ToString();
		}

		/// <summary>Returns a clone of this message</summary>
		/// <example>
		/// <code>
		/// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
		///		msg.Body = "body";
		///		msg.Subject = "subject";
		///
		///		msg2 = msg.Copy();
		/// </code>
		/// </example>
		/// <returns>Mailmessage</returns>
		public MailMessage Copy()
		{
			return (MailMessage)this.MemberwiseClone();
		}

		/// Internal/Private methods below
		// ------------------------------------------------------

		private string GetFileAsString(string filePath)
		{
			FileStream 	fin 	= new FileStream(filePath, FileMode.Open, FileAccess.Read);
			byte[] 		bin		= new byte[fin.Length];
			long 		rdlen	= 0;
			int len;

			StringBuilder sb = new StringBuilder();

			while(rdlen < fin.Length)
			{
				len = fin.Read(bin, 0, (int)fin.Length);
				sb.Append(Encoding.UTF7.GetString(bin, 0, len));
				rdlen = (rdlen + len);
			}

			fin.Close();
			return sb.ToString();
		}


		/// <summary>
		/// Determines the format of the message and adds the
		/// appropriate mime containers.
		/// 
		/// This will return the html and/or text and/or 
		/// embedded images and/or attachments.
		/// </summary>
		/// <returns></returns>
		private String GetMessageBody() 
		{
			StringBuilder sb=new StringBuilder();

			if (attachments.Count>0) 
			{
				sb.Append("Content-Type: multipart/mixed;");
				sb.Append("boundary=\"" + mixedBoundary + "\"");
				sb.Append("\r\n\r\nThis is a multi-part message in MIME format.");
				sb.Append("\r\n\r\n--" + mixedBoundary + "\r\n");
			}

			sb.Append(GetInnerMessageBody());

			if (attachments.Count>0) 
			{
				foreach (Attachment attachment in attachments) 
				{
					sb.Append("\r\n\r\n--" + mixedBoundary + "\r\n");
					sb.Append(attachment.ToMime());
				}
				sb.Append("\r\n\r\n--" + mixedBoundary + "--\r\n");
			}
			return sb.ToString();

		}

		/// <summary>
		/// Get the html and/or text and/or images.
		/// </summary>
		/// <returns></returns>

		private string GetInnerMessageBody()
		{
			StringBuilder sb=new StringBuilder();
			if (images.Count > 0)
			{
				sb.Append("Content-Type: multipart/related;");
				sb.Append("boundary=\"" + relatedBoundary + "\"");
				sb.Append("\r\n\r\n--" + relatedBoundary + "\r\n");
			}

			sb.Append(GetReadableMessageBody());

			if (images.Count > 0)
			{
				foreach (Attachment image in images) 
				{
					sb.Append("\r\n\r\n--" + relatedBoundary + "\r\n");
					sb.Append(image.ToMime());
				}
				sb.Append("\r\n\r\n--" + relatedBoundary + "--\r\n");
			}
			return sb.ToString();
		}

		private String GetReadableMessageBody() {

			StringBuilder sb=new StringBuilder();

			if (htmlBody == null)
			{
				sb.Append(GetTextMessageBody(body, "text/plain"));
			}
			else if (body == null)
			{
				sb.Append(GetTextMessageBody(htmlBody, "text/html"));
			}
			else
			{
				sb.Append(GetAltMessageBody(
					GetTextMessageBody(body, "text/plain"),
					GetTextMessageBody(htmlBody, "text/html")));
			}

			return sb.ToString();

		}


         private string GetTextMessageBody(string messageBody, string textType)
         {
             StringBuilder sb = new StringBuilder();

             sb.Append("Content-Type: " + textType + ";\r\n");
             sb.Append(" charset=\""+ charset + "\"\r\n");
             sb.Append("Content-Transfer-Encoding: quoted-printable\r\n\r\n");
             sb.Append(MailEncoder.ConvertToQP(messageBody, charset));

             return sb.ToString();
         }

         private string GetAltMessageBody(string messageBody1, string messageBody2)
         {
             StringBuilder sb = new StringBuilder();

             sb.Append("Content-Type: multipart/alternative;");
             sb.Append("boundary=\"" + altBoundary + "\"");

             sb.Append("\r\n\r\nThis is a multi-part message in MIME format.");

             sb.Append("\r\n\r\n--" + altBoundary + "\r\n");
             sb.Append(messageBody1);
             sb.Append("\r\n\r\n--" + altBoundary + "\r\n");
             sb.Append(messageBody2);
             sb.Append("\r\n\r\n--" + altBoundary + "--\r\n");

             return sb.ToString();
         }


		// creates comma separated address list from to: and cc:
		private string CreateAddressList(ArrayList msgList)
		{
			StringBuilder sb = new StringBuilder();

			int index = 1;
			int msgCount = msgList.Count;

			for (IEnumerator i = msgList.GetEnumerator(); i.MoveNext(); index++)
			{
				EmailAddress a = (EmailAddress)i.Current;

				// if the personal name exists, add it to the address sent. IE: "Ian Stallings" <istallings@mail.com>
				if (a.FriendlyName != null && a.FriendlyName.Length > 0)
				{					
					sb.Append("\"" + MailEncoder.ConvertHeaderToQP(a.FriendlyName, charset) + "\" <" + a.Address + ">");
				}
				else
				{
					sb.Append("<" + a.Address + ">");
				}

				// if it's not the last address add a semi-colon:
				if (msgCount != index)
				{
					sb.Append(",");
				}
			}

			return sb.ToString();
		}

		private static string generateMixedMimeBoundary()
		{
			// Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
			return "Part." + Convert.ToString(new Random(unchecked((int)DateTime.Now.Ticks)).Next()) + "." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.Ticks)).Next());
		}

         private static string generateAltMimeBoundary()
         {
             // Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
             return "Part." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next()) + "." + Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next());
         }

		private static string generateRelatedMimeBoundary()
		{
			// Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
			return "Part." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(2).Ticks)).Next()) + "." + Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next());
		}

     }
}

