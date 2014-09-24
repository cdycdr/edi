namespace Edi.Core.ViewModels
{
	/// <summary>
	/// AvalonDock base class viewm-model to support tool window views
	/// </summary>
	public class ToolViewModel : PaneViewModel, IToolWindow
	{
		private bool mIsVisible = true;

		/// <summary>
		/// Base constructor from nam of tool window item
		/// </summary>
		/// <param name="name">Name of tool window displayed in GUI</param>
		public ToolViewModel(string name)
		{
			Name = name;
			Title = name;
		}

		public string Name
		{
			get;
			private set;
		}

		#region IsVisible
		public bool IsVisible
		{
			get
			{
				return this.mIsVisible;
			}

			set
			{
				if (this.mIsVisible != value)
				{
					this.mIsVisible = value;
					RaisePropertyChanged(() => this.IsVisible);
				}
			}
		}

		#endregion

		/// <summary>
		/// Ensures the visibility of this toolwindow.
		/// </summary>
		/// <param name="isVisible"></param>
		public virtual void SetToolWindowVisibility(bool isVisible = true)
		{
			this.IsVisible = isVisible;
		}
	}
}
