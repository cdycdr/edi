namespace Edi.ViewModel
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using Edi.Core.App.Events;
  using EdiViews.ViewModels;
  using SimpleControls.Command;
	using Xceed.Wpf.AvalonDock;

  /// <summary>
  /// Class implements a viewmodel to support the
  /// <seealso cref="AvalonDockLayoutSerializer"/>
  /// attached behavior which is used to implement
  /// load/save of layout information on application
  /// start and shut-down.
  /// </summary>
  public class AvalonDockLayoutViewModel
  {
    #region fields
    private RelayCommand<object> mLoadLayoutCommand = null;
    private RelayCommand<object> mSaveLayoutCommand = null;

		private readonly Guid mLayoutID;

    private string mDirAppData = string.Empty, mLayoutFileName = string.Empty;
		#endregion fields

		#region properties
		/// <summary>
		/// Hidden class constructor
		/// </summary>
		//// [ImportingConstructor]
		public AvalonDockLayoutViewModel(string dirAppData, string layoutFileName)
		{
      this.mDirAppData = dirAppData;
      this.mLayoutFileName = layoutFileName;

			this.mLayoutID = Guid.NewGuid();
			this.ViewProperties = new AvalonDockViewProperties();

			this.ViewProperties.InitialzeInstance();
		}

		/// <summary>
		/// Gets the layout id for the AvalonDock Layout that is associated with this viewmodel.
		/// This layout id is a form of identification between viewmodel and view to identify whether
		/// a given event aggregated message is for a given recipient or not.
		/// </summary>
		public Guid LayoutID
		{
			get
			{
				return this.mLayoutID;
			}
		}

		public AvalonDockViewProperties ViewProperties { get; set; }

    #region command properties
    /// <summary>
    /// Implement a command to load the layout of an AvalonDock-DockingManager instance.
    /// This layout defines the position and shape of each document and tool window
    /// displayed in the application.
    /// 
    /// Parameter:
    /// The command expects a reference to a <seealso cref="DockingManager"/> instance to
    /// work correctly. Not supplying that reference results in not loading a layout (silent return).
    /// </summary>
    public ICommand LoadLayoutCommand
    {
      get
      {
        if (this.mLoadLayoutCommand == null)
        {
          this.mLoadLayoutCommand = new RelayCommand<object>((p) =>
          {
            DockingManager docManager = p as DockingManager;

            if (docManager == null)
              return;

            this.LoadDockingManagerLayout(docManager);
          });
        }

        return this.mLoadLayoutCommand;
      }
    }

    /// <summary>
    /// Implements a command to save the layout of an AvalonDock-DockingManager instance.
    /// This layout defines the position and shape of each document and tool window
    /// displayed in the application.
    /// 
    /// Parameter:
    /// The command expects a reference to a <seealso cref="string"/> instance to
    /// work correctly. The string is supposed to contain the XML layout persisted
    /// from the DockingManager instance. Not supplying that reference to the string
    /// results in not saving a layout (silent return).
    /// </summary>
    public ICommand SaveLayoutCommand
    {
      get
      {
        if (this.mSaveLayoutCommand == null)
        {
          this.mSaveLayoutCommand = new RelayCommand<object>((p) =>
          {
            string xmlLayout = p as string;

            if (xmlLayout == null)
              return;

            this.SaveDockingManagerLayout(xmlLayout);
          });
        }

        return this.mSaveLayoutCommand;
      }
    }
    #endregion command properties
		#endregion properties

    #region methods
    #region LoadLayout
    /// <summary>
    /// Loads the layout of a particular docking manager instance from persistence
    /// and checks whether a file should really be reloaded (some files may no longer
    /// be available).
    /// </summary>
    /// <param name="docManager"></param>
    private void LoadDockingManagerLayout(DockingManager docManager)
    {
      ////string lastActiveFile = SettingsManager.Instance.SessionData.LastActiveFile;
      string layoutFileName = System.IO.Path.Combine(this.mDirAppData, this.mLayoutFileName);

      if (System.IO.File.Exists(layoutFileName) == false)
        return;

			this.LoadDockingManagerLayout(layoutFileName, this.LayoutID);
		}

		/// <summary>
		/// Loads the layout of a particular docking manager instance from persistence
		/// and checks whether a file should really be reloaded (some files may no longer
		/// be available).
		/// </summary>
		private void LoadDockingManagerLayout(string layoutFileName, Guid layoutID)
		{
			try
			{
				string sTaskError = string.Empty;

				Task taskToProcess = null;
				taskToProcess = Task.Factory.StartNew<string>((stateObj) =>
				{
					// Begin Aysnc Task
					////this.mParent.IsBusy = true;

					string xmlWorkspaces = string.Empty;
					////WorkspaceItemsCollection workcoll = null;

					try
					{
						////layoutFileName = System.IO.Path.Combine(this.mParent.DirAppData, AvalonDockLayoutViewModel.WorkspaceLayoutFileName);
						////
						////if (System.IO.File.Exists(layoutFileName) == false)
						////{
						////	workcoll = new WorkspaceItemsCollection(this.LoadWorkspaceDefaultDefinition());
						////	this.mParent.AvailableWorkspaces.ReloadWorkspaces(workcoll);
						////
						////	return string.Empty;
						////}

						using (FileStream fs = new FileStream(layoutFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							using (StreamReader reader = ICSharpCode.AvalonEdit.Utils.FileReader.OpenStream(fs, Encoding.Default))
							{
								xmlWorkspaces = reader.ReadToEnd();
							}
						}

						//// workcoll = WorkspaceItemsCollection.DecodeXmlWorkspaceLayout(xmlWorkspaces);
						//// this.mParent.AvailableWorkspaces.ReloadWorkspaces(workcoll);
						LoadLayoutEvent.Instance.Publish(new LoadLayoutEventArgs(xmlWorkspaces, layoutID));
					}
					catch (OperationCanceledException exp)
					{
						throw exp;
					}
					catch (Exception except)
					{
						throw except;
					}
					finally
					{
					}

					return xmlWorkspaces;                     // End of async task

				}, null);
			}
			catch (Exception exp)
			{
				throw exp;
			}
		}
    #endregion LoadLayout

    #region SaveLayout
    private void SaveDockingManagerLayout(string xmlLayout)
    {
      // Create XML Layout file on close application (for re-load on application re-start)
      if (xmlLayout == null)
        return;

      string fileName = System.IO.Path.Combine(this.mDirAppData, this.mLayoutFileName);

      File.WriteAllText(fileName, xmlLayout);
    }
    #endregion SaveLayout
    #endregion methods
  }
}
