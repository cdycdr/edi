namespace Edi.AvalonEdit.Highlighting
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using System.Xml;
  using ICSharpCode.AvalonEdit.Highlighting;
  using ICSharpCode.AvalonEdit.Highlighting.Xshd;
  using System.Globalization;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;

  /// <summary>
  /// Class for handling file streams and association with xshd highlighting patterns and names
  /// </summary>
  internal class HighlightingExtension
  {
    #region constructor
    public HighlightingExtension(string name,
                                  string[] fileNameExtensions,
                                  string resourceURI,
                                  string resourceName)
    {
      this.Name = name;

      if (fileNameExtensions != null)
      {
        this.FileNameExtensions = new string[fileNameExtensions.Length];
        for (int i = 0; i < fileNameExtensions.Length; i++)
        {
          this.FileNameExtensions[i] = fileNameExtensions[i];
        }
      }

      this.ResourceName = resourceName;
      this.ResourceURI = resourceURI;
    }
    #endregion constructor

    #region properties
    public string Name { get; set; }

    public string[] FileNameExtensions { get; set; }

    public string ResourceName { get; set; }
    public string ResourceURI { get; set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Register custom highlighting patterns for usage in the editor
    /// </summary>
    public static void RegisterCustomHighlightingPatterns(HighlightingThemes hlThemes = null)
    {
      try
      {
        HighlightingManager.Instance.InitializeDefinitions(hlThemes);

        string path = Path.GetDirectoryName(Application.ResourceAssembly.Location);
        path = Path.Combine(path , "AvalonEdit\\Highlighting");

        if (Directory.Exists(path))
        {
          // Some HighlightingDefinitions contain 'import' statements which means that some
          // XSHDs have to be loaded before others (an exception is thrown otherwise)
          // Therefore, we use filenames to indicate sequence for loading xshds
          SortedSet<string> files = new SortedSet<string>(Directory.GetFiles(path).Where(x =>
          {
            var extension = Path.GetExtension(x);
            return extension != null && extension.Contains("xshd");
          }));

          foreach (var file in files)
          {
            try
            {
              var definition = LoadXshdDefinition(file);
              var hightlight = LoadHighlightingDefinition(file);

              HighlightingManager.Instance.RegisterHighlighting(definition.Name, definition.Extensions.ToArray(), hightlight);
            }
            catch (Exception ex)
            {
              string filename = file;

              try
              {
                filename = System.IO.Path.GetFileName(file);
              }
              catch
              {
              }

              MsgBox.Msg.Show(ex, string.Format(CultureInfo.InvariantCulture, Util.Local.Strings.STR_MSG_ErrorRegisteringHighlighting, filename),
                              MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Alert, MsgBox.MsgBoxResult.NoDefaultButton,
                              App.IssueTrackerLink, App.IssueTrackerTitle, App.IssueTrackerText, null, true);
            }
          }
        }
      }
      catch (Exception ex)
      {
        MsgBox.Msg.Show(ex, Util.Local.Strings.STR_MSG_Unexpected_Error,
                        MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Alert, MsgBox.MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerTitle, App.IssueTrackerText, null, true);
      }
    }

    private static XshdSyntaxDefinition LoadXshdDefinition(string fullName)
    {
      using (var reader = new XmlTextReader(fullName))
        return HighlightingLoader.LoadXshd(reader);
    }
                                                                                                        
    private static IHighlightingDefinition LoadHighlightingDefinition(string fullName)
    {
      using (var reader = new XmlTextReader(fullName))
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    #endregion methods
  }
}
