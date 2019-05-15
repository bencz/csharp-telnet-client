using System ;
using System.Collections ;
using System.Collections.Specialized;

namespace AutoCoder.Network.Mail.InMail
{

	// --------------------------- PartProperty ----------------------------
	public class PartProperty
	{
		// --------------------- Received property class -----------------------
		// Received: by 10.36.227.59 with HTTP; Sat, 25 Jun 2005 04:58:09 -0700 (PDT) 
		public class Received
		{
			public string By ;
			public string With ;
			public string Date ;
			public string Time ;
			public string TimeZone ;

			public Received(
				string InBy, string InWith, string InDate, string InTime, string InTimeZone )
			{
				By = InBy ;
				With = InWith ;
				Date = InDate ;
				Time = InTime ;
				TimeZone = InTimeZone ;
			}
		}

		// ----------------------------- Date property class ----------------------
		// Date: Sat, 25 Jun 2005 07:58:09 -0400 
		public class Date
		{
			public string DayOfWeek ;
			public string Timestamp ;
			public string TimeZone ;

			public Date( string InRawValue )
			{
			}
		}

		// ----------------------- ContentDisposition property ---------------------------
		public class ContentDisposition
		{
			public string Disposition ;
			public string FileName ;
		}

		// ----------------------- ContentType property ---------------------------
		public class ContentType
		{
			public string Type ;
			public string SubType ;
			public string CharSet ;
			public string mBoundary = null ;
			public string Name ; 

			public string Boundary
			{
				get { return mBoundary ; }
				set { mBoundary = value ; }
			}
		}
		// ---------------------- end sub classes --------------------------------


		string mName ;
		string mValue ;

		public PartProperty( )
		{
		}

		public string Name
		{
			get { return mName ; }
			set { mName = value ; }
		}

//		public PartPropertyValue Value
//		{
//			get { return new PartPropertyValue( mName, mRawValue ) ; }
//		}

		public string Value
		{
			get { return mValue ; }
			set { mValue = value ; }
		}
	}

	// ---------------------------- PartPropertyValue ----------------------------
	public class PartPropertyValue
	{
		string mName ;
		string mRawValue ;

		public PartPropertyValue( string InName, string InRawValue )
		{
			mName = InName ;
			mRawValue = InRawValue ;
		}

		public PartProperty.Received Received
		{
			get
			{
				if ( mName != "received" )
					return null ;
				else
					return( MimeParser.ParsePartProperty_Received( mRawValue )) ;
			}
		}

		public override string ToString( )
		{
			return mRawValue ; 
		}

	}

	// --------------------------- PartPropertyDictionary ------------------------------
	public class PartPropertyDictionary : NameObjectCollectionBase  
	{

		public PartPropertyDictionary( )  
		{
		}

		// ----------- properties that allow direct access to PartProperties -----------

		public string ContentTransferEncoding
		{
			get
			{
				string encodingValue = (string) this.BaseGet( "content-transfer-encoding" ) ;
				if ( encodingValue == null )
					return null ;
				else
					return encodingValue.ToLower( ) ;
			}
		}

		public PartProperty.ContentDisposition ContentDisposition
		{
			get
			{
				string propValue = (string) this.BaseGet( "content-disposition" ) ;
				if ( propValue == null )
					return null ;
				else
					return MimeParser.ParseContentDisposition( propValue ) ;
			}
		}

		public PartProperty.ContentType ContentType
		{
			get
			{
				string propValue = (string) this.BaseGet( "content-type" ) ;
				if ( propValue == null )
					return null ;
				else
					return MimeParser.ParseContentType( propValue ) ;
			}
		}

		public EmailAddress From
		{
			get
			{
				string propValue = (string) this.BaseGet( "from" ) ;
				if ( propValue == null )
					propValue = "" ;
				return( EmailAddress.ParseAddressString( propValue )) ;
			}
		}

		public string Subject
		{
			get
			{
				string propValue = (string) this.BaseGet( "subject" ) ;
				return AcCommon.StringValue( propValue ) ;
			}
		}

		public EmailAddress To
		{
			get
			{
				string propValue = (string) this.BaseGet( "to" ) ;
				if ( propValue == null )
					propValue = "" ;
				ArrayList list =
					MimeParser.ParseStringOfEmailAddresses( propValue ) ;
				if (( list != null ) && ( list.Count >= 1 ))
					return (EmailAddress) list[0] ;
				else
					return null ;
			}
		}

		// Gets or sets the value associated with the specified key.
		public string this[ string InPropertyName ]  
		{
			get  
			{
				return( (string) this.BaseGet( InPropertyName.ToLower( ))) ;
			}
			set  
			{
				this.BaseSet( InPropertyName.ToLower( ), value ) ;
			}
		}

		// Gets a key-and-value pair (DictionaryEntry) using an index.
		public PartProperty this[ int InIndex ]  
		{
			get  
			{
				PartProperty prop = new PartProperty( ) ;
				prop.Name = this.BaseGetKey( InIndex ) ;
				prop.Value = (string) this.BaseGet( InIndex ) ;
				return prop ;
			}
		}

		// Adds an entry to the collection.
		public void Add( string InPropertyName, string InPropertyValue )
		{
			string propertyName = InPropertyName.ToLower( ) ;

			// decode any encoded-word encoded values in the property value string.
			string propertyValue =
				MimeCommon.DecodeHeaderString_EncodedOnly( InPropertyValue ) ;

			this.BaseAdd( propertyName, propertyValue ) ;
		}

		// Removes an entry with the specified key from the collection.
		public void Remove( string InPropertyName )  
		{
			this.BaseRemove( InPropertyName.ToLower( )) ;
		}

		// Removes an entry in the specified index from the collection.
		public void Remove( int InIndex )  
		{
			this.BaseRemoveAt( InIndex ) ;
		}

		// Clears all the elements in the collection.
		public void Clear( )  
		{
			this.BaseClear( ) ;
		}
	}

}
