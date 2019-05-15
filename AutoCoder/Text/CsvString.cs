using System ;
using System.Text ;
using System.Collections.Generic;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;
using System.Collections;

namespace AutoCoder.Text
{

	/// <summary>
	/// Comma separated value string.
	/// </summary>
	public class CsvString : IEnumerable<CsvString.Value>
	{
		public class Value
		{
			CsvString mCsvString ;
			WordCursor mCsr = null ;
			int mValueNx = -1 ;
			bool mAtFinalEmptyValue = false ;	

			public CsvString CsvString
			{
				get { return mCsvString ; }
				set { mCsvString = value ; }
			}

			public WordCursor Cursor
			{
				get { return mCsr ; }
				set { mCsr = value ; }
			}

			public void AdvanceNextValue( )
			{
				mAtFinalEmptyValue = false ;
				if ( mCsr != null )
				{
					// where is the cursor now. is it at a word, does that word have a delim.
					bool delimEndedWord = false ;
					if (( mCsr.IsAtWord == true ) 
            && ( mCsr.DelimClass == DelimClassification.DividerSymbol ))
					{
						delimEndedWord = true ;
					}

//					mCsr.AdvanceNextWord( ) ;
          mCsr = Scanner.ScanNextWord(CsvString.String, mCsr);

          if (mCsr.IsAtWord == true)
            ++mValueNx;
          else if (mCsr.IsDelimOnly == true)
            ++mValueNx;
          else if (delimEndedWord == true)
          {
            mAtFinalEmptyValue = true;
            ++mValueNx;
          }
          else
            mValueNx = -1;
				}
			}

			public void BeginValue( )
			{
				mCsr = new WordCursor( )
					.SetTraits( CsvString.mTraits )
					.SetString( CsvString.String ) ;
        mCsr.Position = RelativePosition.Begin;
				mValueNx = -1 ;
				mAtFinalEmptyValue = false ;
			}

			public Value Clone( )
			{
				Value v = new Value( ) ;
				v.mCsvString = mCsvString ;
        v.mCsr = new WordCursor(mCsr);
				v.mAtFinalEmptyValue = mAtFinalEmptyValue ;
				return v ;
			}

			public bool IsAtValue
			{
				get
				{
					if (( mCsr != null ) && ( mCsr.IsAtWord == true ))
						return true ;
          else if ((mCsr != null) && (mCsr.IsDelimOnly == true))
            return true;
          else if (mAtFinalEmptyValue == true)
						return true ;
					else
						return false ;
				}
			}

			/// <summary>
			/// advance the CsvString value cursor to the next value.
			/// </summary>
			/// <returns></returns>
			public CsvString.Value NextValue( )
			{
				CsvString.Value vlu = Clone( ) ;
				vlu.AdvanceNextValue( ) ;
				if ( vlu.IsAtValue == true )
					return vlu ;
				else
					return null ;
			}

      public Int32 ToInt32()
      {
        Int32 vlu = Int32.Parse(ToString());
        return vlu;
      }

			/// <summary>
			/// Return an object holding the CsvString value.
      /// Object can be a string, CsvString or string[].
			/// </summary>
			/// <returns></returns>
			public object ToObject( )
			{
				if ( IsAtValue == false )
					return null ;
				else if ( mAtFinalEmptyValue == true )
					return null ;
				else if ( mCsr.Word == null )
					return null ;
				else if ( mCsr.Word.Class == WordClassification.NamedBraced )
				{
					string braceName = mCsr.Word.BraceName ;
					if ( braceName == "_qv" )
					{
            string text = mCsr.Word.BracedText ;
            return Stringer.Dequote(text, mCsr.TextTraits.QuoteEncapsulation);
					}

						// a string[] is stored as a sequence of strings in CsvString form. 
					else if ( braceName == "_sa" )	// string array
					{
						CsvString vlu = new CsvString( ) ;
						vlu.String = mCsr.Word.BracedText ;
						return( vlu.ToStringArray( )) ;
					}

						// is a string of comma seperated values.
					else if ( braceName == "_csv" )
					{
						CsvString vlu = new CsvString( ) ;
						vlu.String = mCsr.Word.BracedText ;
						return vlu ;
					}

					else
					{
						return mCsr.Word.BracedText ;
					}
				}
				else
					return mCsr.Word.ToString( ) ;
			}

