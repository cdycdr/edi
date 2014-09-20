namespace Edi
{
  using System;
  using System.ComponentModel.Composition;
  using EdiApp.Events;
  using Edi.Core.Interfaces;
  using Edi.Core.View;
  using EdiViews.ViewModel;
  using Microsoft.Practices.Prism.PubSubEvents;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
	[Export]
  public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow, Edi.Core.Interfaces.ILayoutableWindow
  {
    #region constructors
		[ImportingConstructor]
    public MainWindow(IAvalonDockLayoutViewModel av, IApplicationViewModel appVM)
    {
      this.InitializeComponent();

      this.dockView.SetTemplates(av.ViewProperties.SelectPanesTemplate,
																 av.ViewProperties.DocumentHeaderTemplate,
																 av.ViewProperties.SelectPanesStyle,
																 av.ViewProperties.LayoutInitializer,
																 av.LayoutID);

      // Register these methods to receive PRISM event notifications
      LoadLayoutEvent.Instance.Subscribe(this.OnLoadLayout,
                                         ThreadOption.PublisherThread, true,
                                         s => s.LayoutID == av.LayoutID);

      SynchronousEvent<SaveLayoutEventArgs>.Instance.Subscribe(this.OnSaveLayout);

			// subscribe to close event messing to application viewmodel
			this.Closing += appVM.OnClosing;

			// When the ViewModel asks to be closed, close the window.
			// Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
			appVM.RequestClose += delegate
			{
				// Save session data and close application
				appVM.OnClosed(this);
			};
    }
    #endregion constructors

    #region properties
    /// <summary>
    /// Gets/Sets the LayoutId of the AvalonDocking Manager layout used to manage
    /// the positions and layout of documents and tool windows within the AvalonDock
    /// view.
    /// </summary>
    public Guid LayoutID
    {
      get
      {
        return (this.dockView != null ? this.dockView.LayoutID : Guid.Empty);
      }
    }

    /// <summary>
    /// Gets the current AvalonDockManager Xml layout and returns it as a string.
    /// </summary>
    public string CurrentADLayout
    {
      get
      {
        return (this.dockView != null ? this.dockView.CurrentADLayout : string.Empty);
      }
    }
    #endregion properties

    #region Workspace Layout Management
    /// <summary>
    /// Is executed when PRISM sends a Xml layout string notification
    /// via a sender which could be a viewmodel that wants to receive
    /// the load the <seealso cref="LoadLayoutEvent"/>.
    /// </summary>
    /// <param name="args"></param>
    public void OnLoadLayout(LoadLayoutEventArgs args)
    {
      if (this.dockView != null)
        this.dockView.OnLoadLayout(args);
    }

    /// <summary>
    /// Is executed when PRISM sends a <seealso cref="SynchronousEvent"/> notification
    /// that was initiallized by a third party (viewmodel).
    /// </summary>
    /// <param name="param">Can be used to return a result of this event</param>
    internal void OnSaveLayout(SaveLayoutEventArgs param)
    {
      if (this.dockView != null)
        this.dockView.OnSaveLayout(param);
    }
    #endregion Workspace Layout Management
  }
}
