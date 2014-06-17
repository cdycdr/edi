﻿namespace FileSystemModels.Models
{
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using System.Xml.Serialization;
  using FileSystemModels.Interfaces;

  [Serializable]
  [XmlRoot(ElementName = "ExplorerSettings", IsNullable = true)]
  public class ExplorerSettingsModel
  {
    #region fields
    private ExplorerUserProfile mUserProfile = null;

    private List<string> mRecentFolders = null;
    private List<FilterItemModel> mFilterCollection = null;
    private readonly List<CustomFolderItemModel> mSpecialFolders = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor with default value initialization.
    /// </summary>
    /// <param name="CreateDefaultSettings"></param>
    public ExplorerSettingsModel(bool CreateDefaultSettings)
    : this()
    {
      if (CreateDefaultSettings == true)
        this.CreateDefaultSettings();
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public ExplorerSettingsModel()
    {
      this.mUserProfile = new ExplorerUserProfile();

      this.mRecentFolders = new List<string>();

      this.mFilterCollection = new List<FilterItemModel>();

      this.ShowFolders = this.ShowHiddenFiles = this.ShowIcons = true;

      this.mSpecialFolders = this.CreateSpecialFolderCollection();
      this.ShowSpecialFoldersInTreeBrowser = false;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets/Sets user session specific settings that are
    /// stored and loaded on each application shutdown/re-start.
    /// 
    /// User profile should be persisted seperately, therefore, its ignored here.
    /// </summary>
    [XmlIgnore]
    public ExplorerUserProfile UserProfile
    {
      get
      {
        return this.mUserProfile;
      }

      set
      {
        this.mUserProfile = value;
      }
    }

    [XmlArray("RecentFolders")]
    public List<string> RecentFolders
    {
      get
      {
        return this.mRecentFolders;
      }

      set
      {
        if (this.mRecentFolders != value)
          this.mRecentFolders = value;
      }
    }

    #region filter settings
    [XmlElement(ElementName = "FilterCollection")]
    public List<FilterItemModel> FilterCollection
    {
      get
      {
        return this.mFilterCollection;
      }

      set
      {
        if (this.mFilterCollection != value)
          this.mFilterCollection = value;
      }
    }
    #endregion filter settings

    [XmlAttribute(AttributeName = "ShowIcons")]
    public bool ShowIcons { get; set; }

    [XmlAttribute(AttributeName = "ShowFolders")]
    public bool ShowFolders { get; set; }

    [XmlAttribute(AttributeName = "ShowHiddenFiles")]
    public bool ShowHiddenFiles { get; set; }

    [XmlAttribute(AttributeName = "IsFiltered")]
    public bool IsFiltered { get; set; }

    [XmlIgnore]
    public IEnumerable<CustomFolderItemModel> SpecialFolders
    {
      get
      {
        return this.mSpecialFolders;
      }
    }

    [XmlIgnore]
    public bool ShowSpecialFoldersInTreeBrowser { get; set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Creates default file filter and recent folder settings
    /// for initialization at the very first time (no settings available for load from persistence).
    /// </summary>
    private void CreateDefaultSettings()
    {
      this.ShowFolders = this.ShowHiddenFiles = this.ShowIcons = true;
      this.IsFiltered = false;
      this.UserProfile.CurrentFilter = new FilterItemModel( "Text files", "*.txt");

      this.AddRecentFolder(@"C:\temp\");
      this.AddRecentFolder(@"C:\windows\");

      this.AddFilter("XML", "*.aml;*.xml;*.xsl;*.xslt;*.xsd;*.config;*.addin;*.wxs;*.wxi;*.wxl;*.build;*.xfrm;*.targets;*.xpt;*.xft;*.map;*.wsdl;*.disco;*.ps1xml;*.nuspec");
      this.AddFilter("C#", "*.cs;*.manifest;*.resx;*.xaml");
      this.AddFilter("Edi", "*.xshd");
      this.AddFilter("JavaScript", "*.js");
      this.AddFilter("HTML", "*.htm;*.html");
      this.AddFilter("ActionScript3", "*.as");
      this.AddFilter("ASP/XHTML", "*.asp;*.aspx;*.asax;*.asmx;*.ascx;*.master");
      this.AddFilter("Boo", "*.boo");
      this.AddFilter("Coco", "*.atg");
      this.AddFilter("C++", "*.c;*.h;*.cc;*.cpp;*.hpp;*.rc");
      this.AddFilter("CSS", "*.css");
      this.AddFilter("BAT", "*.bat;*.dos");
      this.AddFilter("F#", "*.fs");
      this.AddFilter("INI", "*.cfg;*.conf;*.ini;*.iss;");
      this.AddFilter("Java", "*.java");
      this.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
      this.AddFilter("LOG", "*.log");
      this.AddFilter("MarkDown", "*.md");
      this.AddFilter("Patch", "*.patch;*.diff");
      this.AddFilter("PowerShell", "*.ps1;*.psm1;*.psd1");
      this.AddFilter("Projects", "*.proj;*.csproj;*.drproj;*.vbproj;*.ilproj;*.booproj");
      this.AddFilter("Python", "*.py");
      this.AddFilter("Ruby", "*.rb");
      this.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
      this.AddFilter("StyleCop", "*.StyleCop");
      this.AddFilter("SQL", "*.sql");
      this.AddFilter("Squirrel", "*.nut");
      this.AddFilter("Tex", "*.tex");
      this.AddFilter("Text files", "*.txt");
      this.AddFilter("VBNET", "*.vb");
      this.AddFilter("VTL", "*.vtl;*.vm");
      this.AddFilter("All Files", "*.*");
    }

    /// <summary>
    /// Creates a default collection of special folders (Desktop, MyDocuments, MyMusic, MyVideos).
    /// </summary>
    public List<CustomFolderItemModel> CreateSpecialFolderCollection()
    {
      var specialFolders = new List<CustomFolderItemModel>();

      specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.Desktop));
      specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyDocuments));
      specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyMusic));
      specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyVideos));

      return specialFolders;
    }

    public void AddSpecialFolder(Environment.SpecialFolder folder)
    {
      this.mSpecialFolders.Add(new CustomFolderItemModel(folder));
    }

    /// <summary>
    /// Add a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    public void AddRecentFolder(string folderPath)
    {
      if ((folderPath = PathModel.ExtractDirectoryRoot(folderPath)) == null)
        return;

      this.mRecentFolders.Add(folderPath);
    }

    /// <summary>
    /// Removes a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="path"></param>
    public void RemoveRecentFolder(string path)
    {
      if (string.IsNullOrEmpty(path) == true)
        return;

      this.mRecentFolders.Remove(path);
    }

    /// <summary>
    /// Add a filter item into the collection of filters.
    /// </summary>
    /// <param name="folderPath"></param>
    public void AddFilter(string name,
                          string pattern,
                          bool bSelectNewFilter = false)
    {
      var newFilter = new FilterItemModel(name, pattern);

      this.mFilterCollection.Add(newFilter);

      if (bSelectNewFilter == true && this.mUserProfile != null)
        this.mUserProfile.CurrentFilter = newFilter;
    }

    /// <summary>
    /// Compares 2 setting models and returns true if they are equal
    /// (data is same between both models) or otherwise false.
    /// 
    /// Attention: The UserProfile data property is ignored here.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="settings"></param>
    /// <returns>false if both collections differ, otherwise true</returns>
    public static bool CompareSettings(ExplorerSettingsModel input,
                                       ExplorerSettingsModel settings)
    {
      if (input == null && settings != null || input != null && settings == null)
        return false;

      // Reference to same object
      if (input == settings)
        return true;

      // Compare file filter collections and return false if they differ
      // Compare recentfolders collections and return false if they differ
      if ((input.FilterCollection == null && settings.FilterCollection != null) ||
          (input.FilterCollection != null && settings.FilterCollection == null))
        return false;

      if (input.FilterCollection == null && settings.FilterCollection == null)
        return true;

      if (input.FilterCollection.Count != settings.FilterCollection.Count)
        return false;

      foreach (var item in input.FilterCollection)
      {
        var found = settings.FilterCollection.Where(i => i.Equals(item) == true);

        if (found.Count() == 0)
          return false;
      }

      // Compare recentfolders collections and return false if they differ
      if ((input.RecentFolders == null && settings.RecentFolders != null) ||
          (input.RecentFolders != null && settings.RecentFolders == null))
        return false;

      if (input.RecentFolders == null && settings.RecentFolders == null)
        return true;

      if (input.RecentFolders.Count != settings.RecentFolders.Count)
        return false;

      foreach (var item in input.RecentFolders)
      {
        var found = settings.RecentFolders.Where(i => string.Compare(i, item, true) == 0);

        if (found.Count() == 0)
          return false;
      }

      // Compare Showfolder, Icons, and hidden files options
      if (input.ShowFolders != settings.ShowFolders)
        return false;

      if (input.ShowIcons != settings.ShowIcons)
        return false;

      if (input.ShowHiddenFiles != settings.ShowHiddenFiles)
        return false;

      if (input.IsFiltered != settings.IsFiltered)
        return false;

      return true; // settings are the same
    }
    #endregion methods
  }
}
