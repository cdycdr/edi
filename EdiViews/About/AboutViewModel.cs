namespace EdiViews.About
{
  using System.Reflection;
  using EdiViews.ViewModel.Base;
  using System.Collections.Generic;

  public class AboutViewModel : DialogViewModelBase
  {
    #region constructor
    public AboutViewModel()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to expose elements necessary to evaluate user input
    /// when the user completes his input (eg.: clicks OK in a dialog).
    /// </summary>
    private DialogViewModelBase mOpenCloseView;
    public DialogViewModelBase OpenCloseView
    {
      get
      {
        return this.mOpenCloseView;
      }

      private set
      {
        if (this.mOpenCloseView != value)
        {
          this.mOpenCloseView = value;

          this.NotifyPropertyChanged(() => this.OpenCloseView);
        }
      }
    }

    public string AppTitle
    {
      get
      {
        return Assembly.GetEntryAssembly().GetName().Name;
      }
    }

    public string SubTitle
    {
      get
      {
        return "A Text Editor";
      }
    }

    /// <summary>
    /// Gets the assembly copyright.
    /// </summary>
    /// <value>The assembly copyright.</value>
    public string AssemblyCopyright
    {
        get
        {
            // Get all Copyright attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            // If there aren't any Copyright attributes, return an empty string
            if (attributes.Length == 0)
                return string.Empty;

            // If there is a Copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    public string AppUrl
    {
      get
      {
        return "http://edi.codeplex.com/";
      }
    }

    public string AppUrlDisplayString
    {
      get
      {
        return "http://edi.codeplex.com/";
      }
    }

    public string AppVersion
    {
      get
      {
        return Assembly.GetEntryAssembly().GetName().Version.ToString();
      }
    }

    public string RunTimeVersion
    {
      get
      {
        return Assembly.GetEntryAssembly().ImageRuntimeVersion;
      }
    }

    public SortedList<string, string> Modules
    {
      get
      {
        SortedList<string, string> l = new SortedList<string, string>();

        foreach (AssemblyName assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies())
        {
          l.Add(assembly.Name, assembly.Name + ", Version=" + assembly.Version);
        }

        return l;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Initialize input states such that user can input information
    /// with a view based GUI (eg.: dialog)
    /// </summary>
    public void InitDialogInputData()
    {
      this.OpenCloseView = new EdiViews.ViewModel.Base.DialogViewModelBase();
    }
    #endregion methods
  }
}
