using AutoCoder;
using AutoCoder.Ext.System.Data;
using AutoCoder.Serialize;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using ScreenDefnLib.Enums;
using ScreenDefnLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public interface IScreenSection : IScreenItem
  {
    IList<IScreenItem> Items { get; set; }
    ScreenPurposeCode? PurposeCode { get; set; }
    string AssocSectionName { get; set; }
    int RepeatCount { get; set; }
    bool IsExpanded { get; set; }
  }

  public static class IScreenSectionExt
  {
    public static DataItemReport CaptureReport(this IScreenSection section, ScreenContent Content)
    {
      var sb = new StringBuilder();
      var sectionHeader = section as ISectionHeader;
      var sectionDim = section.CalcDim();
      var itemReport = new DataItemReport();

      IScreenLoc start = section.ScreenLoc;
      {
        start = start.NewInstance(start.RowNum, start.ColNum);
      }

      int repeatIx = 0;
      bool endOfRows = false;
      while (endOfRows == false)
      {
        if ((repeatIx > 0) && (repeatIx >= section.RepeatCount))
          break;
        repeatIx += 1;

        // section is a subfile. subfile row can be blank. If blank consider as
        // the end of rows of the subfile. So no need to match.
        bool rowIsBlank = false;
        if (section.PurposeCode == ScreenPurposeCode.ReportDetail)
        {
          if (Content.RowIsBlank(start.RowNum) == true)
          {
            rowIsBlank = true;
          }
        }

        // a blank row. no more rows to match.
        if (rowIsBlank == true)
        {
          endOfRows = true;
        }

        if (endOfRows == false)
        {
          var report = sectionHeader.CaptureToReport(start, Content);
          var combo = DataItemReport.CombineVertically(itemReport, report);
          itemReport = combo;
        }
        start.RowNum += 1;
      }

      return itemReport;
    }

    /// <summary>
    /// find the item within the screen section.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="Start"></param>
    /// <param name="FindLoc"></param>
    /// <param name="Content"></param>
    /// <returns></returns>
    public static ScreenItemInstance FindItem(
      this IScreenSection section, IScreenLoc Start, IScreenLoc FindLoc, 
      ScreenContent Content)
    {
      ScreenItemInstance foundItem = null;
      var header = section as ISectionHeader;
      var sectionDim = section.CalcDim();

      var adjRow = Start.RowNum - 1;
      var adjCol = Start.ColNum - 1;
      var start = new OneScreenLoc(
        section.ScreenLoc.RowNum + adjRow, section.ScreenLoc.ColNum + adjCol);

      int repeatIx = 0;
      bool endOfRows = false;
      int loopCx = 0;
      while (foundItem == null)
      {
        if ( section.PurposeCode != ScreenPurposeCode.ReportDetail )
        {
          if (loopCx > 0)
            break;
        }
        else
        {
          if (repeatIx >= section.GetRepeatCount())
            break;
          repeatIx += 1;
        }
        loopCx += 1;

        // section is a subfile. subfile row can be blank. If blank consider as
        // the end of rows of the subfile. So no need to match.
        bool rowIsBlank = false;
        if (section.PurposeCode == ScreenPurposeCode.ReportDetail)
        {
          if (Content.RowIsBlank(start.RowNum) == true)
          {
            rowIsBlank = true;
          }
        }

        // a blank row. no more rows to match.
        if (rowIsBlank == true)
        {
          endOfRows = true;
        }

        if (endOfRows == false)
        {
          foundItem = header.FindItem(start, FindLoc, Content);
          if (foundItem != null)
          {
            foundItem.RepeatNum = repeatIx;
            break;
          }
        }
        start.RowNum += 1;
      }

      return foundItem;
    }

    public static int GetRepeatCount(this IScreenSection section)
    {
      if (section.PurposeCode != ScreenPurposeCode.ReportDetail)
        return 0;
      else if (section.RepeatCount == 0)
        return 1;
      else
        return section.RepeatCount;
    }

    public static bool Match(this IScreenSection section, ScreenContent Content, string DebugInfo)
    {
      bool isMatch = true;
      var header = section as ISectionHeader;
      var sectionDim = section.CalcDim();

      IScreenLoc start = section.ScreenLoc;
      {
        start = start.NewInstance(start.RowNum, start.ColNum);
      }

      int repeatIx = 0;
      bool endOfRows = false;
      while (isMatch == true)
      {
        if ((repeatIx > 0) && (repeatIx >= section.RepeatCount))
          break;
        repeatIx += 1;

        // section is a subfile. subfile row can be blank. If blank consider as
        // the end of rows of the subfile. So no need to match.
        bool rowIsBlank = false;
        if (section.PurposeCode == ScreenPurposeCode.ReportDetail)
        {
          if (Content.RowIsBlank(start.RowNum) == true)
          {
            rowIsBlank = true;
          }
        }

        // a blank row. no more rows to match.
        if (rowIsBlank == true )
        {
          endOfRows = true;
        }

        if ( endOfRows == false)
        {
          isMatch = header.Match(start, Content, DebugInfo);
        }
        start.RowNum += 1;
      }

      return isMatch;
    }

    public static ScreenLocRange ToRange(this IScreenSection Section)
    {
      var range = new ScreenLocRange(Section.ScreenLoc);

      foreach (var item in Section.Items)
      {
        ScreenLocRange r1 = null;
        if (item is IScreenAtomic)
        {
          var atom = item as IScreenAtomic;
          r1 = atom.ToRange();
        }
        else if (item is IScreenSection)
        {
          var sect = item as IScreenSection;
          r1 = sect.ToRange();
        }

        var r2 = r1.ToAbsolute(Section.ScreenLoc);
        range = range.Union(r2);
      }

      return range;
    }

    public static ScreenLocRange ToRepeatRange(this IScreenSection Section)
    {
      var range = Section.ToRange();

      // apply repeat to the section range.
      if (Section.RepeatCount > 1)
      {
        range = range.ApplyRepeat(Section.RepeatCount);
      }

      return range;
    }

    public static IntPair CalcDim(this IScreenSection Section)
    {
      var r1 = Section.ToRange();
      var dim = r1.ToDim();
      return dim;
    }
    public static IntPair CalcRepeatDim(this IScreenSection Section)
    {
      var r1 = Section.ToRepeatRange();
      var dim = r1.ToDim();
      return dim;
    }
  }
}
