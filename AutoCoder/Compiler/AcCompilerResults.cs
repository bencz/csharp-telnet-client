using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection ;
using System.Security.Policy ;
using System.Text;

#if skip

namespace AutoCoder.Compiler
{
  public class AcCompilerResults
  {
    CompilerResults mCr;
    AcCompilerParameters mParms = null;

    protected AcCompilerResults( )
    {
    }

    public Assembly CompiledAssembly
    {
      get { return mCr.CompiledAssembly; }
    }

    public CompilerErrorCollection Errors
    {
      get { return mCr.Errors; }
    }

    public System.Security.Policy.Evidence Evidence
    {
      get { return mCr.Evidence; }
    }

    public AcCompilerParameters Parameters
    {
      get { return mParms; }
    }

    public string PathToAssembly
    {
      get { return mCr.PathToAssembly; }
    }

    public System.Collections.Specialized.StringCollection Output
    {
      get { return mCr.Output; }
    }

    public string CompletionMessage
    {
      get
      {
        string msg = null;
        string fileName = null;

        // the name of the source file 
        if (Parameters.SourceLines != null)
          fileName = "";
        else
          fileName = Parameters.SourceFileInfo.Name;

        if ( this.Errors.Count == 0)
        {
          if ( Parameters.GenerateInMemory == true)
            msg =
              "Source " + fileName +
              " built into memory resident assembly";
          else
            msg = "Source " + fileName +
              " built into " + this.PathToAssembly;
        }
        else
        {
          msg = "Errors building " + fileName;
        }
        return msg;
      }
    }

    protected void SetCompilerResults(CompilerResults InResults)
    {
      mCr = InResults;
    }

    protected void SetCompilerParameters(AcCompilerParameters InParms)
    {
      mParms = InParms;
    }

  }
}

#endif
