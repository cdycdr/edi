﻿namespace Settings.UserProfile
{
  using System.Xml.Serialization;
  using SimpleControls.MRU.ViewModel;

  /// <summary>
  /// This class implements the model of the user profile part
  /// of the application. Typically, users have implicit run-time
  /// settings that should be re-activated when the application
  /// is re-started at a later point in time (e.g. window size
  /// and position).
  /// 
  /// This class organizes these per user specific profile settings
  /// and is responsible for their storage (at program end) and
  /// retrieval at the start-up of the application.
  /// </summary>
  public class Profile
  {
    #region fields
    private MRUListVM mMruList;

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public Profile()
    {
      // Session Data
      this.MainWindowPosSz = new ViewPosSizeModel(100, 100, 1000, 700);

      this.LastActiveFile = string.Empty;

      // Construct MRUListVM ViewModel with parameter to decide whether pinned entries
      // are sorted into the first (pinned list) spot or not (favourites list)
      this.mMruList = new MRUListVM(MRUSortMethod.PinnedEntriesFirst);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set position and size of MainWindow
    /// </summary>
    [XmlElement(ElementName = "MainWindowPos")]
    public ViewPosSizeModel MainWindowPosSz { get; set; }

    /// <summary>
    /// Remember the last active path and name of last active document.
    /// 
    /// This can be useful when selecting active document in next session or
    /// determining a useful default path when there is no document currently open.
    /// </summary>
    [XmlAttribute(AttributeName = "LastActiveFile")]
    public string LastActiveFile { get; set; }

    /// <summary>
    /// List of most recently used files
    /// </summary>
    public MRUListVM MruList
    {
      get
      {
        if (this.mMruList == null)
          this.mMruList = new MRUListVM();

        return this.mMruList;
      }

      set
      {
        if (this.mMruList != value)
        {
          this.mMruList = value;
        }
      }
    }
    #endregion properties

    #region methods
    public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                    double SystemParameters_VirtualScreenTop)
    {
      if (this.MainWindowPosSz == null)
        this.MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

      if (this.MainWindowPosSz.DefaultConstruct == true)
        this.MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

      this.MainWindowPosSz.SetValidPos(SystemParameters_VirtualScreenLeft,
                                       SystemParameters_VirtualScreenTop);
    }


    /// <summary>
    /// Get the path of the file or empty string if file does not exists on disk.
    /// </summary>
    /// <returns></returns>
    public string GetLastActivePath()
    {
      try
      {
        if (System.IO.File.Exists(this.LastActiveFile))
          return System.IO.Path.GetDirectoryName(this.LastActiveFile);
      }
      catch
      {
      }

      return string.Empty;
    }
    #endregion methods
  }
}
