using AutoCoder.Enums;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.File;
using ScreenDefnLib.Defn;
using ScreenDefnLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ScreenDefnLib.CopyPaste
{
  public class CopyPasteList : List<CopyPasteItem>
  {
    public static string ClassName = "CopyPasteList";

    /// <summary>
    /// when this list is the global copy/paste list screen items are marked as
    /// copy/paste when they are added to this list.  That property of the
    /// ScreenItemModel then is used in binding to show the item as highlighted 
    /// when it is copy/paste pending.
    /// </summary>
    private bool IsGlobalList
    { get; set; }

    public CopyPasteList( bool IsGlobalList )
    {
      this.IsGlobalList = IsGlobalList;
    }
    public void AddCopy(ISectionHeader header, IScreenItem screenItem)
    {
      Add_Common(CopyPasteCode.Copy, header, screenItem);
    }
    public void AddCut(IScreenItem screenItem)
    {
      Add_Common(CopyPasteCode.Cut, screenItem.SectionHeader, screenItem);
    }

    public new void Clear( )
    {
      if ( this.IsGlobalList == true )
      {
        foreach( var copyPasteItem in this )
        {
          var screenItem = copyPasteItem.ScreenItem;
          screenItem.CopyPasteCode = null;
        }
      }
      base.Clear();
    }

    public IScreenItem PasteAfter( ISectionHeader ToSectionHeader, int ToIndex )
    {
      var item = Paste_Actual(RelativePosition.After, ToSectionHeader, ToIndex);
      return item;
    }
    public IScreenItem PasteBefore(ISectionHeader ToSectionHeader, int ToIndex)
    {
      var item = Paste_Actual(RelativePosition.Before, ToSectionHeader, ToIndex);
      return item;
    }

    /// <summary>
    /// paste the items from this CopyPasteList onto the end of the ToList.
    /// </summary>
    /// <param name="ToList"></param>
    /// <returns></returns>
    public IScreenItem PasteBottom(ISectionHeader ToSectionHeader)
    {
      var item = Paste_Actual(RelativePosition.End, ToSectionHeader);
      return item;
    }

    private IScreenItem Paste_Actual(
      RelativePosition Rltv, ISectionHeader ToSectionHeader, int ToIndex = -1)
    {
      int toIndex = ToIndex;
      IScreenItem lastPasteItem = null;
      if (ToIndex >= 0)
        lastPasteItem = ToSectionHeader.GetItemAt(ToIndex);
      var rltv = Rltv;

      foreach (var item in this)
      {
        IScreenItem pasteItem = null;

        // remove from the from list.
        if (item.CopyPasteCode == CopyPasteCode.Cut)
        {
          pasteItem = item.Cut();
        }

        // dup the item to copy.
        if (item.CopyPasteCode == CopyPasteCode.Copy)
        {
          pasteItem = ScreenItemModel.Factory(item.ScreenItem);
          pasteItem.AssignItemGuid();
        }

        // index of the insert relative to item.
        toIndex = -1;
        if (lastPasteItem != null)
        {
          toIndex = ToSectionHeader.Items.IndexOf(lastPasteItem);
        }

        // insert into ToList.
        if (rltv == RelativePosition.After)
          ToSectionHeader.InsertItemAfter(toIndex, pasteItem);
        else if (rltv == RelativePosition.Begin)
          ToSectionHeader.InsertItemBegin(pasteItem);
        else if (rltv == RelativePosition.End)
          ToSectionHeader.AddItem(pasteItem);
        else if (rltv == RelativePosition.Before)
          ToSectionHeader.InsertItemBefore(toIndex, pasteItem);
        else
          throw new Exception("invalid relative postion");

        // clear the copy/paste marking on the screenItem.
        if ( this.IsGlobalList == true )
          item.ScreenItem.CopyPasteCode = null;

        // the last pasted item.
        lastPasteItem = pasteItem;

        // subsequent items are placed after the prior pasted item
        rltv = RelativePosition.After;
      }

      // clear the list of pending actions.
      this.Clear();
      return lastPasteItem;
    }

    /// <summary>
    /// paste the items from this CopyPasteList onto the top of the ToList.
    /// </summary>
    /// <param name="ToList"></param>
    /// <returns></returns>
    public IScreenItem PasteTop(ISectionHeader ToSectionHeader)
    {
      var item = Paste_Actual(RelativePosition.Begin, ToSectionHeader);
      return item;
    }

    private void Add_Common(CopyPasteCode code, ISectionHeader header, IScreenItem screenItem)
    {
      if (screenItem != null)
      {
        var wasCode = screenItem.CopyPasteCode;
        EnsureRemoved(screenItem);

        // apply the code to the screenItem as a toggle. If the item CopyPasteCode
        // is the same as code being added, then remove the code. Do not add.
        if (wasCode.CompareEqual(code) != true)
        {
          // mark the screen item with the action.
          if ( this.IsGlobalList == true )
            screenItem.CopyPasteCode = code;

          // add to this pending action list.
          var cpItem = new CopyPasteItem(code, screenItem, header);
          this.Add(cpItem);
        }
      }
    }

    public void EnsureRemoved(IScreenItem ScreenItem)
    {
      var found = this.FindCopyPasteItem(ScreenItem);
      if ( found != null)
      {
        this.Remove(found);
      }

      if ( this.IsGlobalList == true )
        ScreenItem.CopyPasteCode = null;
    }

    /// <summary>
    /// find the CopyPasteItem in the list where the ScreenItem is the 
    /// find item.
    /// </summary>
    /// <param name="find"></param>
    /// <returns></returns>
    public CopyPasteItem FindCopyPasteItem( IScreenItem find)
    {
      var found = this.FirstOrDefault(c => c.ScreenItem == find);
      return found;
    }
    private static string BuildApplDataDir()
    {
      string vendorName = "AutoCoder";
      string solutionName = "tnClient";
      string projectName = "tnClient";

      string applDataDir =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      applDataDir = Path.Combine(applDataDir, vendorName);
      applDataDir = Path.Combine(applDataDir, solutionName);
      applDataDir = Path.Combine(applDataDir, projectName);

      return applDataDir;
    }

    public static CopyPasteList FromXml(XmlReader Reader)
    {
      XDocument doc = XDocument.Load(Reader);
      XNamespace ns = doc.Root.Name.Namespace;
      var docRoot = doc.Root.Elements().First();
      XElement xe = doc.Element(ns + CopyPasteList.ClassName);
      xe = xe.Element(ns + CopyPasteList.ClassName);

      var copyPasteList = xe.Element(ns + "CopyPasteList").ToCopyPasteList(ns);
      return copyPasteList;
    }

    public static CopyPasteList RecallCopyPasteList()
    {
      CopyPasteList copyPasteList = null;
      string applDataDir = CopyPasteList.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "CopyPasteList.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      if (File.Exists(applStatePath) == true)
      {
        try
        {
          using (var tr = new XmlTextReader(applStatePath))
          {
            copyPasteList = CopyPasteList.FromXml(tr);
          }
        }
        catch (Exception excp)
        {
          Debug.Print(excp.ToString());
          throw excp;
        }
      }
      return copyPasteList;
    }
  }

  public static class CopyPasteListExt
  {
    public static CopyPasteList ToCopyPasteList(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return new CopyPasteList(false);
      else
      {
        var sl = from c in Elem.Elements(Namespace + "CopyPasteItem")
                 select c.ToCopyPasteItem(Namespace);

        var copyPasteList = new CopyPasteList(false);
        foreach (var sf in sl)
        {
          copyPasteList.Add(sf);
        }

        return copyPasteList;
      }
    }

  }
}
