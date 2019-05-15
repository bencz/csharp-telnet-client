using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AutoCoder.Ext.System.Xml
{
  public static class XmlElementExt
  {

    public static bool ContainsNodes( this XmlElement parent, string tagName)
    {
      bool doesContain = false;
      if (parent.GetXmlElementNodes(tagName).FirstOrDefault() != null)
        doesContain = true;

      return doesContain;
    }

    public static IEnumerable<XmlElement> GetXmlElementNodes(this XmlElement parent, string tagName = null )
    {
      foreach( var node in parent.ChildNodes)
      {
        if ( node is XmlElement)
        {
          var elem = node as XmlElement;

          if (tagName != null)
          {
            if (elem.LocalName.ToLower() != tagName)
              continue;
          }

          yield return elem;
        }
      }
      yield break;
    }

  }
}
