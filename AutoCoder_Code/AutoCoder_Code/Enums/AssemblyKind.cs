using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Code.Enums
  {
    public enum AssemblyKind
    {
      ConsoleApplication = 0,
      WindowsApplication = 1,
      DynamicallyLinkedLibrary = 2
    }

  public static class AssemblyKindExt
    {
      public static OutputKind ToOutputKind(this AssemblyKind assemblyKind)
      {
        if (assemblyKind == AssemblyKind.ConsoleApplication)
          return OutputKind.ConsoleApplication;
        else if (assemblyKind == AssemblyKind.WindowsApplication)
          return OutputKind.WindowsApplication;
        else if (assemblyKind == AssemblyKind.DynamicallyLinkedLibrary)
          return OutputKind.DynamicallyLinkedLibrary;
        else
          throw new Exception("unexpected AssemblyKind");
      }
    }
  }

