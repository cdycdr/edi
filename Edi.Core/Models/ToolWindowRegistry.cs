namespace Edi.Core.Models
{
	using System.Collections.ObjectModel;
	using System.ComponentModel.Composition;
	using Edi.Core.Interfaces;
	using Edi.Core.ViewModels;
	using EdiApp.Events;

	/// <summary>
	/// Class to register and manage all tool windows in one common place.
	/// </summary>
	[Export(typeof(IToolWindowRegistry))]
	public class ToolWindowRegistry : IToolWindowRegistry
	{
		#region fields
		private readonly ObservableCollection<ToolViewModel> mItems = null;
		#endregion fields

		#region contructors
		public ToolWindowRegistry()
		{
			this.mItems = new ObservableCollection<ToolViewModel>();
		}
		#endregion contructors

		#region properties
		public ObservableCollection<ToolViewModel> Tools
		{
			get
			{
				return this.mItems;
			}
		}

		public IOutput Output { get; set; }
		#endregion properties

		#region methods
		public void RegisterTool(ToolViewModel newTool)
		{
			this.mItems.Add(newTool);

			// Publish the fact that we have registered a new tool window instance
			RegisterToolWindowEvent.Instance.Publish(new RegisterToolWindowEventArgs(newTool));
		}
		#endregion methods
	}
}
