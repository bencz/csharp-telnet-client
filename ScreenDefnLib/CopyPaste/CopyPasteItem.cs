using AutoCoder.Enums;
using AutoCoder.Ext;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScreenDefnLib.CopyPaste
{
  /// <summary>
  /// IScreenItem that is being cut or copied.  There is a list of these items
  /// which is stored in the telnetClient window. Any cut or copy actions are 
  /// stored in the CopyPasteList. Then when the paste action is run the paste
  /// code will work from that list to locate all the items to cut and copy.
  /// </summary>
  public class CopyPasteItem
  {
    public CopyPasteItem(CopyPasteCode code, IScreenItem item, ISectionHeader header)
    {
      this.CopyPasteCode = code;
      this.ScreenItem = item;
      this.SectionHeader = header;
    }

    public CopyPasteCode CopyPasteCode
    { get; set; }
    public IScreenItem ScreenItem
    { get; set; }
    public ISectionHeader SectionHeader
    { get; set; }

    public IScreenItem Cut()
    {
      var screenItem = this.ScreenItem.Remove();
      return screenItem;
    }
  }

  public static class CopyPasteItemExt
  {

    public static CopyPasteItem ToCopyPasteItem(
      this XElement Elem, XNamespace Namespace)
    {
      CopyPasteItem copyPasteItem = null;
      if (Elem != null)
      {
        var copyPasteCode = Elem.Element(Namespace + "CopyPasteCode").StringOrDefault("none").TryParseCopyPasteCode().Value;
        ISectionHeader header = null;
        var item = Elem.Element(Namespace + "ScreenItem").ToScreenItem(Namespace);
        copyPasteItem = new CopyPasteItem(copyPasteCode, item, header);
      }
      return copyPasteItem;
    }

  }
}
