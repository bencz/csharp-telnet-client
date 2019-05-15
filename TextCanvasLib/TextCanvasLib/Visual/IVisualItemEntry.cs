using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Common;
using TextCanvasLib.Canvas;

namespace TextCanvasLib.Visual
{
  // classes that implement IVisualItemEntry: VisualTextBlock.  ( not VisualSpanner )
  public interface IVisualItemEntry
  {
    VisualItem ApplyText(LocatedString Text);
    VisualItem RemoveText(LocatedString RemoveText);

  }
}
