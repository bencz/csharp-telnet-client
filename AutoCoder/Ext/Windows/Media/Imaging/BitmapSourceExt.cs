using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoCoder.Ext.Windows.Media.Imaging
{
  public static class BitmapSourceExt
  {
    /// <summary>
    /// Create BitmapSource from a byte[] of pixel bits.
    /// </summary>
    /// <param name="InImageBits"></param>
    /// <param name="InWidth">Width ( in pixels )</param>
    /// <param name="InHeight">Height ( in pixels )</param>
    /// <returns></returns>
    public static BitmapSource CreateBitmapSourceFromPixelBytes(
      byte[] InImageBits,
      int InWidth, int InHeight)
    {
      List<Color> colors = new List<Color>();
      colors.Add(Colors.Red);
      colors.Add(Colors.Green);
      colors.Add(Colors.Blue);

      BitmapPalette palette = new BitmapPalette(colors);

      var pf = global::System.Windows.Media.PixelFormats.Rgb24;

      int bytesPerPixel = pf.BitsPerPixel / 8;

      // stride is the number of bytes per row of the ImageBits byte array.
      int stride = bytesPerPixel * InWidth;

      BitmapSource image = BitmapSource.Create(
          InWidth,
          InHeight,
          96,
          96,
          pf,
          palette,
          InImageBits,
          stride);

      return image;
    }

    /// <summary>
    /// Return a panel bitmap from the larger, full page input bitmap.
    /// </summary>
    /// <param name="InBitmapPath">Path to the full page, input bitmap file</param>
    /// <param name="InBitmapDim">Pixel dimensions of the full page bitmap</param>
    /// <param name="InDpi">DPI of both input and output images</param>
    /// <param name="InPanelDim">Number of panels. Across and down.</param>
    /// <param name="InPanelPos">Location, in panel numbers, of the panel to extract 
    /// and return</param>
    /// <returns>PNG bitmap containing the extracted panel.</returns>
    public static BitmapSource GetBitmapPanel(
      this BitmapSource InBitmap,
      Point InBitmapDim,
      int InDpi,
      Point InPanelDim, Point InPanelPos)
    {
      BitmapSource bms = null;

      Image im3 = new Image();
      im3.Source = InBitmap;
      im3.Stretch = Stretch.None;

      // canvas holding the entire bitmap image.
      Canvas fullCan = new Canvas();
      fullCan.Width = InBitmapDim.X;
      fullCan.Height = InBitmapDim.Y;
      fullCan.Children.Add(im3);
      fullCan.Measure(new Size(InBitmapDim.X, InBitmapDim.Y));
      fullCan.Arrange(new Rect(0, 0, InBitmapDim.X, InBitmapDim.Y));
      fullCan.UpdateLayout();

      // calc the pixel size of each panel.
      int panelWxRem = 0;
      int panelHxRem = 0;
      int panelWx = Math.DivRem(
        (int)InBitmapDim.X, (int)InPanelDim.X, out panelWxRem);
      int panelHx = Math.DivRem(
        (int)InBitmapDim.Y, (int)InPanelDim.Y, out panelHxRem);

      // based on the address of the panel to get, calc the starting coordinates.
      int leftPos = 0;
      for (int ix = 0; ix < InPanelPos.X; ++ix)
      {
        leftPos -= panelWx;
      }

      int topPos = 0;
      for (int ix = 0; ix < InPanelPos.Y; ++ix)
      {
        topPos -= panelHx;
      }

      // the overlay, panel canvas.
      Canvas panelCan = new Canvas();
      panelCan.Width = panelWx;
      panelCan.Height = panelHx;

      // add the full canvas to the panel canvas. 
      // The negative top and left values of the full canvas shift the target panel
      // within view of the panel canvas.
      panelCan.Children.Add(fullCan);

      // set the full bitmap canvas within the smaller, panel canvas.
      // Use a negative left and top start position to shift the full bitmap canvas
      // up and the the left, effectively moving the targeted panel into the viewport
      // of the overlayed canvas.
      // This overlayed canvas will then be rendered to the output, panel bitmap.
      fullCan.SetValue(Canvas.TopProperty, (double)topPos);
      fullCan.SetValue(Canvas.LeftProperty, (double)leftPos);

      // update the layout of the panel.
      panelCan.Measure(new Size(panelWx, panelHx));
      panelCan.Arrange(new Rect(0, 0, panelWx, panelHx));
      panelCan.UpdateLayout();

      // bitmap to create from the contents of the panel canvas.
      RenderTargetBitmap bitmap = new RenderTargetBitmap(
        (int)panelWx, (int)panelHx, InDpi, InDpi,
        PixelFormats.Pbgra32);
      bitmap.Render(panelCan);

      bms = bitmap;

      return bms;
    }

    /// <summary>
    /// Load the bitmap from a PNG file.
    /// </summary>
    /// <param name="InPngPath"></param>
    /// <returns></returns>
    public static BitmapSource LoadBitmapSourceFromPngFile(string InPngPath)
    {
      BitmapSource bms = null;
      byte[] fileBytes = global::System.IO.File.ReadAllBytes(InPngPath);
      var ms = new MemoryStream(fileBytes);
      PngBitmapDecoder decoder = new PngBitmapDecoder(
          ms, BitmapCreateOptions.PreservePixelFormat,
          BitmapCacheOption.Default);
      bms = decoder.Frames[0];
      return bms;
    }

    /// <summary>
    /// Print a bitmap that stretches or shrinks to fit the page.
    /// </summary>
    /// <param name="InBms"></param>
    /// <param name="InMargin"></param>
    /// <param name="InDescription"></param>
    public static void PrintBitmapSource(
      this BitmapSource InBms, Thickness InMargin,
      string InDescription = "Bitmap image")
    {
      PrintDialog pd = new PrintDialog();
      bool? ret = pd.ShowDialog();
      if (ret.Value == true)
      {
        Grid drawingArea = new Grid();
        drawingArea.Width = pd.PrintableAreaWidth;
        drawingArea.Height = pd.PrintableAreaHeight;

        Image im3 = new Image();
        im3.Margin = InMargin;
        im3.Source = InBms;
        im3.Stretch = Stretch.Fill;
        drawingArea.Children.Add(im3);

        Rect rect = new Rect(0, 0, pd.PrintableAreaWidth, pd.PrintableAreaHeight);
        Size size = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
        drawingArea.Arrange(rect);
        drawingArea.UpdateLayout();

        pd.PrintVisual(drawingArea, InDescription);
      }
    }

    /// <summary>
    /// Print a bitmapsource without the prompt of the printer dialog.
    /// Supply the PrintQueue and PrintTicket used to assign the printer and the
    /// printer options.
    /// </summary>
    /// <param name="InBms"></param>
    /// <param name="InMargin"></param>
    /// <param name="InPrintQueue"></param>
    /// <param name="InPrintTicket"></param>
    /// <param name="Description"></param>
    public static void PrintBitmapSource(
      this BitmapSource InBms, Thickness InMargin,
      PrintQueue InPrintQueue,
      PrintTicket InPrintTicket,
      string Description = "Bitmap image")
    {
      PrintDialog pd = new PrintDialog();

      pd.PrintQueue = InPrintQueue;
      pd.PrintTicket = InPrintTicket;

      Grid drawingArea = new Grid();
      drawingArea.Width = pd.PrintableAreaWidth;
      drawingArea.Height = pd.PrintableAreaHeight;

      Image im3 = new Image();
      im3.Margin = InMargin;
      im3.Source = InBms;
      im3.Stretch = Stretch.Fill;
      drawingArea.Children.Add(im3);

      Rect rect = new Rect(0, 0, pd.PrintableAreaWidth, pd.PrintableAreaHeight);
      Size size = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
      drawingArea.Arrange(rect);
      drawingArea.UpdateLayout();

      pd.PrintVisual(drawingArea, Description);
    }

    public static BitmapSource Rotate(this BitmapSource InBms, double InAngle)
    {
      BitmapSource b2 = new TransformedBitmap(InBms, new RotateTransform(InAngle));
      return b2;
    }

    /// <summary>
    /// store the bitmap as a PNG file.
    /// </summary>
    /// <param name="InBitmap"></param>
    /// <param name="InPngPath"></param>
    public static void StoreBitmapSourceToPngFile(
      this BitmapSource InBitmap, string InPngPath)
    {
      using (FileStream fs = new FileStream(InPngPath, FileMode.Create))
      {
        PngBitmapEncoder enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(InBitmap));
        enc.Save(fs);
      }
    }

    public static BitmapImage ToBitmapImage(this BitmapSource bms)
    {
      //      JpegBitmapEncoder encoder = new JpegBitmapEncoder();
      TiffBitmapEncoder encoder = new TiffBitmapEncoder();

      var memoryStream = new MemoryStream();
      var bi = new BitmapImage();

      encoder.Frames.Add(BitmapFrame.Create(bms));
      encoder.Save(memoryStream);

      bi.BeginInit();
      bi.StreamSource = new MemoryStream(memoryStream.ToArray());
      bi.EndInit();

      memoryStream.Close();

      return bi;
    }

    /// <summary>
    /// Write the bitmap to the file in the encoding implied by the extension of the
    /// destination file.
    /// </summary>
    /// <param name="Bitmap"></param>
    /// <param name="ToFilePath"></param>
    public static void WriteToFile(this BitmapSource Bitmap, string ToFilePath)
    {
      var ext = global::System.IO.Path.GetExtension(ToFilePath).ToLower();
      var ms = new MemoryStream();

      if ((ext == ".jpg") || ( ext == ".jpeg"))
      {
        var encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else if ((ext == ".tif") || ( ext == ".tiff"))
      {
        var encoder = new TiffBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else if (ext == ".png")
      {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else if (ext == ".bmp")
      {
        var encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else if (ext == ".gif")
      {
        var encoder = new GifBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else if (ext == ".wmp")
      {
        var encoder = new WmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(Bitmap));
        encoder.Save(ms);
      }

      else
      {
        throw new Exception("extension of file " + ToFilePath + " not supported.");
      }

      byte[] bytes = new byte[ms.Length];
      ms.Seek(0, SeekOrigin.Begin);
      ms.Read(bytes, 0, (int)ms.Length);
      ms.Close();

      global::System.IO.File.WriteAllBytes(ToFilePath, bytes);
    }
  }
}