			/// <summary>
			/// Return the encapsulated value in string form.
			/// </summary>
			/// <returns></returns>
			public override string ToString( )
			{
        if (IsAtValue == false)
          return null;
        else if (mAtFinalEmptyValue == true)
          return null;
        else if (mCsr.Word == null)
          return "";
        else if (mCsr.Word.Class == WordClassification.NamedBraced)
        {
          string braceName = mCsr.Word.BraceName;
          if (braceName == "_qv")
          {
            string text = mCsr.Word.BracedText;
            return Stringer.Dequote(text, mCsr.TextTraits.QuoteEncapsulation);
          }
          else
          {
            return mCsr.Word.BracedText;
          }
        }
        else if (mCsr.Word.IsQuoted == true)
          return mCsr.Word.DequotedValue;
        else
          return mCsr.Word.Value ;
			}

			public int ValueNx
			{
				get
				{
					if (( mCsr != null ) && ( mCsr.IsAtWord == true ))
						return mValueNx ;
          else if ((mCsr != null) && (mCsr.IsDelimOnly == true))
            return mValueNx;
          else if (mAtFinalEmptyValue == true)
						return mValueNx ;
					else
						return -1 ;
				}
			}
		}
		// --------------------- end sub class CsvString.Value -----------------------

		StringBuilder mWip = null ;
		string mString = null ;
		TextTraits mTraits ;
		CsvString.Value mCurrentValue = null ; 
		int mValueCx = -1 ;
		bool mEncapsulateValues = true ;

		public CsvString( )
		{
      ConstructCommon();
		}

    public CsvString(string InCommaDelimitedString)
    {
      ConstructCommon();
      AddString(InCommaDelimitedString);
    }

		// ------------------------ operator[] ------------------------------
		public CsvString.Value this[ int InValueIx ]
		{
			get
			{
				CsvString.Value v = null ;
				if ( CurrentValueNx == InValueIx )
					v = CurrentValue.Clone( ) ;
				else
				{
					if (( CurrentValueNx >= InValueIx ) 
						|| ( CurrentValueNx == -1 ))
						BeginValue( ) ;
					while( true )
					{
						v = NextValue( ) ;
						if ( v == null )
							break ;
						else if ( CurrentValueNx == InValueIx )
							break ;
					}
				}
				return v ;
			}
		}

		/// <summary>
		/// add a value to the end of the string.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public CsvString Add( string InValue )
		{
			// prepare the value to be added to the CSV string.
			string prepValue ;
			if ( InValue == null )
				prepValue = "" ;
			else if (( ShouldQuoteValue( InValue ) == true )
				|| ( InValue.Length == 0 ))
			{
				prepValue = Stringer.Enquote( InValue, '"', mTraits.QuoteEncapsulation ) ;
				if ( EncapsulateValues == true )
				{
					prepValue = "_qv(" + prepValue + ")" ;
				}
			}
			else
				prepValue = InValue ;

			// add the value to the string.
			AddString( prepValue ) ;

			return this ;
		}

		/// <summary>
		/// Add an integer value to the comma sep value string.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public CsvString Add( int InValue )
		{
			AddString( InValue.ToString( )) ;
			return this ;
		}

		/// <summary>
		/// Add a boolean value to the comma sep value string.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public CsvString Add( bool InValue )
		{
			AddString( InValue.ToString( )) ;
			return this ;
		}

		/// <summary>
		/// Add an array of strings as a value to the comma sep value string. ( the array
		/// is its converted to CsvString form and enclosed in parenthesis before being added
		/// to the string.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public CsvString Add( string[] InValue )
		{
			if ( InValue == null )
			{
				string Value = null ;
				AddString( Value ) ;
			}
			else
			{
				StringBuilder sb = new StringBuilder( ) ;
				sb.Append( "_sa(" ) ;
				sb.Append( AcCommon.ToCsvString( InValue )) ;
				sb.Append( ")" ) ;
				AddString( sb.ToString( )) ;
			}
			return this ;
		}

		/// <summary>
		/// Add a comma separated value string as a value to this CsvString.  Will enclose
		/// the value in paren, then add the value as a string.   
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public CsvString Add( CsvString InValue )
		{
			if ( InValue == null )
			{
				string Value = null ;
				AddString( Value ) ; // will add an empty value.
			}
			else
			{
				StringBuilder sb = new StringBuilder( ) ;
				sb.Append( "_csv(" ) ;
				sb.Append( InValue.ToString( )) ;
				sb.Append( ")" ) ;
				AddString( sb.ToString( )) ;
			}
			return this ;
		}

