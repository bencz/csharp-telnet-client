using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  /// <summary>
  /// class that stores the column headings of a flat file.
  /// Provides a FindColumn method that returns the column number of a
  /// column name.
  /// The column number is then used as an index into an array containing the
  /// report data line split on the data seperator character ( tab ).
  /// </summary>
  public class FlatFileSchema
  {
    public FlatFileSchema(string[] HeadColumns)
    {
      this.HeadColumns = HeadColumns;
    }

    public string[] HeadColumns
    { get; set; }

    public int FindColumn(string ColumnName)
    {
      int fx = Array.IndexOf(this.HeadColumns, ColumnName);
      return fx;
    }

    /// <summary>
    /// Find the index of the column in the schema. Return the string contained
    /// in that column.
    /// </summary>
    /// <param name="Columns"></param>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    public string GetColumnString( string[] Columns, string ColumnName )
    {
      string s1 = null ;
        int ix = FindColumn(ColumnName) ;
      if ( ix == -1 )
        throw new ApplicationException("Column " + ColumnName + " is not found.") ;
      else
      {
        s1 = Columns[ix] ;
      }
      return s1 ;
    }

    public int GetColumnInt( string[] Columns, string ColumnName )
    {
        int ix = FindColumn(ColumnName) ;
      if ( ix == -1 )
        throw new ApplicationException("Column " + ColumnName + " is not found.") ;
      else
      {
        int rv ;
        string s1 = Columns[ix] ;
        if ( s1.IsNullOrEmpty( ))
          s1 = "0" ;
        bool rc = Int32.TryParse(s1, out rv) ; 
        if ( rc == false )
          throw new ApplicationException(
            "Column " + ColumnName + " data " + s1 + " is not an integer.") ;
        return rv ;
      }
    }

    public decimal GetColumnDecimal( string[] Columns, string ColumnName )
    {
        int ix = FindColumn(ColumnName) ;
      if ( ix == -1 )
        throw new ApplicationException("Column " + ColumnName + " is not found.") ;
      else
      {
        decimal rv ;
        string s1 = Columns[ix] ;
        if ( s1.IsNullOrEmpty( ))
          s1 = "0" ;
        bool rc = Decimal.TryParse(s1, out rv) ; 
        if ( rc == false )
          throw new ApplicationException(
            "Column " + ColumnName + " data " + s1 + " is not decimal.") ;
        return rv ;
      }
    }
  }
}
