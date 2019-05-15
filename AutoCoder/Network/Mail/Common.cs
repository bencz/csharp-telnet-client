using global::System;
using System.Collections ;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail
{
	
	// ------------------------- MailAddressType -------------------------------
	public enum MailAddressType
	{
		To = 1,
		Cc = 2,
		Bcc = 3
	}

	// -------------------------- MimeLineCode -----------------------------
	public enum MimeLineCode { PartBdry, PartEndBdry, Property, Text, None }

	// --------------------------- MimePartCode -----------------------------
	public enum MimePartCode { Top, Content, None }

	// ------------------------- MailException ---------------------------
	public class MailException : ApplicationException 
	{
		public MailException( string InMessage )
			:	base( InMessage )
		{
		}
		public MailException( string InMessage, Exception InCaughtExcp )
			: base( InMessage, InCaughtExcp )
		{
		}   
	}

	// ---------------------------- MimeContentException ---------------------
	public class MimeContentException : ApplicationException 
	{
		public MimeContentException( string InMessage )
			:	base( InMessage )
		{
		}
		public MimeContentException( string InMessage, Exception InCaughtExcp )
			: base( InMessage, InCaughtExcp )
		{
		}   
	}

	// ------------------------- MimeConstants -----------------------------
	public class MimeConstants
	{
		public static readonly string CommaFold = ",\r\n " ;
		public static readonly string Fold = "\r\n " ;
		public static readonly string CrLf = "\r\n" ;
		public static readonly char[] WhitespaceChars = { ' ', '\t' } ;
	}

	// ---------------------------- PopConstants ---------------------------
	public class PopConstants
	{
		public static readonly string CrLf = "\r\n" ;
		public static readonly string Ok = "+OK" ;
		public static readonly string Error = "-ERR" ;
	}

	// ------------------------- SmtpConstants -----------------------------
	public class SmtpConstants
	{
		public static readonly string CommaFold = ",\r\n " ;
		public static readonly string CrLf = "\r\n" ;
		public static readonly string Ok = "250" ;
		public static readonly string ServerReady = "220" ;
		public static readonly string Quit = "221" ;
		public static readonly string Authenticated	= "235" ;
		public static readonly string Challenge = "334" ;
		public static readonly string DataReady = "354" ;
		public static readonly string SyntaxError = "501" ;

		public static readonly string SERVER_READY = "220" ;
		public static readonly string SERVER_CHALLENGE = "334" ;
		public static readonly string SYNTAX_ERROR = "501" ;
	}

}
