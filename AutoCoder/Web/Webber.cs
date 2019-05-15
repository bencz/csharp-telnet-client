using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls ;

namespace AutoCoder.Web
{

  /// <summary>
  /// a collection of web related static methods
  /// </summary>
  public class Webber
  {

    /// <summary>
    /// Add a header cell to the table row.
    /// </summary>
    /// <param name="InRow"></param>
    /// <param name="InText"></param>
    /// <returns></returns>
    public static TableRow AddTableHeaderCell(TableRow InRow, string InText)
    {
      TableHeaderCell cell = new TableHeaderCell();
      cell.Text = InText;
      InRow.Controls.Add(cell);
      return InRow;
    }

    /// <summary>
    /// Add a TableRow to a System.Web.Ui.WebControls.Table
    /// </summary>
    /// <param name="InTable"></param>
    /// <returns></returns>
    public static TableRow AddTableRow(Table InTable)
    {
      TableRow row = new TableRow();
      InTable.Controls.Add(row);
      return row;
    }

    // calc the sub directory needed when redirecting from this page to another page.
    public static string CalcRedirectDirPath(
      IPageProperties InPageProperties, string InToPageDirPath)
    {
      string DirPath;
      string FromPageDirPath = InPageProperties.GetPageDirPath( ) ;
      if ((FromPageDirPath == null) && (InToPageDirPath == null))
        DirPath = null;
      else if (FromPageDirPath == null)
        DirPath = InToPageDirPath;

        // to page in the appl root. add ".." to the relative page for each directory
      // in the from page dirpath. ( have to unwind 1 or more directory levels )
      else if (InToPageDirPath == null)
      {
        StringBuilder sb = new StringBuilder();
        int Cx = Text.Stringer.Talley(FromPageDirPath, new char[] { '/', '\\' });
        ++Cx;
        for (int Ix = 0; Ix < Cx; ++Ix)
        {
          if (sb.Length > 0)
            sb.Append("/");
          sb.Append("..");
        }
        DirPath = sb.ToString();
      }

      else if (FromPageDirPath.ToLower() == InToPageDirPath.ToLower())
        DirPath = null;
      else
        DirPath = "../" + InToPageDirPath;
      return DirPath;
    }

    // ------------------------- RedirectTo -------------------------------
    // Called by pages when redirecting to this page.
    // aspx pages should implement their own version when the page contains 
    // QueryString parameters.  Otherwise, this base class can do the job.
    public static void RedirectTo(AcPage InFromPage, PageProperties InToPage )
    {
      RedirectTo(InFromPage, InToPage, new RedirectParms());
    }

    // ------------------------- RedirectTo -------------------------------
    // Called by pages when redirecting to this page.
    // aspx pages should implement their own version when the page contains 
    // QueryString parameters.  Otherwise, this base class can sparamete
    public static void RedirectTo( 
      AcPage InFromPage, PageProperties InToPage, RedirectParms InParms)
    {
      bool bArgs = false;
      string subDir = InFromPage.CalcRedirectDirPath( InToPage.PageDirPath);
      string passSessId = InFromPage.ComposePassSessId();

      // Build the full path name needed to redirect to this page relative to the
      // current page ( the from page )
      System.Text.StringBuilder url = new System.Text.StringBuilder(256);
      if (subDir != null)
        url.Append(subDir + "/");
      url.Append( InToPage.PageFileName);

      // add the SessId needed when a "no cookies browser" 
      if ( passSessId != null)
      {
        url.Append("?" + passSessId);
        bArgs = true;
      }

      // add the querystring parms.
      if (InParms.IsNotEmpty)
      {
        if (bArgs == false)
          url.Append("?" + InParms.QueryString);
        else
          url.Append("&" + InParms.QueryString);
        bArgs = true;
      }

      // redirect to the page.
      InFromPage.Response.Redirect(url.ToString());
    }

    public static void RedirectTo(
      System.Web.HttpResponse InResponse,
      IPageProperties InToPage, RedirectParms InParms)
    {
      bool bArgs = false;
      string subDir = InToPage.GetPageDirPath( ) ;

      // Build the full path name needed to redirect to this page relative to the
      // current page ( the from page )
      System.Text.StringBuilder url = new System.Text.StringBuilder(256);
      if (subDir != null)
        url.Append(subDir + "/");
      url.Append(InToPage.GetPageFileName( )) ;

      // add the querystring parms.
      if (InParms.IsNotEmpty)
      {
        if (bArgs == false)
          url.Append("?" + InParms.QueryString);
        else
          url.Append("&" + InParms.QueryString);
        bArgs = true;
      }

      // redirect to the page.
      InResponse.Redirect(url.ToString());
    }

    public static void RedirectTo(
      System.Web.HttpResponse InResponse,
      string InPageDirPath, string InPageName, RedirectParms InParms)
    {
      bool bArgs = false;

      // Build the full path name needed to redirect to this page relative to the
      // current page ( the from page )
      System.Text.StringBuilder url = new System.Text.StringBuilder(256);
      if (InPageDirPath != null)
        url.Append(InPageDirPath + "/");
      url.Append( InPageName );

      // add the querystring parms.
      if (InParms.IsNotEmpty)
      {
        if (bArgs == false)
          url.Append("?" + InParms.QueryString);
        else
          url.Append("&" + InParms.QueryString);
        bArgs = true;
      }

      // redirect to the page.
      InResponse.Redirect(url.ToString());
    }



  }
}
