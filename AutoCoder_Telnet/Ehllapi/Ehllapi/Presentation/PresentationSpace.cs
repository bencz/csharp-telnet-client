using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using AutoCoder.File;
using AutoCoder.Ehllapi.Common;
using AutoCoder.Ext;

namespace AutoCoder.Ehllapi.Presentation
{
  /// <summary>
  /// represents the presentation space bytes returned by CopyPresentationSpace.
  /// Each char on the display is represente by a PresentationSpacePixel.
  /// </summary>
  public class PresentationSpace
  {
    PresentationSpaceDim mDim = null;
    DisplayLocation mCursorLocation = null;

    LinkedList<PresentationSpaceField> mFields;
    
    string mPsText = null;
    int mColCx = 0;
    int mRowCx = 0;
    string[] mLines = null ;

    public PresentationSpace(LowPresentationSpace LowSpace)
    {
      this.Dim = LowSpace.Dim;
      this.CursorLocation = LowSpace.CursorLocation;
      this.Pixels = LowSpace.Pixels;
    }

    public PresentationSpace(DisplayLocation CursorLocation, PresentationSpaceDim Dim,
      IEnumerable<PresentationSpaceField> Fields)
    {
      this.Dim = Dim;
      this.CursorLocation = CursorLocation;
      this.Fields = new LinkedList<PresentationSpaceField>();
      foreach (var item in Fields)
      {
        this.Fields.AddLast(item);
      }
    }

    public DisplayLocation CursorLocation
    {
      get { return mCursorLocation; }
      set { mCursorLocation = value; }
    }

    public PresentationSpaceDim Dim
    {
      get { return mDim; }
      set { mDim = value; }
    }

    public LinkedList<PresentationSpaceField> Fields
    {
      get
      {
        if (mFields == null)
          LoadFields();
        return mFields;
      }
      set { mFields = value; }
    }

    public LinkedList<PresentationSpacePixel> Pixels
    {
      get ; set ; 
    }

    /// <summary>
    /// Find the PresentationSpaceField that the FindLoc location is
    /// located within. 
    /// </summary>
    /// <param name="InSess"></param>
    /// <param name="InFindLoc"></param>
    /// <returns></returns>
    public PresentationSpaceField FindField(DisplayLocation InFindLoc )
    {
      foreach (PresentationSpaceField fld in Fields)
      {
        if ( fld.ContainsLocation( InFindLoc ) == true )
        return fld;
      }
      return null;
    }

    private void LoadFields()
    {
      mFields = new LinkedList<PresentationSpaceField>();
      LinkedListNode<PresentationSpacePixel> node = Pixels.First;
      while (node != null)
      {
        if (node.Value.IsFieldAttribute == true)
        {
          LinkedListNode<PresentationSpacePixel> endNode = null;
          PresentationSpaceField fld = 
            new PresentationSpaceField( out endNode, this, node);
          mFields.AddLast(fld);
          node = endNode; // the end node of this just loaded field.
        }

        node = node.Next;
      }
    }

    /// <summary>
    /// Report the fields of the presentation space.
    /// </summary>
    /// <returns></returns>
    public List<string> ReportFields()
    {
      List<string> lines = new List<string>();
      foreach (PresentationSpaceField fld in this.Fields)
      {
        if (lines.Count > 0)
          lines.Add(" ");
        string s1 = fld.Text;
        string s2 = fld.Location.ToString();
        string s3 = fld.Length.ToString();
        string s4 = s2 + " Lgth: " + s3 + " vlu: " + s1;
        lines.Add(s4);
        lines.Add(fld.FieldAttribute.ToString());
        if (fld.CharAttrByte != null)
          lines.Add(fld.CharAttrByte.ToString());
        if (fld.ColorAttrByte != null)
          lines.Add(fld.ColorAttrByte.ToString());
      }
      return lines;
    }

