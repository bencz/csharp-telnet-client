using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TextCanvasLib.Common
{
  public class FontDefn
  {
    public FontFamily Family { get; set; }
    public double PointSize { get; set; }
    public FontStyle Style { get; set; }
    public FontWeight Weight { get; set; }
    public FontStretch Stretch { get; set; }
    public Brush Foreground { get; set; }

    public FontDefn(FontFamily Family, double PointSize,
      FontStyle Style, FontWeight Weight, FontStretch Stretch,
      Brush Foreground )
    {
      this.Family = Family;
      this.PointSize = PointSize;
      this.Style = Style;
      this.Weight = Weight;
      this.Stretch = Stretch;
      this.Foreground = Foreground;
    }

    public FontDefn( double PointSize)
      : this(new FontFamily("Lucida Console"), PointSize, FontStyles.Normal, 
          FontWeights.Normal, FontStretches.Normal, Brushes.White )
    {
    }

    public Size MeasureString(string Text)
    {
      var formattedText = new FormattedText(
          Text,
          CultureInfo.CurrentUICulture,
          FlowDirection.LeftToRight,
          new Typeface(this.Family, this.Style, this.Weight, this.Stretch),
          this.PointSize,
          Brushes.Black);

      return new Size(formattedText.Width, formattedText.Height);
    }

    public override string ToString()
    {
      var s1 = "FontDefn. " + this.Family.ToString() +
        " PointSize:" + this.PointSize.ToString() +
        " Foreground:" + this.Foreground.ToString();
      return s1;
    }

  }
}
