using System ;

namespace AutoCoder.Network
{

	// ------------------ ServerTransactionCompletionEventHandler --------------------
	public delegate void
	ServerTransactionCompletionEventHandler(
	object o, ServerTransactionCompletionInfo args ) ;

	public enum ServerTransactionCode
	{ None, Connect, Disconnect, Pop3List, Pop3Retr, Pop3Stat, Unsupported }

	/// <summary>
	/// Summary description for ServerTransactionEvent.
	/// </summary>
	public class ServerTransactionCompletionInfo
	{
		string mTransactionDescription ;
		ServerConnection mConnection ;
		Logger mResultsLogger ;
		AcReturnCode mRc = AcReturnCode.None ;
		string mReturnText ;
		System.Exception mException ;
		object mTransactionSpecificInfo ;
		ServerTransactionCode mTransactionCode ;

		public ServerTransactionCompletionInfo( )
		{
		}

		public ServerTransactionCompletionInfo(
			ServerConnection InConnect, Logger InResults )
		{
			mConnection = InConnect ;
			mResultsLogger = InResults ;
		}

		public ServerConnection ServerConnection
		{
			get { return mConnection ; }
			set { mConnection = value ; }
		}

		public System.Exception Exception
		{
			get { return mException ; }
			set { mException = value ; }
		}

		public Logger ResultsLogger
		{
			get { return mResultsLogger ; }
			set { mResultsLogger = value ; }
		}

		public ServerTransactionCode TransactionCode
		{
			get { return mTransactionCode ; }
			set { mTransactionCode = value ; }
		}

		public object TransactionSpecificInfo
		{
			get { return mTransactionSpecificInfo ; }
			set { mTransactionSpecificInfo = value ; }
		}

		public string ReturnText
		{
			get
			{
				if ( mReturnText == null )
					return "" ;
				else
					return mReturnText ;
			}
			set { mReturnText = value ; }
		}

		public AcReturnCode ReturnCode
		{
			get { return mRc ; }
			set { mRc = value ; }
		}
		public string TransactionDescription
		{
			get { return AcCommon.StringValue( mTransactionDescription ) ; }
			set { mTransactionDescription = value ; }
		}

	}
}
