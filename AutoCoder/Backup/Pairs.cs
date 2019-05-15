using System ;

namespace AutoCoder
{

	// ------------------------ ObjectPair --------------------------------
	// pair of objects
	public struct ObjectPair
	{
		public object a ;
		public object b ;

		// -------------------- constructor ------------------------
		public ObjectPair( object InObjectA, object InObjectB )
		{
			a = InObjectA ;
			b = InObjectB ;
		}
	}

	// -------------------------- BoolObjectPair ---------------------------
	public struct BoolObjectPair
	{
		public bool a ;
		public object b ;

		// ------------------------- constructor --------------------------
		public BoolObjectPair( bool InValueA, object InValueB )
		{
			a = InValueA ;
			b = InValueB ;
		}
	}

	// -------------------------- CharObjectPair ---------------------------
	public struct CharObjectPair
	{
		public char a ;
		public object b ;

		// ------------------------- constructor --------------------------
		public CharObjectPair( char InValueA, object InValueB )
		{
			a = InValueA ;
			b = InValueB ;
		}
	}

	// -------------------------- IntCharPair ---------------------------
	public struct IntCharPair
	{
		public int a ;
		public char b ;

		// ------------------------- constructor --------------------------
		public IntCharPair( int InValueA, char InValueB )
		{
			a = InValueA ;
			b = InValueB ;
		}
	}

	// -------------------------- IntStringPair ---------------------------
	public struct IntStringPair
	{
		public int a ;
		public string b ;

		public IntStringPair( int InValueA, string InValueB )
		{
			a = InValueA ;
			b = InValueB ;
		}
	}

	// -------------------------- StringPair ---------------------------
	public struct StringPair
	{
		public string a ;
		public string b ;

		public StringPair( string InValueA, string InValueB )
		{
			a = InValueA ;
			b = InValueB ;
		}
	}

}
