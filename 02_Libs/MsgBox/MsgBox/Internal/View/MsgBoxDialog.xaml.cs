namespace MsgBox.Internal.View
{
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for WPFMessageBox.xaml
  /// 
  /// Source: http://www.codeproject.com/Articles/70223/Using-a-Service-Locator-to-Work-with-MessageBoxes
  /// </summary>
  internal partial class MsgBoxDialog : Window
  {
    [ThreadStatic]
    private static MsgBoxDialog mMessageBox;

    #region constructors
    public MsgBoxDialog()
    {
      this.InitializeComponent();

      Window w = MsgBoxDialog.GetOwnerWindow();

      if (w != null)
        this.Owner = w;
    }

    public static MsgBoxResult Show(string messageBoxText,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, string.Empty, string.Empty,
                  MsgBoxButtons.OK,
                  MsgBoxImage.Default,
                  btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, string.Empty, MsgBoxButtons.OK, MsgBoxImage.Default, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption, string details,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, details, MsgBoxButtons.OK, MsgBoxImage.Default, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption,
                                    MsgBoxButtons buttonOption,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, string.Empty, buttonOption, MsgBoxImage.Default, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption, string details,
                                    MsgBoxButtons buttonOption,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, details, buttonOption, MsgBoxImage.Default, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption, MsgBoxImage image,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, string.Empty, MsgBoxButtons.OK, image, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption, string details,
                                    MsgBoxImage image,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, details, MsgBoxButtons.OK, image, btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText, string caption,
                                    MsgBoxButtons buttonOption,
                                    MsgBoxImage image,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(messageBoxText, caption, string.Empty,
                  buttonOption,
                  image,
                  btnDefault,
                  helpLink, helpLinkTitle, helpLabel, navigateHelplinkMethod, enableCopyFunction);
    }

    public static MsgBoxResult Show(string messageBoxText,
                                    string caption,
                                    string details,
                                    MsgBoxButtons buttonOption,
                                    MsgBoxImage image,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      // Construct the message box viewmodel
      ViewModel.MsgBoxViewModel viewModel = new ViewModel.MsgBoxViewModel(messageBoxText,
                                                                          caption,
                                                                          details,
                                                                          buttonOption,
                                                                          image,
                                                                          btnDefault,
                                                                          helpLink, helpLinkTitle, navigateHelplinkMethod,
                                                                          enableCopyFunction);

     viewModel.HyperlinkLabel = helpLabel;

      // Construct the message box view and add the viewmodel to it
      MsgBoxDialog.mMessageBox = new MsgBoxDialog();

      MsgBoxDialog.mMessageBox.DataContext = viewModel;

      MsgBoxDialog.mMessageBox.ShowDialog();

      return viewModel.Result;
    }

    public static MsgBoxResult Show(Exception exp, string caption,
                                    MsgBoxButtons buttonOption,
                                    MsgBoxImage image,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      return Show(exp, "", caption, buttonOption, image,
                                    btnDefault,
                                    helpLink, helpLinkTitle, helpLabel,
                                    navigateHelplinkMethod,
                                    enableCopyFunction);
    }

    public static MsgBoxResult Show(Exception exp,
                                    string textMessage = "", string caption = "",
                                    MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                                    MsgBoxImage image = MsgBoxImage.Error,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      string sMess = MsgBox.Local.Strings.Unknown_Error_Message;

      string messageBoxText = string.Empty;

      try
      {
        // Write Message tree of inner exception into textual representation
        messageBoxText = exp.Message;

        Exception innerEx = exp.InnerException;

        for (int i = 0; innerEx != null; i++, innerEx = innerEx.InnerException)
        {
          string spaces = string.Empty;

          for (int j = 0; j < i; j++)
            spaces += "  ";

          messageBoxText += "\n" + spaces + "+->" + innerEx.Message;
        }

        // Label message tree with meaning ful info: "Error while reading file X."
        if (textMessage != null)
        {
          if (textMessage.Length > 0)
          {
            messageBoxText = string.Format("{0}\n\n{1}", textMessage, messageBoxText); 
          }
        }

        // Write complete stack trace info into details section
        sMess = exp.ToString();
      }
      catch
      {
      }

      // Construct the message box viewmodel
      ViewModel.MsgBoxViewModel viewModel = new ViewModel.MsgBoxViewModel(messageBoxText, caption,
                                                                          sMess,
                                                                          buttonOption, image, btnDefault,
                                                                          helpLink, helpLinkTitle, navigateHelplinkMethod,
                                                                          enableCopyFunction);

      viewModel.HyperlinkLabel = helpLabel;

      // Construct the message box view and add the viewmodel to it
      MsgBoxDialog.mMessageBox = new MsgBoxDialog();

      MsgBoxDialog.mMessageBox.DataContext = viewModel;

      try
      {
        bool? b = MsgBoxDialog.mMessageBox.ShowDialog();
      }
      catch
      {
      }

      return viewModel.Result;
    }

    public static MsgBoxResult Show(Window owner,
                                    string messageBoxText, string caption = "", 
                                    MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                                    MsgBoxImage image = MsgBoxImage.Error,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool enableCopyFunction = false)
    {
      // Construct the message box viewmodel
      ViewModel.MsgBoxViewModel viewModel = new ViewModel.MsgBoxViewModel(messageBoxText,
                                                                          caption,
                                                                          string.Empty, // details
                                                                          buttonOption,
                                                                          image,
                                                                          btnDefault,
                                                                          helpLink, helpLinkTitle, navigateHelplinkMethod,
                                                                          enableCopyFunction);

      viewModel.HyperlinkLabel = helpLabel;

      MsgBoxDialog.mMessageBox = new MsgBoxDialog() { Owner = owner }; // Construct the message box view and add the viewmodel for to it

      MsgBoxDialog.mMessageBox.DataContext = viewModel;

      MsgBoxDialog.mMessageBox.ShowDialog();

      return viewModel.Result;
    }
    #endregion constructors

    #region methods
    /// <summary>
    /// Remove the icon from the window chrome to give this window a message box like appearrance.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSourceInitialized(EventArgs e)
    {
      IconHelper.RemoveIcon(this);
    }

    /// <summary>
    /// Attempt to find the owner window for a message box
    /// </summary>
    /// <returns>Owner Window</returns>
    private static Window GetOwnerWindow()
    {
      Window owner = null;

      if (Application.Current != null)
      {
        foreach (Window w in Application.Current.Windows)
        {
          if (w != null)
          {
            if (w.IsActive)
            {
              owner = w;
              break;
            }
          }
        }
      }

      return owner;
    }
    #endregion methods
  }
}
