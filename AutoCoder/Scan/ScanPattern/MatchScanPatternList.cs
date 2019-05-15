using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{
  public class MatchScanPatternList : LinkedList<MatchScanPattern>
  {
    public MatchScanPatternList()
      : base()
    {
    }

    /// <summary>
    /// Add the ScanPattern to the list of ScanPattern.
    /// That is, add to the list if another ScanPattern with the same delim class does
    /// not exist in the list.
    /// If a ScanPattern with the same delim class does exist, replace that ScanPattern
    /// with this one if the match length exceeds the pattern in the list. 
    /// As an example, the ** pattern is found. It could be the start of a comment or the
    /// dereference of a pointer to a pointer. Also, a * could be a mult symbol, a 
    /// dereference of a pointer, or the start of a special value.
    /// The ** that is a unary operator would replace the * unary operator in the list. 
    /// The other entries in the list, the ones for delim class comment begin and
    /// special value starter would stay.
    /// </summary>
    /// <param name="Pat"></param>
    /// <param name="MatchLx"></param>
    public void Add(ScanPattern Pat, int Pos, int MatchLx)
    {
      MatchScanPattern matpat = new MatchScanPattern(Pat, Pos, MatchLx);
      Add(matpat);
    }

    public void Add(MatchScanPattern Pat)
    {
      if (this.Count == 0)
      {
        this.AddLast(Pat);
      }
      else
      {
        LinkedListNode<MatchScanPattern> foundNode = null;
        foundNode = this.FindNode(
          c => c.MatchPattern.CompareClassification == Pat.MatchPattern.CompareClassification);

        // find the existing entry for the delim class of the Pattern being added.
        if (foundNode == null)
        {
          this.AddLast(Pat);
        }

        else
        {
          if (foundNode.Value.MatchLength < Pat.MatchLength)
          {
            this.Remove(foundNode);
            this.AddLast(Pat);
          }
        }
      }

    }
  }

  public static class MatchScanPatternListExt
  {
    public static MatchScanPattern FindPattern(
      this MatchScanPatternList List, DelimClassification DelimClass)
    {
      MatchScanPattern found = null;
      if (List != null)
      {
        var node = List.FindNode(c => c.MatchPattern.DelimClassification == DelimClass);
        if (node != null)
          found = node.Value;
      }

      return found;
    }
  }

}
