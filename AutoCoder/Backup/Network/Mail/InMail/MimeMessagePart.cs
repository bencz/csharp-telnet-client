using System;
using System.Collections ;

namespace AutoCoder.Network.Mail.InMail
{

	// ------------------------- MimeMessagePart -----------------------------
	public class MimeMessagePart
	{

		// ---------------------------- PartInProgress --------------------------------
		// message part identification in progress 
		public class PartInProgress
		{
			public MimePartCode PartCode = MimePartCode.None ;
			public int PropBx = -1 ;
			public int PropLineCx = 0 ;
			public int MessageBx = -1 ;
			public int MessageLineCx = 0 ;
			public bool CurrentlyAmoungPropertyLines = true ;
			public bool CurrentlyAmoungMessageLines = false ;
			public bool PartIsJustStarting = true ;

			public PartInProgress( MimePartCode InCode )
			{
				PartCode = InCode ;
			}

			// the in progress part has some identified lines.
			public bool HasLines
			{
				get
				{
					if (( PropBx != -1 ) || ( MessageBx != -1 ))
						return true ;
					else
						return false ;
				}
			}

			/// <summary>
			/// Add a line to the part in progress. Does not actually store the line
			/// text.  Only the bounds of the line. 
			/// </summary>
			/// <param name="InLineBx"></param>
			/// <param name="InLine"></param>
			public void AddLine( int InLineBx, string InLine )
			{
				if ( CurrentlyAmoungPropertyLines == true )
				{
					++PropLineCx ;
					if ( PropBx == -1 )
						PropBx = InLineBx ;
				}
				else
				{
					if (( PartIsJustStarting == true ) && ( InLine == "" ))
					{
					}
					else
					{
						++MessageLineCx ;
						if ( MessageBx == -1 )
							MessageBx = InLineBx ;
					}
				}
				PartIsJustStarting = false ;
			}
		}

		MimePartCode mPartCode ;
		string[] mPropertyLines = null ;
		string[] mMessageLines = null ;
		string[] mRawMessageLines = null ;
		PartPropertyDictionary mPropertyDictionary = new PartPropertyDictionary( ) ;

		// ----------------------------- MimeMessagePart -----------------------
		public MimeMessagePart( )
		{
		}

		// ------------------------------ properties ---------------------------
		public MimePartCode PartCode
		{
			get { return mPartCode ; }
			set { mPartCode = value ; }
		}
	
		// portal to the properties of the part. Use standard dictionary ["PropertyName]
		// notation, or for the common properties like From, To, Subject, ... can also use
		// ".PropertyName" notation.
		public PartPropertyDictionary Properties
		{
			get { return mPropertyDictionary ; }
		}

		public string[] PropertyLines
		{
			get { return mPropertyLines ; }
		}

		public string[] MessageLines
		{
			get { return mMessageLines ; }
		}

		public string[] RawMessageLines
		{
			get { return mRawMessageLines ; }
		}

		// ----------------------------- CountPropertyLines --------------------------
		// The property lines of the part start from the first line and continue until
		// an empty line. 
		private int CountPropertyLines( string[] InLines, int InLineBx, int InLineCx )
		{
			int propLineCx = 0 ;
			for( int Ix = 0 ; Ix < InLineCx ; ++Ix )
			{
				string line = InLines[InLineBx + Ix] ;
				if ( line == "" )
					break ;
				++propLineCx ;
			}
			return propLineCx ;
		}

		public void DecodeMessageLines( )
		{
			if ( AcCommon.StringValue( Properties.ContentTransferEncoding )
				== "quoted-printable" )
			{
				mRawMessageLines = mMessageLines ;
				mMessageLines = QuotedPrintable.DecodeLines( mRawMessageLines ) ;
			}
		}


		// ------------------------------- LoadLines -------------------------------
		/// <summary>
		/// load the lines of the part from an array of all the lines of the message.
		/// </summary>
		/// <param name="InLines"></param>
		/// <param name="InLineBx"></param>
		/// <param name="InLineCx"></param>
		/// <returns>reference to this object</returns>
		public MimeMessagePart LoadLines( string[] InLines, int InLineBx, int InLineCx )
		{
			int lineBx = InLineBx ;
			int lineCx = InLineCx ;
			string line = null ;

			// first thing.  if this is the top part, check for and trim off the initial
			// "+OK" response line from the server. ( server sends this line, followed immed
			// by the message data.  this is the first opportunity to strip it out. )
			if (( PartCode == MimePartCode.Top )
				&& ( lineCx > 0 )
				&& ( InLines[0].Length >= 3 )
				&& ( InLines[0].Substring( 0, 3 ) == "+OK" ))
			{
				++lineBx ;
				--lineCx ;
			}

			// calc the number of property lines, then calc the number of message lines.
			int propLineCx = CountPropertyLines( InLines, lineBx, lineCx ) ;
			int msgLineCx = lineCx - propLineCx ;
			int msgLineBx = lineBx + propLineCx ;

			// store the part property lines.
			mPropertyLines = new string[propLineCx] ;
			for( int Ix = 0 ; Ix < propLineCx ; ++Ix )
			{
				mPropertyLines[Ix] = InLines[ Ix + lineBx ] ;
			}

			// load the property dictionary from the property lines.
			LoadPropertyDictionary( ) ;

			// reduce the message line bounds by one in case the first line is blank.
			if ( msgLineCx > 0 )
			{
				line = InLines[msgLineBx] ;
				if ( line == "" )
				{
					msgLineCx -= 1 ;
					msgLineBx += 1 ;
				}
			}

			// store the message lines of the part.
			mMessageLines = new string[msgLineCx] ;
			for( int Ix = 0 ; Ix < msgLineCx ; ++Ix )
			{
				mMessageLines[Ix] = InLines[ Ix + msgLineBx ] ;
			}

			// the message lines are either quoted-printable or base64 encoded.
			// Decode the message lines.
			if ( AcCommon.StringValue( Properties.ContentTransferEncoding )
				== "quoted-printable" )
			{
				mRawMessageLines = mMessageLines ;
				mMessageLines = QuotedPrintable.DecodeLines( mRawMessageLines ) ;
			}

			return this ;
		}

