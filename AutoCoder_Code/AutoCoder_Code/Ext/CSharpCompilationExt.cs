using AutoCoder.Code.Enums;
using AutoCoder.Systm.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCoder.Code.Ext
{
  public static class CSharpCompilationExt
  {
    public static Tuple<Assembly, IEnumerable<string>> xCompileSource(string sourceText)
    {
      var errmsgs = new List<string>();
      Assembly assem = null;

      using (var ms = new MemoryStream())
      {
        string assemblyFileName =
          "gen" + Guid.NewGuid().ToString().Replace("-", "") + ".dll";

        // get a "PortableExecutableReference" that identifies each referenced 
        // assembly.
        // ( To get that reference, pass an instance of an object from each 
        //   assembly to an extension method. )
        var mscorlib =
          MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var windowsBase = (new FileFormatException()).GetReference();
        var presentationCore = (DragAction.Cancel).GetReference();
        var presentationFramework = (new EventTrigger()).GetReference();
        var diagnostics =
          (new System.Diagnostics.BooleanSwitch("abc", "efg")).GetReference();

        CSharpCompilation compilation = CSharpCompilation.Create(assemblyFileName,
            new[] { CSharpSyntaxTree.ParseText(sourceText) },
            new[] { mscorlib, windowsBase, presentationCore, presentationFramework, diagnostics },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

        var rv = compilation.Emit(ms);
        foreach (var item in rv.Diagnostics)
        {
          var s1 = item.ToString();
          errmsgs.Add(s1);
        }

        if (rv.Success == true)
          assem = Assembly.Load(ms.GetBuffer());
      }

      return new Tuple<Assembly, IEnumerable<string>>(assem, errmsgs);
    }

    /// <summary>
    /// compile the C# code contained in the array of input streams. Where each
    /// stream is a text string containing C# code lines delimited by New
    /// </summary>
    /// <param name="sourceText"></param>
    /// <param name="outputKind"></param>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static Tuple<Assembly, IEnumerable<string>> CompileSource(
      string[] sourceText,
      PortableExecutableReference anotherReference,
      AssemblyKind assemKind = AssemblyKind.DynamicallyLinkedLibrary,
      string assemblyName = null)
    {
      var errmsgs = new List<string>();
      Assembly assem = null;

      var treeArray = SyntaxFactoryExt.ParseToSyntaxTree(sourceText);
      var compileOptions = new CSharpCompilationOptions(assemKind.ToOutputKind( ));
      if (assemblyName == null)
        assemblyName = "gen" + Guid.NewGuid().ToString().Replace("-", "")
                        + ".dll";

      // get a "PortableExecutableReference" that identifies each referenced 
      // assembly.
      // ( To get that reference, pass an instance of an object from each 
      //   assembly to an extension method. )
      var mscorlib =
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
      var windowsBase = (new FileFormatException()).GetReference();
      var systemData = (new DataTable()).GetReference();
      var presentationCore = (DragAction.Cancel).GetReference();
      var presentationFramework = (new EventTrigger()).GetReference();
      var diagnostics =
        (new System.Diagnostics.BooleanSwitch("abc", "efg")).GetReference();
      var systemCore = typeof(ImmutableArrayExtensions).GetReference();
      var autocoder = typeof(EnhancedDataTable).GetReference();

      // run the compiler, using the parsed syntax trees as input.
      CSharpCompilation compilation = CSharpCompilation.Create(
        assemblyName: assemblyName,
        options: compileOptions,
        syntaxTrees: treeArray,
        references: new[] { mscorlib, windowsBase, presentationCore,
                            presentationFramework, diagnostics, systemData,
                            systemCore, autocoder, anotherReference }
          );

      using (var ms = new MemoryStream())
      {
        var rv = compilation.Emit(ms);
        foreach (var item in rv.Diagnostics)
        {
          var s1 = item.ToString();
          errmsgs.Add(s1);
        }

        if (rv.Success == true)
        {
          assem = Assembly.Load(ms.GetBuffer());
        }
      }

      return new Tuple<Assembly, IEnumerable<string>>(assem, errmsgs);
    }

    /// <summary>
    /// sample use of the CompileSource method.
    /// </summary>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <returns></returns>
    public static Tuple<Assembly, IEnumerable<string>> CompileSourceDemo(
      string source1, string source2,
      PortableExecutableReference anotherReference)
    {
      IEnumerable<string> errmsgs = null;
      Assembly assem = null;
      {
        string[] sourceInput = new string[] { source1, source2 };
        var rv = CSharpCompilationExt.CompileSource(sourceInput, anotherReference);
        assem = rv.Item1;
        errmsgs = rv.Item2;
      }

      if (errmsgs != null)
      {
        foreach (var errmsg in errmsgs)
        {
          Debug.WriteLine(errmsg);
        }
      }

      return new Tuple<Assembly, IEnumerable<string>>(assem, errmsgs);
    }
  }
}
