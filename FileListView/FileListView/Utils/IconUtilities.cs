namespace FileListView.Utils
{
  using System;
  using System.ComponentModel;
  using System.Drawing;
  using System.Runtime.InteropServices;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  /// <summary>
  /// Internal extension method for ImageSource class.
  /// </summary>
  internal static class IconUtilities
  {
    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    public static ImageSource ToImageSource(this Icon icon)
    {
      if (icon == null)
        return null;
      Bitmap bitmap = icon.ToBitmap();
      IntPtr hBitmap = bitmap.GetHbitmap();

      ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
          hBitmap,
          IntPtr.Zero,
          Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());

      if (!DeleteObject(hBitmap))
      {
        throw new Win32Exception();
      }

      return wpfBitmap;
    }
  }
}