		/// <summary>
		/// Add a string, as is, to the CsvString. 
		/// </summary>
		/// <param name="InValue"></param>
		public void AddString( string InValue )
		{
			// adding a value to string invalidates the current value cursor.
			mCurrentValue = null ;
			mValueCx = -1 ;

			// switch to wip mode.
			if ( mWip == null )
			{
				mWip = new StringBuilder( ) ;
				if ( mString != null )
				{
					mWip.Append( mString ) ;
					mString = null ;
				}
			}

			// add the value to the string.
			if ( mWip.Length > 0 )
				mWip.Append( ", " ) ;
			mWip.Append( InValue ) ;
		}

		/// <summary>
		/// Position at begin of sequence of comma delim values. 
		/// </summary>
		/// <returns></returns>
		public CsvString.Value BeginValue( )
		{
			mCurrentValue = new CsvString.Value( ) ;
			mCurrentValue.CsvString = this ;
			mCurrentValue.BeginValue( ) ;
			CsvString.Value vlu = mCurrentValue.Clone( ) ;
			return vlu ;
		}

    void ConstructCommon()
    {
      Empty();
      mTraits = new TextTraits();
      mTraits.DividerPatterns.AddDistinct(",", DelimClassification.DividerSymbol) ;
      mTraits.OpenNamedBracedPatterns.Replace("(", DelimClassification.OpenNamedBraced) ;
      mTraits.QuoteEncapsulation = QuoteEncapsulation.Double;
      EncapsulateValues = false;
    }

		/// <summary>
		/// get count of values in the string.
		/// </summary>
		public int Count
		{
			get
			{
				int Cx = 0 ;
				if ( mValueCx != -1 )
					Cx = mValueCx ;
				else
				{
					BeginValue( ) ;
					while( true )
					{
						mCurrentValue.AdvanceNextValue( ) ;
						if ( mCurrentValue.IsAtValue == false )
							break ;
						++Cx ;
					}
				}
				return Cx ;
			}
		}

		public CsvString.Value CurrentValue
		{
			get { return mCurrentValue ; }
		}

		public int CurrentValueNx
		{
			get
			{
				if ( mCurrentValue == null )
					return -1 ;
				else
					return mCurrentValue.ValueNx ;
			}
		}

		// --------------------------- Empty ------------------------------
		private CsvString Empty( )
		{
			mCurrentValue = null ;
			mWip = null ;
			mValueCx = -1 ;
			mString = null ;
			return this ;
		}

		public bool EncapsulateValues
		{
			get { return mEncapsulateValues ; }
			set { mEncapsulateValues = value ; }
		}

		/// <summary>
		/// return the first value in the CSV string.
		/// </summary>
		/// <returns></returns>
		public CsvString.Value FirstValue( )
		{
			BeginValue( ) ;
			return( NextValue( )) ;
		}

		public CsvString.Value NextValue( )
		{
			if ( mCurrentValue == null )
				BeginValue( ) ;
			mCurrentValue.AdvanceNextValue( ) ;
			if ( mCurrentValue.IsAtValue )
			{
				return mCurrentValue.Clone( ) ;
			}
			else
			{
				mCurrentValue = null ;
				return null ;
			}
		}

		public static bool ShouldQuoteValue( string InValue )
		{
			int Fx = InValue.IndexOfAny( new char[] {' ', '\t', '\"', ',', '('} ) ;  
			if ( Fx >= 0 )
				return true ;
			else
				return false ;
		}

		/// <summary>
		/// the string value of the CsvString.
		/// </summary>
		public string String
		{
			get
			{
				if ( mWip != null )
				{
					mString = mWip.ToString( ) ;
					mWip = null ;
				}
				if ( mString == null )
					mString = "" ;
				return mString ;
			}
			set
			{
				Empty( ) ;
				mString = value ;
			}
		}

		public override string ToString( )
		{
			return String ;
		}

		public string[] ToStringArray( )
		{
			string[] arr = new string[ Count ] ;
			CsvString.Value vlu = BeginValue( ) ;
			int Ix = 0 ;
			while( true )
			{
				vlu.AdvanceNextValue( ) ;
				if ( vlu.IsAtValue == false )
					break ;
				arr[ Ix ] = vlu.ToString( ) ;
				++Ix ;
			}
			return arr ;
 		}

    #region IEnumerable Members

    IEnumerator<CsvString.Value> IEnumerable<CsvString.Value>.GetEnumerator()
    {
      CsvString.Value vlu = null;
      BeginValue();
      while (true)
      {
        vlu = NextValue();
        if (vlu == null)
          yield break;
        else
          yield return vlu;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
