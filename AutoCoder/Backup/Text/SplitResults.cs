using System;

namespace AutoCoder.Text
{
	// ------------------------ SplitResults ------------------------------
	public class SplitResults
	{
		string[] mContent ;
		string[] mDelim ;

		public SplitResults( string[] InContent, string[] InDelim )
		{
			mContent = InContent ;
			mDelim = InDelim ;
		}

		public string[] Content
		{
			get { return mContent ; }
		}
		public string[] Delim
		{
			get { return mDelim ; }
		}

		// get the length, the number of content split strings
		public int Length
		{
			get { return mContent.Length ; }
		}
	}

}
