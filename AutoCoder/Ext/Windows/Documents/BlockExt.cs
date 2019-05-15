using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace AutoCoder.Ext.Windows.Documents
{
  public static class BlockExt
  {
    /// <summary>
    /// Create a FlowDocument from a list of blocks.
    /// </summary>
    /// <param name="Blocks"></param>
    /// <returns></returns>
    public static FlowDocument ToDocument(this List<Block> Blocks)
    {
      FlowDocument doc = null;
      if (Blocks.Count > 0)
      {
        doc = new FlowDocument();
        foreach (var block in Blocks)
        {
          doc.Blocks.Add(block);
        }
      }
      return doc;
    }
  }
}
