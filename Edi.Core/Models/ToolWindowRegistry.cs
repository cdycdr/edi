namespace Edi.Core.Models
{
	using System.Collections.ObjectModel;
	using System.ComponentModel.Composition;
	using Edi.Core.Interfaces;
	using Edi.Core.ViewModels;

	[Export(typeof(IToolWindowRegistry))]
	public class ToolWindowRegistry : IToolWindowRegistry
	{
		private ObservableCollection<ToolViewModel> mItems = null;

		public ToolWindowRegistry()
		{
			this.mItems = new ObservableCollection<ToolViewModel>();
		}

		public ObservableCollection<ToolViewModel> Tools
		{
			get
			{
				return this.mItems;
			}
		}

		public void RegisterTool(ToolViewModel newTool)
		{
			this.mItems.Add(newTool);
		}
	}
}
