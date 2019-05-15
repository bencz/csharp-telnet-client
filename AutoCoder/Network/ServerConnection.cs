using System;
using System.ComponentModel ;
using System.Globalization ;
using AutoCoder.Text ;

namespace AutoCoder.Network
{
	/// <summary>
	/// Summary description for ServerConnection.
	/// </summary>
	[ TypeConverter( typeof( ServerConnectionConverter)) ] 
	public class ServerConnection
	{
		ConnectProtocol mProtocol = ConnectProtocol.None ;
		string mServerName ;
		int mPortNx ;
		bool mSecureConnect ;
		string mAccountName ;
		string mPassword ;

		public ServerConnection( )
		{
		}
		public ServerConnection(
			ConnectProtocol InProtocol, string InServerName, int InPortNx )
		{
			mProtocol = InProtocol ;
			mServerName = InServerName ;
			mPortNx = InPortNx ;
		}

		public ConnectProtocol Protocol
		{
			get { return mProtocol ; }
			set { mProtocol = value ; }
		}
		public string ServerName
		{
			get { return mServerName ; }
			set { mServerName = value ; }
		}
		public int PortNx
		{
			get { return mPortNx ; }
			set { mPortNx = value ; }
		}
		public bool SecureConnect
		{
			get { return mSecureConnect ; }
			set { mSecureConnect = value ; }
		}
		public string AccountName
		{
			get { return mAccountName ; }
			set { mAccountName = value ; }
		}
		public string Password
		{
			get { return mPassword ; }
			set { mPassword = value ; }
		}

		// ------------------------ FromString --------------------------
		// create a ServerConnection object from a comma delimeted string.
		public static ServerConnection FromString( string InValue )
		{
			ServerConnection sc = new ServerConnection( ) ;
			CsvString line = new CsvString( ) ;
			line.String = InValue ;
			sc.Protocol =
				(ConnectProtocol) Enum.Parse( typeof(ConnectProtocol), line[0].ToString( ))  ;
			sc.ServerName = line[1].ToString( ) ;
			sc.PortNx = int.Parse( line[2].ToString( )) ;
			sc.SecureConnect = bool.Parse( line[3].ToString( )) ;
			sc.AccountName = line[4].ToString( ) ;
			sc.Password = line[5].ToString( ) ;
			return sc ;
		}

		public override string ToString( )
		{
			CsvString line = new CsvString( ) ;
			line.Add( Protocol.ToString( )) ;
			line.Add( ServerName ) ;
			line.Add( PortNx ) ;
			line.Add( SecureConnect ) ;
			line.Add( AccountName ) ;
			line.Add( Password ) ;
			return line.ToString( ) ;
		}
	}

	// --------------------- ServerConnectionConverter ---------------------------
	public class ServerConnectionConverter : TypeConverter 
	{

		// this TypeConverter can convert from string.
		public override bool CanConvertFrom(
			ITypeDescriptorContext context, 
			Type sourceType) 
		{
			if (sourceType == typeof(string)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		// convert from string to ServerConnection 
		public override object ConvertFrom(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue ) 
		{
			if ( InValue is string ) 
			{
				return ServerConnection.FromString( (string)InValue ) ;
			}
			return base.ConvertFrom(context, culture, InValue ) ;
		}

		// convert from ServerConnection object to string.
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture,
			object InValue,
			Type destinationType) 
		{  
			if (destinationType == typeof(string)) 
			{
				ServerConnection sc = (ServerConnection) InValue ;
				return( sc.ToString( )) ;
			}
			return base.ConvertTo(context, culture, InValue, destinationType);
		}
	}

}
