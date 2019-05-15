using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace AutoCoder.Ext.Windows.Media.Imaging
{
  public static class BitmapImageExt
  {

    /// <summary>
    /// create a new BitmapImage from image encoded bytes.
    /// </summary>
    /// <param name="ImageBytes"></param>
    /// <returns></returns>
    public static BitmapImage New(byte[] ImageBytes)
    {
      var image = new BitmapImage();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.BeginInit();
      image.StreamSource = ImageBytes.ToMemoryStream();
      image.EndInit();

      return image;
    }
  }
}
