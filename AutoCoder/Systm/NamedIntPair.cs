using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  public class NamedIntPair : IntPair
  {
    public NamedIntPair(int a, int b, string Name)
      : base(a,b)
    {
//      this.a = a;
//      this.b = b;
      this.Name = Name;
    }

#if skip
    public int a
    { get; set; }

    public int b
    { get; set; }
#endif

    public string Name
    { get; set; }

    public override string ToString()
    {
      return Name + ":" + a + ',' + b;
    }
  }
}
