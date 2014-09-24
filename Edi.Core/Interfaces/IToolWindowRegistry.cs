namespace Edi.Core.Interfaces
{
	using System.Collections.ObjectModel;
	using Edi.Core.ViewModels;

	public interface IToolWindowRegistry
	{
		ObservableCollection<ToolViewModel> Tools { get; }

		void RegisterTool(ToolViewModel newTool);
	}
}
