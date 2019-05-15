using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.IO ;
using AutoCoder.Text ;

#if skip

namespace AutoCoder.Compiler
{
  public enum CompilerInputForm { File, Array } ;

  public class AcCompilerParameters : CompilerParameters 
  {
    ExecutableType mExecutableType = ExecutableType.Console ;
    GenerateAssemblyIn mGenerateAssemblyIn ;
    FileInfo mSourceFileInfo = null ;
    string mSourceLines = null ;

    public AcCompilerParameters()
    {
      GenerateAssemblyIn = GenerateAssemblyIn.File ;
    }

    public ExecutableType ExecutableType
    {
      get { return mExecutableType ; }
      set
      {
        mExecutableType = value ;
        if ( mExecutableType == ExecutableType.Windows )
        {
          AddToCompilerOptions( "/target:winexe" ) ;
          this.GenerateExecutable = true;
        }
        else if (mExecutableType == ExecutableType.ClassLibrary)
        {
          AddToCompilerOptions( "/target:library" ) ;
          this.GenerateExecutable = false;
        }
      }
    }

    public GenerateAssemblyIn GenerateAssemblyIn
    {
      get { return mGenerateAssemblyIn; }

      set
      {
        mGenerateAssemblyIn = value;

        // generate the assembly in memory
        if (value == GenerateAssemblyIn.Memory)
        {
          this.GenerateInMemory = true;
          this.OutputAssembly = null;
        }

        // generate the assembly in an output file.
        else
        {
          this.GenerateInMemory = false;
        }
      }
    }

    /// <summary>
    /// the file containing the source statement lines to compile from.
    /// </summary>
    public FileInfo SourceFileInfo
    {
      get { return mSourceFileInfo; }
      set { mSourceFileInfo = value; }
    }

    /// <summary>
    /// the array containing the source statement lines to compile from.
    /// </summary>
    public string SourceLines
    {
      get { return mSourceLines; }
      set { mSourceLines = value; }
    }

    public void AddToCompilerOptions(string InOptions)
    {
      if (Stringer.IsBlank(this.CompilerOptions) == true)
        this.CompilerOptions = InOptions;
      else
        this.CompilerOptions = this.CompilerOptions + " " + InOptions;
    }

  }
}

#endif

