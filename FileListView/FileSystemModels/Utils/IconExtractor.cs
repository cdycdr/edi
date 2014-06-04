namespace FileSystemModels.Utils
{
  using System;
  using System.Drawing;
  using System.Runtime.InteropServices;

  [StructLayout(LayoutKind.Sequential)]
  public struct SHFILEINFO
  {
    public IntPtr hIcon;
    public IntPtr iIcon;
    public uint dwAttributes;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szDisplayName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    public string szTypeName;
  }

  public class IconExtractor
  {
    public const uint SHGFI_ICON = 0x100;
    public const uint SHGFI_LARGEICON = 0x0;
    public const uint SHGFI_SMALLICON = 0x1;
    public const uint SHGFI_OPENICON = 0x2;

    [DllImport("shell32.dll")]
    public static extern IntPtr SHGetFileInfo(string pszPath,
                                uint dwFileAttributes,
                                ref SHFILEINFO psfi,
                                uint cbSizeFileInfo,
                                uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static Icon GetFileIcon(string cFile)
    {
      // return Icon.ExtractAssociatedIcon(cFile).;
      SHFILEINFO shi = new SHFILEINFO();
      IntPtr hIcon = SHGetFileInfo(cFile, 0, ref shi, (uint)(Marshal.SizeOf(shi)), SHGFI_SMALLICON | SHGFI_ICON);

      if (shi.hIcon != IntPtr.Zero)
      {
        Icon ret = (Icon)Icon.FromHandle(shi.hIcon).Clone();
        DestroyIcon(shi.hIcon);
        return ret;
      }
      else
        return null;
    }

    public static Icon GetFolderIcon(string cFolder, bool bOpen = false)
    {
      SHFILEINFO shi = new SHFILEINFO();
      uint uFlags = SHGFI_SMALLICON | SHGFI_ICON;

      if (bOpen == true)
        uFlags |= SHGFI_OPENICON;

      IntPtr hIcon = SHGetFileInfo(cFolder, 0, ref shi, (uint)Marshal.SizeOf(shi), uFlags);

      if (shi.hIcon != IntPtr.Zero)
      {
        Icon ret = (Icon)Icon.FromHandle(shi.hIcon).Clone();
        bool l = DestroyIcon(shi.hIcon);
        return ret;
      }
      else
        return null;
    }
  }
}