    public string ToDumpString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (PresentationSpacePixel pixel in Pixels)
      {
        if (pixel.DisplayLocation.Column == 1)
        {
          if (pixel.DisplayLocation.Row != 1)
            sb.Append(Environment.NewLine);
          sb.Append(pixel.DisplayLocation.Row.ToString() + ".) ");
        }
        else
          sb.Append(", ");
        sb.Append(pixel.ToDumpString());
      }
      sb.Append(Environment.NewLine);
      return sb.ToString();
    }


    public static Tuple<PresentationSpace, string> ReadCaptureDoc(string FilePath)
    {
      string docText = System.IO.File.ReadAllText(FilePath);
      var doc = XDocument.Parse(docText);
      var ns = doc.Root.Name.Namespace;

      var topElem = doc.Element(ns + "ScreenCapture");
      var sessName = topElem.Element(ns + "LongName").StringOrDefault("");
      var ps = topElem.Element(ns + "PresentationSpace").PresentationSpaceOrDefault(ns, null);

      return new Tuple<PresentationSpace, string>(ps, sessName);
    }

  }

  public static class PresentationSpaceExt
  {

    public static XElement ToXElement(this PresentationSpace Space, XName Name)
    {
      if (Space == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,

             new XElement("Fields",
               from c in Space.Fields
               select c.ToXElement("Field")),

               Space.Dim.ToXElement("Dim"),
               Space.CursorLocation.ToXElement("CursorLocation")
          );
        return xe;
      }
    }

    public static PresentationSpace PresentationSpaceOrDefault(
      this XElement Elem, XNamespace ns, PresentationSpace Default = null)
    {
      if (Elem == null)
        return Default;
      else
      {
        var dim = Elem.Element("Dim").PresentationSpaceDimOrDefault(ns, null);
        var loc = Elem.Element("CursorLocation").DisplayLocationOrDefault(ns, null);

        var flds =
        from sam in Elem.Element(ns + "Fields")
          .Elements(ns + "Field")
          select sam.PresentationSpaceFieldOrDefault(ns, null) ;

        var ps = new PresentationSpace(loc, dim, flds);
        return ps;
      }
    }

    public static void WriteCaptureDoc(
      this PresentationSpace Space, string CaptureFolderPath, string LongName, string WindowText)
    {
      var xe = Space.ToXElement("PresentationSpace");

      XDocument xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("display session presentation space"),
            new XElement("ScreenCapture",
            xe,
            new XElement("LongName", LongName),
            new XElement("WindowText", WindowText))
            );

      // setup the capture file name.
      var nowDate = DateTime.Now;
      string fullPath = CaptureFolderPath;

      // year name.
      {
        string yearName = nowDate.ToString("yyyy");
        fullPath = System.IO.Path.Combine(fullPath, yearName);
        Pather.AssureDirectoryExists(fullPath);
      }

      // month name.
      {
        string monthName = nowDate.ToString("MMMM");
        fullPath = System.IO.Path.Combine(fullPath, monthName);
        Pather.AssureDirectoryExists(fullPath);
      }

      // day name.
      {
        string dayName = nowDate.ToString("dd");
        fullPath = System.IO.Path.Combine(fullPath, dayName);
        Pather.AssureDirectoryExists(fullPath);
      }

      // hour name.
      {
        string hourName = nowDate.ToString("hh");
        fullPath = System.IO.Path.Combine(fullPath, hourName);
        Pather.AssureDirectoryExists(fullPath);
      }

      // session name.
      {
        fullPath = System.IO.Path.Combine(fullPath, LongName);
        Pather.AssureDirectoryExists(fullPath);
      }

      // file name.
      {
        string fileName = "Capture_" + LongName + "_" + nowDate.ToString("HH.mm.ss") + ".xml";
        fullPath = System.IO.Path.Combine(fullPath, fileName);
      }

      // save the document
      xdoc.Save(fullPath);
    }
  }
}
