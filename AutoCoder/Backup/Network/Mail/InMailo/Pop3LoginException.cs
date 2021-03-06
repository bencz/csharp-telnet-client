using System;

namespace AutoCoder.Network.Mail.InMailo
{
	public class Pop3LoginException : Exception
	{
		private string m_exceptionString;

		public Pop3LoginException(): base()
		{
			m_exceptionString = null;
		}

		public Pop3LoginException(string exceptionString): base()
		{
			m_exceptionString = exceptionString;
		}

		public Pop3LoginException(string exceptionString,
			Exception ex) : base(exceptionString,ex)
		{
		}

		public override string ToString()
		{
			return m_exceptionString;
		}
	}
}
