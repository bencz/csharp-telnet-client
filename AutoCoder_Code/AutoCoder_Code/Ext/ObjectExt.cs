using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Code.Ext
{
  public static class ObjectExt
  {
    public static PortableExecutableReference GetReference(this object Obj)
    {
      var tx = Obj.GetType();
      var s3 = tx.AssemblyQualifiedName;
      var tx4 = Type.GetType(s3);
      var execRef = MetadataReference.CreateFromFile(tx.Assembly.Location);
      return execRef;
    }
  }
}
