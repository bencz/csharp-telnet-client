using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoCoder.Ext.System
{
  public static class DateTimeExt
  {
    public static double ElapsedMilliseconds(this DateTime Time1, DateTime? Time2)
    {
      if (Time2 == null)
        return -1;
      else
      {
        var ts = Time1 - Time2.Value;
        return ts.TotalMilliseconds;
      }
    }
    public static XElement ToXElement(this DateTime Value, string Name)
    {
      return new XElement(Name, Value.ToString());
    }

    public static XElement ToXElement(this DateTime? Value, string Name)
    {
      if (Value.Value == null)
        return new XElement(Name);
      else
        return new XElement(Name, Value.Value.ToString());
    }

    /// <summary>
    /// return date that is the first day of the month of the input date.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static DateTime FirstDayOfMonthDate(this DateTime Date)
    {
      int yyyy = Date.Year;
      int mm = Date.Month;
      return new DateTime(yyyy, mm, 1);
    }

    /// <summary>
    /// return date that is the last day of the month of the input date.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static DateTime LastDayOfMonthDate(this DateTime Date)
    {
      int yyyy = Date.Year;
      int mm = Date.Month;
      int dd = DateTime.DaysInMonth(yyyy, mm);
      return new DateTime(yyyy, mm, dd);
    }

    /// <summary>
    /// return the DateTime value rounded to the closest second.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime RoundSecond(this DateTime dateTime)
    {
      if (dateTime.Millisecond > 500)
        dateTime = dateTime + TimeSpan.FromSeconds(1);

      var dttm = new DateTime(
        dateTime.Year, dateTime.Month, dateTime.Day, 
        dateTime.Hour, dateTime.Minute, dateTime.Second);
      return dttm;
    }

    public static string ToMillisecondString(this DateTime Value)
    {
      return Value.ToString("HH:mm:ss.fff");
    }

    /// <summary>
    /// return the DateTime value clipped to a whole hh:mm:ss
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime WholeSecond(this DateTime dateTime)
    {
      var dttm = new DateTime(
        dateTime.Year, dateTime.Month, dateTime.Day,
        dateTime.Hour, dateTime.Minute, dateTime.Second);
      return dttm;
    }

    public static DateTime? xTryParseDateTime(this string Text)
    {
      DateTime dtm;
      var rc = DateTime.TryParseExact(
        Text, "yyyy-MM-dd HH.mm.ss.ffff", null, DateTimeStyles.None, out dtm);
      if (rc == true)
        return dtm;
      else
        return null;
    }
  }
}
