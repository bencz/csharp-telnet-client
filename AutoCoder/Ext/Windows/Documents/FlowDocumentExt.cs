using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Markup;
using AutoCoder.Ext.System.Collections.Generic;

namespace AutoCoder.Ext.Windows.Documents
{
  public static class FlowDocumentExt
  {

    public static FlowDocument Concat(this FlowDocument FlowDoc1, FlowDocument FlowDoc2)
    {
      var concatBlocks = FlowDoc1.Blocks.Concat(FlowDoc2.Blocks).ToList();
      var doc = concatBlocks.ToDocument();
      return doc;
    }

    /// <summary>
    /// Split the list of blocks of the document into a list of blocks before the text
    /// pointer, the block at the text pointer, and the list of blocks after the text
    /// pointer.
    /// </summary>
    /// <param name="Document"></param>
    /// <param name="Pointer"></param>
    /// <returns></returns>
    public static Tuple<List<Block>, Block, List<Block>> SplitBlocks(
      this FlowDocument Document, TextPointer Pointer)
    {
      var beforeBlocks = new List<Block>();
      var afterBlocks = new List<Block>();
      Block atBlock = null;
      foreach (var block in Document.Blocks)
      {
        if (block.ContentEnd.CompareTo(Pointer) == -1)
          beforeBlocks.Add(block);
        else if (block.ContentStart.CompareTo(Pointer) == 1)
          afterBlocks.Add(block);
        else if (atBlock == null)
          atBlock = block;
        else
          throw new ApplicationException(
            "SplitBlocks exception. More than 1 block at the pointer.");
      }
      return new Tuple<List<Block>, Block, List<Block>>(beforeBlocks, atBlock, afterBlocks);
    }

    public static string ToXaml(this FlowDocument Document)
    {
      if (Document == null)
        return "";
      else
      {
        var xamlString = XamlWriter.Save(Document);
        return xamlString;
      }
    }

    public static FlowDocument ToFlowDocument(this string Text)
    {
      var flowDoc = new FlowDocument();
      if (Text != null)
      {
        flowDoc = (FlowDocument)XamlReader.Parse(Text);
      }

      return flowDoc;
    }
  }
}
