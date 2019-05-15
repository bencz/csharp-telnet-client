using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using AutoCoder.File;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace AutoCoder.Print
{
  /// <summary>
  /// Provides static methods used to print text lines to a printer. 
  /// </summary>
  public class LinePrinter
  {
    Font mPrinterFont;
    string[] mLinesToPrint = null;
    int mCurrentLineNx = 0;

    /// <summary>
    /// LinePrinter class constructor. Does not have to be public since 
    /// access to the classes functionality is thru its static methods. 
    /// </summary>
    LinePrinter()
    {
    }

    /// <summary>
    /// Print the array of strings to the default printer.
    /// </summary>
    /// <param name="InLines"></param>
    public static void PrintLines(IEnumerable<string> Lines, string Title = "", int TabSize = 2 )
    {
      PrintDialog printDialog = new PrintDialog();

      if ((bool)printDialog.ShowDialog().GetValueOrDefault())
      {
        FlowDocument flowDoc = new FlowDocument();
        flowDoc.ColumnWidth = printDialog.PrintableAreaWidth;
        flowDoc.PagePadding = new Thickness(25);

        var fontFamily = new System.Windows.Media.FontFamily("Lucida console");
        double pointSize = 12;

        foreach (string line in Lines)
        {
          var para = new Paragraph();
          para.Margin = new Thickness(0);
          para.FontFamily = fontFamily;
          para.FontSize = pointSize;
          para.Inlines.Add(new Run(line));
          flowDoc.Blocks.Add(para);
        }

        DocumentPaginator paginator = 
          (flowDoc as IDocumentPaginatorSource).DocumentPaginator;

        printDialog.PrintDocument(paginator, Title);
      }
    }

    public static void PrintLines(string[] Lines, Font Font, int TabSize = 2)
    {
      LinePrinter lp = new LinePrinter();

      // expand tab chars

      lp.PrintLines_Actual(Lines, Font );
    }

    // The Click event is raised when the user clicks the Print button.
    public static void PrintFile(string TextFilePath, int TabSize = 2 )
    {
      string title = "";
      string[] lines = System.IO.File.ReadAllLines(TextFilePath);
      PrintLines(lines, title, TabSize );
    }

    /// <summary>
    /// Use the PrintDocument class to print the lines, a page at a time. 
    /// </summary>
    /// <param name="InLines"></param>
    private void PrintLines_Actual(string[] Lines, Font Font )
    {
      mLinesToPrint = Lines;

      if (Font == null)
        mPrinterFont = new Font("Courier New", 7);
      else
        mPrinterFont = Font;

      PrintDocument pd = new PrintDocument();

      mCurrentLineNx = 0;

      pd.DefaultPageSettings.Margins.Top = 50;
      pd.DefaultPageSettings.Margins.Bottom = 50;
      pd.DefaultPageSettings.Margins.Left = 50;
      pd.DefaultPageSettings.Margins.Right = 50;

      pd.PrintPage += 
        new PrintPageEventHandler( PageEventHandler );
      pd.Print();
    }

    /// <summary>
    /// PrintPageEventHandler method called by PrintDocument to print
    /// the lines, a page at a time.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ev"></param>
    private void PageEventHandler(object sender, PrintPageEventArgs ev)
    {
      float linesPerPage = 0;
      float yPos = 0;
      int pageLineNx = 0;
      float leftMargin = ev.MarginBounds.Left;
      float topMargin = ev.MarginBounds.Top;
      string line = null;

      // Calculate the number of lines per page.
      linesPerPage = 
        ev.MarginBounds.Height / mPrinterFont.GetHeight(ev.Graphics);

      // Print each line of the array.
      while( true )
      {
        // the current line to print.
        if ( mCurrentLineNx >= mLinesToPrint.Length )
        {
          line = null ;
          break ;
        }
        line = mLinesToPrint[mCurrentLineNx] ;

        // past end of page.
        if ( pageLineNx >= linesPerPage )
          break ;

        yPos = 
          topMargin + (pageLineNx * mPrinterFont.GetHeight(ev.Graphics));
        
        ev.Graphics.DrawString(
          line, mPrinterFont, System.Drawing.Brushes.Black, leftMargin, yPos, new StringFormat());
        
        // advance to next line in array and location on the page. 
        ++pageLineNx ;
        ++mCurrentLineNx;
      }

      // If more lines exist, print another page.
      if (line != null)
        ev.HasMorePages = true;
      else
        ev.HasMorePages = false;
    }
  }
}
