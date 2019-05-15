using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Code
{
  public static class CodeAnalyzer
  {
    public static int dummy( )
    {
      return 0;
    }

    /// <summary>
    /// get the only or first static method contained in the source code.
    /// </summary>
    /// <param name="Source"></param>
    /// <returns></returns>
    public static Tuple<string, string, string> GetFirstMethod( string source )
    {
      string methodName = null;
      string className = null;
      string namespaceName = null;

      var tree = SyntaxFactory.ParseSyntaxTree(source);
      var syntaxRoot = tree.GetRoot();
      var firstClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
      var firstMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

      if ( firstClass != null)
      {
        className = firstClass.Identifier.ToString();
        var firstNamespace = firstClass.Parent as NamespaceDeclarationSyntax;
        if (firstNamespace != null)
          namespaceName = firstNamespace.Name.ToString();
      }

      if ( firstMethod != null)
        methodName = firstMethod.Identifier.ToString();

      return new Tuple<string, string, string>(namespaceName, className, methodName);
    }
  }
}
