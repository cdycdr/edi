namespace FileListView.ViewModels
{
  /// <summary>
  /// The Viewmodel for filter item displayed in list of filters
  /// </summary>
  public class FilterItemViewModel : Base.ViewModelBase
  {
    #region fields
    private string mFilterText;
    private string mFilterDisplayName;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FilterItemViewModel(string filter = "")
      : this()
    {
      if (string.IsNullOrEmpty(filter) == false)
      {
        this.mFilterText = filter;
      }
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FilterItemViewModel(string name, string extensions)
      : this()
    {
      this.FilterDisplayName = name;
      this.FilterText = extensions;
    }

    /// <summary>
    /// Protected statndard class constructor
    /// (Consumers of this class shall use the parameterized version).
    /// </summary>
    protected FilterItemViewModel()
    {
      this.FilterDisplayName = string.Empty;
      this.FilterText = "*";
    }
    #endregion constructor
    
    #region properties
    /// <summary>
    /// Gets the regular expression based filter string eg: '*.exe'.
    /// </summary>
    public string FilterText
    {
      get
      {
        return this.mFilterText;
      }

      set
      {
        if (this.mFilterText != value)
        {
          this.mFilterText = value;
          this.NotifyPropertyChanged(() => this.FilterText);
        }
      }
    }

    /// <summary>
    /// Gets the name for this filter
    /// (human readable for display in tool tip or label).
    /// </summary>
    public string FilterDisplayName
    {
      get
      {
        return this.mFilterDisplayName;
      }

      set
      {
        if (this.mFilterDisplayName != value)
        {
          this.mFilterDisplayName = value;
          this.NotifyPropertyChanged(() => this.FilterDisplayName);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Standard method to display contents of this class.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.FilterText;
    }
    #endregion methods
  }
}
