using System ;
using System.Collections ;
using System.Collections.Specialized ;
using System.ComponentModel ;
using System.Globalization ;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail.InMail
{
	/// <summary>
	/// Summary description for MailDropMessage.
	/// </summary>
	public class MailDropMessage 
	{
		string mMessageNx ;
		string mMessageSx ;
		string mOtherListInfo ;
		string mUidl ;
		string[] mTopLines ;
		string mRawLine ;

		public MailDropMessage( )
		{
		}

		public MailDropMessage( string InListLine )
		{
			Crack( InListLine ) ;
		}

		public string MessageNx
		{
			get { return mMessageNx ; }
			set { mMessageNx = value ; }
		}
		public string MessageSx
		{
			get { return mMessageSx ; }
			set { mMessageSx = value ; }
		}
		public string OtherListInfo
		{
			get { return mOtherListInfo ; }
			set { mOtherListInfo = value ; }
		}
		public string[] TopLines
		{
			get { return mTopLines ; }
			set { mTopLines = value ; }
		}
		public string Uidl
		{
			get { return mUidl ; }
			set { mUidl = value ; }
		}

		// ------------------------- Crack ------------------------------------
		// crack the mail drop LIST line into its message number and size components.
		public MailDropMessage Crack( string InLine )
		{
			mRawLine = InLine ;

			WordCursor csr = new WordCursor( )
				.SetString( InLine ) ;

			csr = csr.NextWord( ) ;
			MessageNx = csr.Word.ToString( ) ;
			csr = csr.NextWord( ) ;
			MessageSx = csr.Word.ToString( ) ;

			return this ;
		}

		// ------------------------ FromString --------------------------
		// create a MenuOptionRow object from a comma delimeted string.
		public static MailDropMessage FromString( string InValue )
		{
			MailDropMessage msg = new MailDropMessage( ) ;
			CsvString line = new CsvString( ) ;
			line.String = InValue ;
			msg.MessageNx = line[0].ToString( )  ;
			msg.MessageSx = line[1].ToString( ) ;
			msg.OtherListInfo = line[2].ToString( ) ;
			msg.TopLines = (string[]) line[3].ToObject( ) ;
			msg.Uidl = line[4].ToString( ) ;
			return msg ;
		}

		/// <summary>
		/// MailDropMessage in comma separated value string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			return( ToCsvString( ).ToString( )) ;
		}

		/// <summary>
		/// MailDropMessage in comma separated value form.
		/// </summary>
		/// <returns></returns>
		public CsvString ToCsvString( )
		{
			CsvString line = new CsvString( ) ;
			line.Add( mMessageNx ) ;
			line.Add( mMessageSx ) ;
			line.Add( OtherListInfo ) ;
			line.Add( TopLines ) ;
			line.Add( Uidl ) ;
			return line ;
		}
	}

	// ----------------------------- MailDropMessages --------------------------
	[ TypeConverter( typeof( MailDropMessagesConverter)) ] 
	public class MailDropMessages : NameObjectCollectionBase, IEnumerable
	{
		public MailDropMessages AddMessage( MailDropMessage InMsg )
		{
			if ( InMsg.MessageNx == null )
				throw( new ApplicationException( "MailDropMessage message number not assigned" )) ;
			this.BaseAdd( InMsg.MessageNx, InMsg ) ;
			return this ;
		}

		public MailDropMessage this[ int InIx ]  
		{
			get  
			{
				MailDropMessage msg = null ;
				msg = (MailDropMessage) this.BaseGet( InIx ) ;
				return msg ;
			}
		}

		// Gets a key-and-value pair (DictionaryEntry) using an index.
		public MailDropMessage this[ string InMessageNx ]  
		{
			get  
			{
				MailDropMessage msg = null ;
				msg = (MailDropMessage) this.BaseGet( InMessageNx ) ;
				return msg ;
			}
			set  
			{
				this.BaseSet( InMessageNx, value ) ;
			}
		}

		public new virtual IEnumerator GetEnumerator( )
		{
			return BaseGetAllValues( ).GetEnumerator( ) ;
		}

		/// <summary>
		/// messages list to string.
		/// The string will 
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			object[] allValues = BaseGetAllValues( ) ;
			CsvString lines = new CsvString( ) ;
			for( int Ix = 0 ; Ix < allValues.Length ; ++Ix )
			{
				MailDropMessage msg = (MailDropMessage) allValues[Ix] ;
				lines.Add( msg.ToCsvString( )) ;
			}
			return lines.ToString( ) ;
		}

		// ------------------------ FromString --------------------------
		// create a MailDropMessages object from a comma delimeted string.
		public static MailDropMessages FromString( string InValue )
		{
			MailDropMessages msgs = new MailDropMessages( ) ;
			CsvString msgsLine = new CsvString( ) ;
			msgsLine.String = InValue ;
			CsvString.Value vlu = msgsLine.BeginValue( ) ;
			while( true )
			{
				vlu.AdvanceNextValue( ) ;
				if ( vlu.IsAtValue == false )
					break ;
				CsvString line = (CsvString) vlu.ToObject( ) ;
				MailDropMessage msg = MailDropMessage.FromString( line.ToString( )) ;
				msgs.AddMessage( msg ) ;
			}
			return msgs ;
		}
		}

	// --------------------- MailDropMessagesConverter ---------------------------
	public class MailDropMessagesConverter : TypeConverter 
	{

		// this TypeConverter can convert from string.
		public override bool CanConvertFrom(
			ITypeDescriptorContext context, 
			Type sourceType) 
		{
			if (sourceType == typeof(string)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		// convert from string to MailDropMessages 
		public override object ConvertFrom(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue ) 
		{
			if ( InValue is string ) 
			{
				return MailDropMessages.FromString( (string)InValue ) ;
			}
			return base.ConvertFrom(context, culture, InValue ) ;
		}

		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue,
			Type destinationType) 
		{  
			if (destinationType == typeof(string)) 
			{
				MailDropMessages msgs = (MailDropMessages) InValue ;
				return( msgs.ToString( )) ;
			}
			return base.ConvertTo(context, culture, InValue, destinationType);
		}
	}

}