		// ------------------------------ LoadMessageLines ----------------------------
		// load the message lines of the part.
		public void LoadMessageLines( string InStream, int InStreamBx, int InLineCx )
		{
			int Ex = InStreamBx - 1 ;
			mMessageLines = new string[ InLineCx ] ;
			for( int Ix = 0 ; Ix < InLineCx ; ++Ix )
			{
				int Bx = Ex + 1 ;
				IntStringPair rv = MimeCommon.ScanEndLine( InStream, Bx ) ;
				Ex = rv.a ;
				mMessageLines[Ix] = rv.b ;
			}
		}

		// ---------------------------- LoadPropertyDictionary --------------------
		private void LoadPropertyDictionary( )
		{
			for( int Ix = 0 ; Ix < mPropertyLines.Length ; ++Ix )
			{
				string line = mPropertyLines[ Ix ] ;
				StringPair pair = MimeParser.SplitPropertyLine( line ) ;
				mPropertyDictionary.Add( pair.a, pair.b ) ;
			}
		}

		// ------------------------------ LoadPropertyLines ----------------------------
		// load the property lines of the part.
		public void LoadPropertyLines( string InStream, int InStreamBx, int InLineCx )
		{
			int Ex = InStreamBx - 1 ;
			mPropertyLines = new string[ InLineCx ] ;
			for( int Ix = 0 ; Ix < InLineCx ; ++Ix )
			{
				int Bx = Ex + 1 ;
				IntStringPair rv = MimeCommon.ScanEndUnfoldedLine( InStream, Bx ) ;
				Ex = rv.a ;
				mPropertyLines[Ix] = rv.b ;
			}

			// load the property dictionary from the property lines.
			LoadPropertyDictionary( ) ;
		}

		// ------------------------- SetPartCode -----------------------------
		public MimeMessagePart SetPartCode( MimePartCode InPartCode )
		{
			mPartCode = InPartCode ;
			return this ;
		}
	}

	// --------------------- MimeMessagePartList ----------------------------
	public class MimeMessagePartList : ArrayList
	{

		// ----------------------- AddPart -------------------------------
		public MimeMessagePartList AddPart( MimeMessagePart InPart )
		{
			base.Add( InPart ) ;
			return this ;
		}

		// ----------------------- AddNewPart -------------------------------
		public MimeMessagePart AddNewPart( MimePartCode InPartCode )
		{
			MimeMessagePart part = null ;

			if ( InPartCode == MimePartCode.Top )
				part = new MimeTopPart( ) ;
			else
				part = new MimeMessagePart( ) ;

			part.PartCode = InPartCode ;
			base.Add( part ) ;

			return part ;
		}

		public MimeMessagePart AddNewPart(
			string InStream, MimeMessagePart.PartInProgress InPip )
		{
			MimeMessagePart part = null ;

			if ( InPip.PartCode == MimePartCode.Top )
				part = new MimeTopPart( ) ;
			else
				part = new MimeMessagePart( ) ;

			part.PartCode = InPip.PartCode ;
			base.Add( part ) ;

			// store the property lines of the part.
			if ( InPip.PropBx != -1 )
				part.LoadPropertyLines( InStream, InPip.PropBx, InPip.PropLineCx ) ;

			// store the message lines of the part.
			if ( InPip.MessageBx != -1 )
				part.LoadMessageLines( InStream, InPip.MessageBx, InPip.MessageLineCx ) ;

			// the message lines are quoted-printable encoded. decode here.
			if ( AcCommon.StringValue( part.Properties.ContentTransferEncoding )
				== "quoted-printable" )
				part.DecodeMessageLines( ) ;

			return part ;
		}

		public AcEnumerator BeginParts( )
		{
			AcEnumerator it = new AcEnumerator( GetEnumerator( )) ;
			return it ;
		}

		/// <summary>
		/// return Enumerator to the first part in the list.
		/// </summary>
		/// <returns></returns>
		public AcEnumerator FirstPart( )
		{
			return( BeginParts( ).MoveNext( )) ;
		}

		// -------------------------- GetTopPart -------------------------
		// find and return the MimePartCode.Top part in the list of parts.
		// ( this is the part that contains the message header info like subject, from, ...
		public MimeMessagePart GetTopPart( )
		{
			MimeMessagePart returnPart = null ;
			foreach( MimeMessagePart part in this )
			{
				if ( part.PartCode == MimePartCode.Top )
				{
					returnPart = part ;
					break ;
				}
			}
			return returnPart ;
		}

	}
}
