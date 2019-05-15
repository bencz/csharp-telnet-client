using System ;
using AutoCoder.Core;

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

  // -------------------------- AcPair ---------------------------
  public class AcPair<A, B>
  {
    public A a;
    public B b;

    public AcPair(A InValueA, B InValueB)
    {
      a = InValueA;
      b = InValueB;
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

  // ------------------------ BytePair ---------------------------
  public struct BytePair
  {
    public byte a;
    public byte b;

    public BytePair(byte InValueA, byte InValueB)
    {
      a = InValueA;
      b = InValueB;
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

  // -------------------------- IntPair ---------------------------
  public class IntPair
  {
    public int a;
    public int b;

    // ------------------------- constructor --------------------------
    public IntPair(int InValueA, int InValueB)
    {
      a = InValueA;
      b = InValueB;
    }
  }

  // --------------------------- CheckedObjectPair --------------------------
  public class CheckedObjectPair<T>
  {
    public CheckResult a;
    public T b;

    public CheckedObjectPair( )
    {
      a = CheckResult.NotFound;
      b = default(T);
    }

    public CheckedObjectPair(CheckResult InCheckResult, T InObject)
    {
      a = InCheckResult;
      b = InObject;;
    }
  }

  // ------------------------ DecimalPair ---------------------------
  public struct DecimalPair
  {
    public decimal a;
    public decimal b;

    public DecimalPair(decimal InValueA, decimal InValueB)
    {
      a = InValueA;
      b = InValueB;
    }
  }

  // --------------------------- ErrorObjectPair --------------------------
  public class ErrorObjectPair<T>
  {
    public ErrorResult a;
    public T b;

    public ErrorObjectPair()
    {
      a = ErrorResult.OK ;
      b = default(T);
    }

    public ErrorObjectPair(ErrorResult InResult, T InObject)
    {
      a = InResult;
      b = InObject; ;
    }
  }

  // -------------------------- IntObjectPair ---------------------------
  public class IntObjectPair<T>
  {
    public int a;
    public T b;

    public IntObjectPair(int InValueA, T InValueB)
    {
      a = InValueA;
      b = InValueB;
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

  public class StringBoolStringBool
  {
    string String1 { get; set; }
    bool Bool1 { get; set; }
    string String2 { get; set; }
    bool Bool2 { get; set; }

    public StringBoolStringBool(
      string InString1, bool InBool1,
      string InString2, bool InBool2)
    {
      String1 = InString1;
      Bool1 = InBool1;
      String2 = InString2;
      Bool2 = InBool2;
    }
  }

}
