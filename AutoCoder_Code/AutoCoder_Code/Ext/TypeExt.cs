using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Code.Ext
{
  public static class TypeExt
  {
    public static PortableExecutableReference GetReference(this Type type)
    {
      var execRef = MetadataReference.CreateFromFile(type.Assembly.Location);
      return execRef;
    }
  }
}
