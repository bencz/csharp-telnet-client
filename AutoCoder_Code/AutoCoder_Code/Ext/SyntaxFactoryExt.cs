using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Code.Ext
{
  public static class SyntaxFactoryExt
  {
    public static SyntaxTree[] ParseToSyntaxTree(string[] ArraySourceCode)
    {
      var treeList = new List<SyntaxTree>();

      foreach (var sourceCode in ArraySourceCode)
      {
        var tree = SyntaxFactory.ParseSyntaxTree(sourceCode);
        treeList.Add(tree);
      }
      return treeList.ToArray();
    }
  }
}
