using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  /// <summary>
  /// methods used to support the ToText method of a class.
  /// ToText and ClassName are virtual methods. ToText concats the important 
  /// properties of a class to text form. ClassName provides the short class name
  /// of the class. The ToString method then concats the return values of each
  /// method.
  /// </summary>
  public interface IToText
  {
    string ToText();
    string ClassName();
  }
}
