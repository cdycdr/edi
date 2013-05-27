namespace MsgBox
{
  using System;
  using System.Security;
  using System.Windows;

  /// <summary>
  /// Provide a WPF message box implementation that can be fully styled, templated, and themed.
  /// </summary>
  public static class Msg
  {
    static Msg()
    {
      ServiceInjector.InjectServices();
    }

    /// <summary>
    /// Retrieves a service object identified by <typeparamref name="TServiceContract"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">The type identifier of the service.</typeparam>
    public static TServiceContract GetService<TServiceContract>()
                  where TServiceContract : class
    {
      return ServiceContainer.Instance.GetService<TServiceContract>();
    }

    #region System.MessageBox replacements    
    /// <summary>
    /// Displays a message box that has a message and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText);    
    }


    /// <summary>
    /// Displays a message box that has a message and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText, string caption)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption);
    }

    /// <summary>
    /// Displays a message box that has a message and that returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Window owner, string messageBoxText)
    {
      return Msg.GetService<IMsgBoxService>().Show(owner, messageBoxText);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, and button; and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText, string caption, MsgBoxButtons button)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption, button);    
    }

    /// <summary>
    /// Displays a message box in front of the specified window.
    /// The message box displays a message and title bar caption; and it returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Window owner, string messageBoxText, string caption)
    {
      return Msg.GetService<IMsgBoxService>().Show(owner, messageBoxText, caption);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MsgBoxImage value that specifies the icon to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText, string caption, MsgBoxButtons button, MsgBoxImage icon)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption, button, icon);
    }

    /// <summary>
    /// Displays a message box in front of the specified window. The message box displays a message,
    /// title bar caption, and button; and it also returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MsgBoxButtons value that specifies which button or buttons to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Window owner, string messageBoxText, string caption, MsgBoxButtons button)
    {
      return Msg.GetService<IMsgBoxService>().Show(owner, messageBoxText, caption, button);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText, string caption, MsgBoxButtons button, MsgBoxImage icon, MsgBoxResult defaultResult)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption, button, icon, defaultResult);
    }

    /// <summary>
    /// Displays a message box in front of the specified window. The message box displays
    /// a message, title bar caption, button, and icon; and it also returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Window owner, string messageBoxText, string caption, MsgBoxButtons button, MsgBoxImage icon)
    {
      return Msg.GetService<IMsgBoxService>().Show(owner, messageBoxText, caption, button, icon);
    }

    /// <summary>
    /// Displays a message box in front of the specified window. The message box displays a message,
    /// title bar caption, button, and icon; and accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Window owner, string messageBoxText, string caption, MsgBoxButtons button, MsgBoxImage icon, MsgBoxResult defaultResult)
    {
      return Msg.GetService<IMsgBoxService>().Show(owner, messageBoxText, caption, button, icon, defaultResult);
    }
    #endregion System.MessageBox replacements    

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <param name="helpLink">Determines the address to browsed to when displaying a help link. This parameter can be a URI or string object.</param>
    /// <param name="helpLinkTitle">Determines the text for displaying a help link.
    /// By default the text is the toString() content of the <paramref name="helpLink"/>
    /// but it can also be a different text set here.</param>
    /// <param name="helpLinkLabel">Text label of the displayed hyperlink (if any)</param>
    /// <param name="navigateHelplinkMethod">Method to execute when the user clicked the hyperlink</param>
    /// <param name="showCopyMessage">Show a UI element (e.g. button) that lets the user copy the displayed message into the Windows clipboard</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText, string caption, MsgBoxButtons button, MsgBoxImage icon, MsgBoxResult defaultResult,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLinkLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool showCopyMessage = false)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption, button, icon, defaultResult,
                                                   helpLink, helpLinkTitle, helpLinkLabel, navigateHelplinkMethod, showCopyMessage);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="details">Displays a details section (e.g. expander) where a long message, such as, a stacktrace can be displayed within a scrollviewer.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <param name="helpLink">Determines the address to browsed to when displaying a help link. This parameter can be a URI or string object.</param>
    /// <param name="helpLinkTitle">Determines the text for displaying a help link.
    /// By default the text is the toString() content of the <paramref name="helpLink"/>
    /// but it can also be a different text set here.</param>
    /// <param name="helpLinkLabel">Text label of the displayed hyperlink (if any)</param>
    /// <param name="navigateHelplinkMethod">Method to execute when the user clicked the hyperlink</param>
    /// <param name="showCopyMessage">Show a UI element (e.g. button) that lets the user copy the displayed message into the Windows clipboard</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText,
                                    string caption = "",
                                    string details = "",
                                    MsgBoxButtons button = MsgBoxButtons.OK,
                                    MsgBoxImage icon = MsgBoxImage.Error,
                                    MsgBoxResult defaultResult = MsgBoxResult.OK,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLinkLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool showCopyMessage = false)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, caption, details, button, icon, defaultResult,
                                                   helpLink, helpLinkTitle, helpLinkLabel, navigateHelplinkMethod, showCopyMessage);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="ex">Exception object (details: stacktrace and messages are displayed in details section)</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <param name="helpLink">Determines the address to browsed to when displaying a help link. This parameter can be a URI or string object.</param>
    /// <param name="helpLinkTitle">Determines the text for displaying a help link.
    /// By default the text is the toString() content of the <paramref name="helpLink"/>
    /// but it can also be a different text set here.</param>
    /// <param name="helpLinkLabel">Text label of the displayed hyperlink (if any)</param>
    /// <param name="navigateHelplinkMethod">Method to execute when the user clicked the hyperlink</param>
    /// <param name="showCopyMessage">Show a UI element (e.g. button) that lets the user copy the displayed message into the Windows clipboard</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Exception ex,
                                    string caption = "",
                                    MsgBoxButtons button = MsgBoxButtons.OK,
                                    MsgBoxImage icon = MsgBoxImage.Error,
                                    MsgBoxResult defaultResult = MsgBoxResult.OK,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLinkLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool showCopyMessage = false)
    {
      return Msg.GetService<IMsgBoxService>().Show(ex, caption, button, icon, defaultResult,
                                                   helpLink, helpLinkTitle, helpLinkLabel, navigateHelplinkMethod, showCopyMessage);
    }


    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="btnDefault">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <param name="helpLink">Determines the address to browsed to when displaying a help link. This parameter can be a URI or string object.</param>
    /// <param name="helpLinkTitle">Determines the text for displaying a help link.
    /// By default the text is the toString() content of the <paramref name="helpLink"/>
    /// but it can also be a different text set here.</param>
    /// <param name="helpLinkLabel">Text label of the displayed hyperlink (if any)</param>
    /// <param name="navigateHelplinkMethod">Method to execute when the user clicked the hyperlink</param>
    /// <param name="showCopyMessage">Show a UI element (e.g. button) that lets the user copy the displayed message into the Windows clipboard</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(string messageBoxText,
                                    MsgBoxResult btnDefault = MsgBoxResult.None,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLinkLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool showCopyMessage = false)
    {
      return Msg.GetService<IMsgBoxService>().Show(messageBoxText, btnDefault, helpLink,
                                                   helpLinkTitle, helpLinkLabel, navigateHelplinkMethod, showCopyMessage);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon;
    /// and that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="ex">Exception object (details: stacktrace and messages are displayed in details section)</param>
    /// <param name="messageBoxText">A System.String that specifies the details to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A Msg.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A Msg.MessageBoxImage value that specifies the icon to display.</param>
    /// <param name="defaultResult">A MsgBox.MsgBoxResult value that specifies the default result of the message box.</param>
    /// <param name="helpLink">Determines the address to browsed to when displaying a help link. This parameter can be a URI or string object.</param>
    /// <param name="helpLinkTitle">Determines the text for displaying a help link.
    /// By default the text is the toString() content of the <paramref name="helpLink"/>
    /// but it can also be a different text set here.</param>
    /// <param name="helpLinkLabel">Text label of the displayed hyperlink (if any)</param>
    /// <param name="navigateHelplinkMethod">Method to execute when the user clicked the hyperlink</param>
    /// <param name="showCopyMessage">Show a UI element (e.g. button) that lets the user copy the displayed message into the Windows clipboard</param>
    /// <returns>A MsgBox.MsgBoxResult value that specifies which message box button is clicked by the user.</returns>
    [SecurityCritical]
    public static MsgBoxResult Show(Exception ex,
                                    string messageBoxText = "",
                                    string caption = "",
                                    MsgBoxButtons button = MsgBoxButtons.OK,
                                    MsgBoxImage icon = MsgBoxImage.Error,
                                    MsgBoxResult defaultResult = MsgBoxResult.OK,
                                    object helpLink = null,
                                    string helpLinkTitle = "",
                                    string helpLinkLabel = "",
                                    Func<object, bool> navigateHelplinkMethod = null,
                                    bool showCopyMessage = false)
    {
      return Msg.GetService<IMsgBoxService>().Show(ex, messageBoxText, caption, button, icon, defaultResult,
                                                   helpLink, helpLinkTitle, helpLinkLabel, navigateHelplinkMethod, showCopyMessage);
    }

  }
}
