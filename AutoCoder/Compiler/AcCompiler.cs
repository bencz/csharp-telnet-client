using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using AutoCoder ;
using AutoCoder.Text;

#if skip

namespace AutoCoder.Compiler
{
  public class AcCompiler
  {
    /// <summary>
    /// Class that is private to the statics in the AcCompiler class.
    /// Used to allow access to the protected constructor and "Set" methods of
    /// the AcCompilerResults class.
    /// The purpose being that the AcCompilerResults return value cant be 
    /// instantiated and does not allow the setting of any properties or members.
    /// </summary>
    private class WipCompilerResults : AcCompilerResults
    {
      public new void SetCompilerResults(CompilerResults InResults)
      {
        base.SetCompilerResults(InResults);
      }

      public new void SetCompilerParameters(AcCompilerParameters InParms)
      {
        base.SetCompilerParameters(InParms);
      }
    }

    /// <summary>
    /// Compile the supplied source file. 
    /// Return the compile output messages and the resulting assembly.
    /// </summary>
    /// <param name="InSourcePath"></param>
    /// <param name="InExecType"></param>
    /// <param name="InGenerateIn"></param>
    /// <returns></returns>
    public static 
      AcCompilerResults 
      CompileFromFile(
      String InSourcePath, ExecutableType InExecType,
      GenerateAssemblyIn InGenerateIn,
      CompilerVersion CompilerVersion)
    {
      List<string> addnReferencedAssemblies = new List<string>();
      AcCompilerResults results = Compile_Actual( 
        InSourcePath, null, InExecType, InGenerateIn,
        addnReferencedAssemblies,
        CompilerVersion) ;

      return results;
    }

    /// <summary>
    /// Compile an assembly from array of source lines.
    /// Return the compile output messages and the resulting assembly.
    /// </summary>
    /// <param name="InSourceLines"></param>
    /// <param name="InExecType"></param>
    /// <param name="InGenerateIn"></param>
    /// <returns></returns>
    public static 
      AcCompilerResults 
      CompileFromSource(
      string InSourceLines, ExecutableType InExecType,
      GenerateAssemblyIn InGenerateIn,
      CompilerVersion CompilerVersion)
    {
      List<string> addnReferencedAssemblies = new List<string>();
      AcCompilerResults results = Compile_Actual( 
        null, InSourceLines, InExecType, InGenerateIn,
        addnReferencedAssemblies,
        CompilerVersion ) ;

      return results;
    }

    public static
      AcCompilerResults
      CompileFromSource(
      string InSourceLines, ExecutableType InExecType,
      GenerateAssemblyIn InGenerateIn,
      List<string> InAddnReferencedAssemblies,
      CompilerVersion CompilerVersion)
    {
      AcCompilerResults results = Compile_Actual(
        null, InSourceLines, InExecType, InGenerateIn,
        InAddnReferencedAssemblies,
        CompilerVersion );

      return results;
    }

    // ------------------------- Compile_Actual -----------------------------
    private static AcCompilerResults
      Compile_Actual(
      string InSourcePath,
      string InSourceLines,
      ExecutableType InExecType,
      GenerateAssemblyIn InGenerateIn,
      List<string> InAddnReferencedAssemblies,
      CompilerVersion CompilerVersion
      )
    {
      System.Reflection.Assembly assem = null;
      List<string> results = new List<string>();
      CodeDomProvider provider = null;
      // Microsoft.CSharp.CSharpCodeProvider provider = null;
      string exeName = null;
      WipCompilerResults wipCr = new WipCompilerResults();
      AcCompilerParameters cp = new AcCompilerParameters();

      // compile from lines in a source array or source file
      if (InSourceLines != null)
        cp.SourceLines = InSourceLines;
      else
        cp.SourceFileInfo = new FileInfo(InSourcePath);

      // Select the code provider based on the input file extension.
      if (cp.SourceLines != null)
      {
        Dictionary<string, string> provOpts =
          new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } };
        provider = new Microsoft.CSharp.CSharpCodeProvider( provOpts );
//        provider = CodeDomProvider.CreateProvider("CSharp");
      }
      else if (cp.SourceFileInfo.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
      {
        Dictionary<string, string> provOpts =
          new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } };
        provider = new Microsoft.CSharp.CSharpCodeProvider(provOpts);
//        provider = CodeDomProvider.CreateProvider("CSharp");
      }
      else
      {
        results.Add("Source file must have a .cs or .vb extension");
      }

      if (provider != null)
      {
        //  type of executable: Windows, Console, ClassLibrary
        cp.ExecutableType = InExecType;
        cp.GenerateAssemblyIn = InGenerateIn;

        // setup name and path of the executable file to be created.
        if (cp.GenerateAssemblyIn == GenerateAssemblyIn.File)
        {
          if (cp.ExecutableType == ExecutableType.ClassLibrary)
            exeName = AcFilePath.NameSansExtension(cp.SourceFileInfo.Name) + ".dll";
          else
            exeName = AcFilePath.NameSansExtension(cp.SourceFileInfo.Name) + ".exe";
          string exePath = AcFilePath.AddTo(cp.SourceFileInfo.DirectoryName, exeName);
          cp.OutputAssembly = exePath;
        }

        // Set whether to treat all warnings as errors.
        cp.TreatWarningsAsErrors = false;
        cp.ReferencedAssemblies.Add("System.Dll");
        cp.ReferencedAssemblies.Add("System.Drawing.Dll");
        cp.ReferencedAssemblies.Add("System.Xml.Dll");
        cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

        // additional assemblies
        foreach (string s1 in InAddnReferencedAssemblies)
        {
          cp.ReferencedAssemblies.Add(s1);
        }

        // Invoke compilation of the source file.
        CompilerResults cr = null;
        if ( cp.SourceLines != null)
        {
          cr = provider.CompileAssemblyFromSource(cp, cp.SourceLines);
        }
        else
        {
          cr = provider.CompileAssemblyFromFile(cp, cp.SourceFileInfo.FullName);
        }
        wipCr.SetCompilerResults( cr ) ;
        wipCr.SetCompilerParameters( cp ) ;

        // completion message
        results.Add( wipCr.CompletionMessage );

        if (wipCr.Errors.Count > 0)
        {
          foreach (CompilerError ce in wipCr.Errors)
          {
            results.Add("   " + ce.ToString());
          }
        }

        // Return the results of the compilation.
        if (wipCr.Errors.Count > 0)
        {
          assem = null;
        }

        else
        {
          assem = wipCr.CompiledAssembly;
        }
      }
      return wipCr;
    }

    public static string ResultsxCompletionMessage(
      AcCompilerParameters InParms, CompilerResults InResults)
    {
      string msg = null;
      string fileName = null;

      // the name of the source file 
      if (InParms.SourceLines != null)
        fileName = "";
      else
        fileName = InParms.SourceFileInfo.Name;

      if ( InResults.Errors.Count == 0)
      {
        if (InParms.GenerateInMemory == true)
          msg =
            "Source " + fileName +
            " built into memory resident assembly";
        else
          msg = "Source " + fileName +
            " built into " + InResults.PathToAssembly;
      }
      else
      {
        msg = "Errors building " + fileName;
      }
      return msg;
    }

  }

#endif


