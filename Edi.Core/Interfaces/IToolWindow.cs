namespace Edi.Core.ViewModels
{
	using Edi.Core.Interfaces.Enums;

	public interface IToolWindow
	{
		#region properties
		string Name	{ get; }

		bool IsVisible { get; }

		PaneLocation PreferredLocation { get; }
		double PreferredWidth { get; }
		double PreferredHeight { get; }
		#endregion properties

		#region methods
		void SetToolWindowVisibility(bool isVisible = true);
		#endregion methods
	}
}
