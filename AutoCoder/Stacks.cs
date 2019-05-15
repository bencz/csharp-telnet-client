using System ;
using System.Collections ;

namespace AutoCoder
{

	/// <summary>
	/// A stack of strings. Basic methods: Push, Pop, GetTop.
	/// </summary>
	public class StringStack : ArrayList
	{

		public StringStack( )
		{
		}

		// ----------------------------- properties ------------------------
		/// <summary>
		/// Stack is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return( base.Count == 0 ) ; }
		}

		/// <summary>
		/// Stack is not empty.
		/// </summary>
		public bool IsNotEmpty
		{
			get { return !IsEmpty ; }
		}

		// --------------------------- methods ----------------------------------
		/// <summary>
		/// Return the string from the top of the stack.
		/// </summary>
		/// <returns></returns>
		public string GetTop( )
		{
			int Ix = base.Count - 1 ;
			if ( Ix < 0 )
				return null ;
			else
				return (string) base[Ix] ;
		}


		/// <summary>
		/// Push a string onto the stack.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public StringStack Push( string InValue )
		{
			base.Add( InValue ) ;
			return this ;
		}

		/// <summary>
		/// Pop ( remove ) a string from the top of the stack.
		/// </summary>
		/// <returns></returns>
		public string Pop( )
		{
			int Ix = base.Count - 1 ;
			if ( Ix < 0 )
				throw( new ApplicationException( "Pop StringStack exception. Stack is empty." )) ;
			string popValue = (string) base[ Ix ] ;
			base.RemoveAt( Ix ) ;
			return popValue ;
		}
	}
}
