using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoCoder.Windows
{
  public static class DependencyPropertier
  {
    public static DependencyProperty Register<T>( string InPropName, Type InOwningClass )
    {
      DependencyProperty prop = DependencyProperty.Register(
        InPropName, typeof(T), InOwningClass, 
        new PropertyMetadata(default(T)));
      return prop;
    }
  }
}
