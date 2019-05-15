using AutoCoder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Serialize
{
  /// <summary>
  /// the data definition of a data item.
  /// </summary>
  public class DataDefn
  {
    public DataDefn( DataType dataType, int length, int precision )
    {
      this.DataType = dataType;
      this.Length = length;
      this.Precision = precision;
    }
    public DataType DataType
    { get; set; }
    public int Length { get; set; }
    public int Precision { get; set; }

  }
}
