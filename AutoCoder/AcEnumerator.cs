using System ;
using System.Collections ;
using AutoCoder.Core;

namespace AutoCoder
{

	/// <summary>
	/// An IEnumerator combined with knowledge of the IEnumerator's current position. 
	/// </summary>
	public class AcEnumerator
	{
		IEnumerator mIt ;
		AcRelativePosition mRltv ;

		public AcEnumerator( )
		{
		}
		public AcEnumerator( IEnumerator InIt )
		{
			AssignResetEnumerator( InIt ) ;
		}

		/// <summary>
		/// Assign an IEnumerator positioned to the begin of the list.
		/// </summary>
		/// <param name="InIt"></param>
		/// <returns></returns>
		public AcEnumerator AssignResetEnumerator( IEnumerator InIt )
		{
			mIt = InIt ;
			mRltv = AcRelativePosition.Begin ;
			return this ;
		}

		public object Current
		{
			get
			{
				if ( mRltv != AcRelativePosition.At )
					return null ;
				else
					return mIt.Current ;
			}
		}

		/// <summary>
		/// Advance Enumerator to the next element.
		/// </summary>
		/// <returns></returns>
		public AcEnumerator MoveNext( )
		{
			if ( mRltv != AcRelativePosition.None )
			{
				bool rc = mIt.MoveNext( ) ;
				if ( rc == false )
					mRltv = AcRelativePosition.None ;
				else
					mRltv = AcRelativePosition.At ;
			}
			return this ;
		}

		public AcEnumerator Reset( )
		{
			mIt.Reset( ) ;
			mRltv = AcRelativePosition.Begin ;
			return this ;
		}
	}
}
