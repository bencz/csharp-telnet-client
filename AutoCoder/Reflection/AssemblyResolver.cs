using AutoCoder.Ext.System;
using AutoCoder.Ext.System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Reflection
{
  // usage of this class. Within the using braces, any load of an assembly
  // will search for the assembly in the specified assemDirPath.
  // ( this directory will be search in addition to normal search path. )
  // using (var ar = new AssemblyResolver(assemDirPath))
  //      {
  //
  //        // the type to make the XElement method for.
  //      }

  public class AssemblyResolver : IDisposable
  {

    public AssemblyResolver(string resolveDirPath)
    {
      this.AssemblyResolveDirectoryPath = resolveDirPath;
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    private string AssemblyResolveDirectoryPath
    { get; set; }

    public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      Assembly assem = null;
      var assemName = args.Name;

      // get simple name of assembly being resolved to.
      string simpleName = null;
      {
        var fx = args.Name.IndexOf(',');
        simpleName = args.Name.Substring(0, fx);
      }

      // setup 
      string assemPath = null;
      if (this.AssemblyResolveDirectoryPath.IsNullOrEmpty() == false)
      {
        assemPath = FileExt.ResolveSimpleName(
          this.AssemblyResolveDirectoryPath, simpleName,
          new string[] { "dll", "exe" });
      }

      // resolve to the assembly.
      if (assemPath.IsNullOrEmpty() == false)
      {
        assem = Assembly.LoadFile(assemPath);
      }

      return assem;
    }
  }

}
