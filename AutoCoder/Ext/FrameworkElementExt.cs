using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AutoCoder.Ext
{
  public static class FrameworkElementExt
  {

    /// <summary>
    /// return the child elements of a framework element. In turn, for each
    /// child, return its child elements, down to child elements without
    /// framework element contents.
    /// </summary>
    /// <param name="Element"></param>
    /// <returns></returns>
    public static IEnumerable<FrameworkElement> ChildFrameworkElementsDeep(
      this FrameworkElement Element)
    {
      if (Element is Decorator)
      {
        Decorator dec = Element as Decorator;
        if (dec.Child is FrameworkElement)
        {
          FrameworkElement fwChild = dec.Child as FrameworkElement;

          // first, return the child of the decorator.
          yield return fwChild;

          // next, return children of the child of the decorator.
          foreach (var childOfChild in fwChild.ChildFrameworkElementsDeep())
          {
            yield return childOfChild;
          }
        }
      }

      else if (Element is Panel)
      {
        Panel pan = Element as Panel;
        foreach (var child in pan.Children)
        {
          if (child is FrameworkElement)
          {
            FrameworkElement fwChild = child as FrameworkElement;

            // first, return the current child of the Panel.
            yield return fwChild;

            // next, use recursion to return children of the current child.
            foreach (var childOfChild in fwChild.ChildFrameworkElementsDeep( ))
            {
              yield return childOfChild;
            }
          }
        }
      }

      yield break;
    }

    /// <summary>
    /// Return the layout slot of the FrameworkElement within the context of
    /// a parent framework element.
    /// </summary>
    /// <param name="Element"></param>
    /// <param name="Context"></param>
    /// <returns></returns>
    public static Tuple<Rect, Rect> GetLayoutSlotWithinContext(
      this FrameworkElement Element, FrameworkElement Context)
    {
      // Rect loc = LayoutInformation.GetLayoutSlot(Element);
      Rect loc = Element.GetLocWithinParent();

      // get the parent of the element.
      FrameworkElement fwParent = null;
      if (Element.Parent is FrameworkElement)
      {
        fwParent = Element.Parent as FrameworkElement;
      }

      // the parent is the context to get the absolute position in.
      if ((fwParent == null) || (fwParent == Context))
      {
        return new Tuple<Rect, Rect>(loc, loc);
      }

        // not yet up to the parent. use recursion to go up the next level and
      // calc the position at that parents level.
      else
      {
        Rect parentSlot = fwParent.GetLayoutSlotWithinContext(Context).Item1;
        double top = parentSlot.Top + fwParent.Margin.Top + loc.Top;
        double left = parentSlot.Left + fwParent.Margin.Left + loc.Left;
        Rect abs = new Rect(new Point(left, top), loc.Size);
        return new Tuple<Rect, Rect>(abs, loc);
      }
    }

    /// <summary>
    /// Total up the child layout slot rects of the StackPanel parent.
    /// </summary>
    /// <param name="Parent"></param>
    /// <returns></returns>
    public static Rect? GetUnionChildLayoutSlots(this StackPanel Parent)
    {
      Rect? unionRect = null ;

      foreach( UIElement uiChild in Parent.Children )
      {
        if ( uiChild is FrameworkElement )
        {
          FrameworkElement fwChild = uiChild as FrameworkElement ;
          Rect slot = LayoutInformation.GetLayoutSlot(fwChild) ;
          if (unionRect == null)
            unionRect = slot;
          else
          {
            Rect ur = unionRect.Value;
            ur.Union(slot);
            unionRect = ur;
//            unionRect.Value.Union(slot);
          }
        }
      }
      return unionRect;
    }

    public static double VerticalAdjustToChildLayoutSlot(this StackPanel Parent)
    {
      if (Parent.VerticalAlignment == VerticalAlignment.Top)
        return 0;
      else if (Parent.VerticalAlignment == VerticalAlignment.Stretch)
        return 0;
      else if (Parent.VerticalAlignment == VerticalAlignment.Center)
      {
        Rect layoutRect = LayoutInformation.GetLayoutSlot(Parent);
        double height = layoutRect.Height;
        double adjust = 0;
        Rect? unionRect = Parent.GetUnionChildLayoutSlots();
        if (unionRect != null)
        {
          double used = unionRect.Value.Height;
          adjust = (height - used) / 2;
        }
        return adjust;
      }
      else
      {
        Rect layoutRect = LayoutInformation.GetLayoutSlot(Parent);
        double height = layoutRect.Height;
        double adjust = 0;
        Rect? unionRect = Parent.GetUnionChildLayoutSlots();
        if (unionRect != null)
        {
          double used = unionRect.Value.Height;
          adjust = height - used;
        }
        return adjust;
      }
    }

    /// <summary>
    /// return a Rect that contains the location and dimensions of the element on
    /// the canvas.
    /// </summary>
    /// <param name="Element"></param>
    /// <returns></returns>
    public static Rect GetCanvasRect(this FrameworkElement Element)
    {
      var top = Canvas.GetTop(Element);
      var left = Canvas.GetLeft(Element);
      var height = Element.ActualHeight;
      var width = Element.ActualWidth;
      return new Rect(left, top, width, height);
    }

    /// <summary>
    /// Similar to GetLayoutSlot. Attempts to compute the actual location of the
    /// FrameworkElement within the layout slot.
    /// </summary>
    /// <param name="Element"></param>
    /// <returns></returns>
    public static Rect GetLocWithinParent(this FrameworkElement Element)
    {
      Rect loc = LayoutInformation.GetLayoutSlot(Element);

      // get the parent of the element.
      FrameworkElement fwParent = null;
      if (Element.Parent is FrameworkElement)
      {
        fwParent = Element.Parent as FrameworkElement;
      }

      if ((fwParent != null) && (fwParent is StackPanel))
      {
        StackPanel stackParent = fwParent as StackPanel;
        if ( stackParent.Orientation == Orientation.Vertical )
        {
          double adjust = stackParent.VerticalAdjustToChildLayoutSlot();
          double adjustedTop = loc.Top + adjust ;
          loc = new Rect(new Point(loc.Left, adjustedTop), loc.Size);
        }
      }

      return loc;
    }

    /// <summary>
    /// determine if the parent of a frameworkElement is a Grid or not.
    /// </summary>
    /// <param name="Child"></param>
    /// <returns></returns>
    public static bool ParentIsGrid(this FrameworkElement Child)
    {
      FrameworkElement Parent = Child.Parent as FrameworkElement;
      if (Parent == null)
        return false;
      else if (Parent is Grid)
        return true;
      else
        return false;
    }
  }
}
