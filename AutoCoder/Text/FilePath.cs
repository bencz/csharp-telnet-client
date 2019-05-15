using System;

namespace AutoCoder.Text
{
	/// <summary>
	/// Summary description for FilePath.
	/// </summary>
	public class AcFilePath
	{
		public static string AddTo( string InPath, string InAdd )
		{
			char slashChar = CalcSlashChar( InPath ) ;
			char[] slashChars = { slashChar } ;
			return( InPath.TrimEnd( slashChars ) + slashChar + InAdd.TrimStart( slashChars )) ;
		}

		private static char CalcSlashChar( string InPath )
		{
			char slashChar ;

			// what is the path delim char.
			int ix = InPath.IndexOf( @"\" ) ;
			if ( ix != -1 )
				slashChar = InPath[ix] ;
			else
			{
				ix = InPath.IndexOf( "/" ) ;
				if ( ix != -1 )
					slashChar = InPath[ix] ;
				else
					slashChar = '\\' ;
			}
      return slashChar; 
		}

    /// <summary>
    /// Return the file name stripped of the extension part.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public static string NameSansExtension( string InFileName )
    {
      Scanner.ScanCharResults rv = 
        Scanner.ScanReverseEqual( InFileName, '.' ) ;
      if ( rv.ResultPos < 0 )
        return InFileName ;
      else if ( rv.ResultPos == 0 )
        return "" ;
      else
        return InFileName.Substring(0,rv.ResultPos) ;
    }

		// split the file path on the last delimeter.
		public static string[] SplitEnd( string InPath )
		{
			string path1 = "" ;
			string path2 = "" ;
			char[] slashChars = { CalcSlashChar( InPath ) } ;
			string path = InPath.TrimEnd( slashChars ) ;
			int ix = path.LastIndexOf( slashChars[0] ) ;

			// the slash found starts in pos 0. 
			if ( ix == 0 )
			{
				path1 = null ;
				path2 = path.Substring( ix + 1 ) ;
			}

				// no slash found.
			else if ( ix == -1 )
			{
				path1 = path ;
				path2 = null ;
			}

				// split on the slash in middle of path.
			else
			{
				path1 = path.Substring( 0, ix ) ;
				path2 = path.Substring( ix + 1 ) ;
			}

			string[] parts = { path1, path2 } ;
			return parts ;
		}

	} // end class FilePath
}
